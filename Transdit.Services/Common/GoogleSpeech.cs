using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Logging;
using Google.Cloud.Speech.V1;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Transcription;
using Transdit.Utilities.Globalization;
using static Google.Cloud.Speech.V1.RecognitionConfig.Types;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Transdit.Services.Common
{
    [ExcludeFromCodeCoverage]
    public class GoogleSpeech : IGoogleSpeech
    {
        private ILogger<GoogleSpeech> _logger;
        private SpeechClient _speechClient;
        private readonly ITranscriptionMediaConvertion _mediaConvertion;
        private readonly IStringLocalizer<Languages> _stringLocalizer;

        private readonly Dictionary<string, AudioEncoding> _mediaTypeToEncoding = new Dictionary<string, AudioEncoding>
        {
            { ".flac", AudioEncoding.Flac },
            { ".ogg", AudioEncoding.OggOpus },
        };
        public GoogleSpeech(string credentialsJson, ILogger<GoogleSpeech> logger, ITranscriptionMediaConvertion mediaConvertion, IStringLocalizer<Languages> stringLocalizer)
        {
            var credential = GoogleCredential.FromJson(credentialsJson);
            var builder = new SpeechClientBuilder()
            {
                GoogleCredential = credential
            };

            _speechClient = builder.Build();
            _logger = logger;
            _mediaConvertion = mediaConvertion;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<RepeatedField<SpeechRecognitionResult>> Transcribe(string storageUri, string lang, CancellationToken cancellationToken = default)
        {
            RecognitionAudio audio = RecognitionAudio.FromStorageUri(storageUri);
            var flacDefaultConfig = new RecognitionConfig()
            {
                Encoding = AudioEncoding.Flac,
                AudioChannelCount = 2,
                SampleRateHertz = 16000,
                LanguageCode = lang,
                MaxAlternatives = 2,
                EnableWordConfidence = true,
                EnableAutomaticPunctuation = true,
                ProfanityFilter = false,
                EnableWordTimeOffsets = true
            };

            return await Execute(flacDefaultConfig, audio, cancellationToken, true);
        }
        public async Task<RepeatedField<SpeechRecognitionResult>> Transcribe(InputTranscription toTranscribe, CancellationToken cancellationToken = default, bool isLongRunning = false)
        {
            RecognitionAudio audio = isLongRunning ? RecognitionAudio.FromStorageUri(toTranscribe.StorageUri) : GetInputFile(toTranscribe);

            var config = GetRecognitionConfig(toTranscribe);

            return await Execute(config, audio, cancellationToken, isLongRunning);
        }
        public async Task Transcribe()
        {
            //TODO: Em algum momento entender como fazer transcrição por streaming
            //var callSettings = new CallSettings(d);
            //var operation = _speechClient.StreamingRecognize();
            //var message = new StreamingRecognizeRequest() { StreamingConfig = new StreamingRecognitionConfig { a } }
            //operation.WriteAsync()
        }
        public TranscriptionList SpeechResponseToTranscriptionList(RepeatedField<SpeechRecognitionResult> responses)
        {
            var result = new TranscriptionList();
            foreach (var response in responses)
            {
                var mostAccurate = response.Alternatives.FirstOrDefault(x => response.Alternatives.Max(y => y.Confidence) == x.Confidence);
                if (mostAccurate is not null)
                {
                    var previousItem = result.LastOrDefault();
                    var startTime = previousItem is null ? TimeSpan.Zero : TimeSpan.FromSeconds(previousItem.EndTimeSeconds);
                    var endTime = response.ResultEndTime.ToTimeSpan();
                    var conteudo = new TranscriptionItem()
                    {
                        Precision = mostAccurate.Confidence,
                        Text = mostAccurate.Transcript,
                        StartTimeSeconds = startTime.TotalSeconds,
                        EndTimeSeconds = endTime.TotalSeconds
                    };
                    result.Add(conteudo);
                }
            }
            return result;
        }

        public string SpeechResponseToString(RepeatedField<SpeechRecognitionResult> responses, bool detailed = false)
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan previousStartTime = TimeSpan.Zero;
            foreach (var response in responses)
            {
                var mostAccurate = response.Alternatives.FirstOrDefault(x => response.Alternatives.Max(y => y.Confidence) == x.Confidence);
                if (mostAccurate is not null)
                {
                    if (detailed)
                    {                       
                        var endTime = response.ResultEndTime.ToTimeSpan();

                        sb.AppendLine($"{previousStartTime.ToString("c")} - {endTime.ToString("c")}");
                        previousStartTime = endTime;
                    }
                    sb.AppendLine($"({mostAccurate.Confidence * 100}%) - {mostAccurate.Transcript}");
                }
            }
            return sb.ToString();
        }

        private RecognitionAudio GetInputFile(InputTranscription toTranscribe)
        {
            string pathToFile = toTranscribe.IsConverted 
                ? Path.Combine(_mediaConvertion.TempConvertedFolderPath, toTranscribe.FileName) 
                : Path.Combine(_mediaConvertion.TempFolderPath, toTranscribe.FileName);

            return RecognitionAudio.FromFile(pathToFile);
        }

        private async Task<RepeatedField<SpeechRecognitionResult>> Execute(RecognitionConfig config, RecognitionAudio audio, CancellationToken cancellationToken, bool isLongRunning = false)
        {
            try
            {
                if (isLongRunning)
                {
                    var operation = await _speechClient.LongRunningRecognizeAsync(config, audio, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        operation.Cancel();

                    var completedOperation = await operation.PollUntilCompletedAsync();

                    if (completedOperation == null)
                    {
                        throw new Exception("The operation was not completed.");
                    }

                    if (completedOperation.IsFaulted)
                        throw completedOperation.Exception;

                    if (completedOperation.Result == null)
                        throw new Exception("The operation returned a null result.");

                    return completedOperation.Result.Results;
                }
                else
                {
                    var result = await _speechClient.RecognizeAsync(config, audio, cancellationToken);
                    return result.Results;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);

                return default(RepeatedField<SpeechRecognitionResult>);
            }

        }
        private RecognitionConfig GetRecognitionConfig(InputTranscription toTranscribe)
        {
            double speakers = toTranscribe.Speakers;
            int minSpeakers = (int)Math.Ceiling(speakers / 2);
            var diarizationConfig = toTranscribe.MultipleSpeakers ? new SpeakerDiarizationConfig()
            {
                EnableSpeakerDiarization = true,
                MaxSpeakerCount = toTranscribe.Speakers,
                MinSpeakerCount = minSpeakers
            }
            : null;

            AudioEncoding audioEncoding = GetAudioEncoding(toTranscribe.ContentType);
            var config = new RecognitionConfig
            {
                DiarizationConfig = diarizationConfig,
                Encoding = audioEncoding,
                AudioChannelCount = toTranscribe.Channels,
                SampleRateHertz = toTranscribe.SambleRate,
                LanguageCode = toTranscribe.Language,
                MaxAlternatives = 1,
                EnableWordConfidence = false,
                EnableAutomaticPunctuation = true,
                ProfanityFilter = false,
                EnableWordTimeOffsets = true,
            };

            if (toTranscribe.HasHints)
            {
                var context = new SpeechContext() { Boost = toTranscribe.HintsImpact };
                context.Phrases.Add(toTranscribe.Hints);
                config.SpeechContexts.Add(context);
            }

            if (toTranscribe.HasAdditionalLanguages)
                config.AlternativeLanguageCodes.Add(toTranscribe.AdditionalLanguages);

            return config;
        }
        public AudioEncoding GetAudioEncoding(string contentType)
        {
            var invariantString = contentType.ToUpper();
            if (_mediaTypeToEncoding.TryGetValue(invariantString, out var encoding))
                return encoding;

            return AudioEncoding.EncodingUnspecified;
        }
        public Dictionary<string, AudioEncoding> GetExpectedAudioEncodings() => _mediaTypeToEncoding;
        public IEnumerable<string> GetEncodingTypes()
            => Enum.GetValues(typeof(AudioEncoding)).Cast<string>().ToList();
        public Dictionary<string, IEnumerable<string>> GetLanguageCodes() => 
            typeof(LanguageCodes).GetNestedTypes().ToDictionary(c => _stringLocalizer.GetString(c.Name).Value, c => c.GetFields().Select(p => p.GetValue(p).ToString()));
    }
}