using Google.Cloud.Speech.V1;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Transcription;

namespace Transdit.Core.Contracts
{
    public interface IGoogleSpeech
    {
        Task<RepeatedField<SpeechRecognitionResult>> Transcribe(InputTranscription toTranscribe, CancellationToken cancellationToken = default, bool isLongRunning = false);
        string SpeechResponseToString(RepeatedField<SpeechRecognitionResult> response, bool detailed = false);
        TranscriptionList SpeechResponseToTranscriptionList(RepeatedField<SpeechRecognitionResult> response);
        IEnumerable<string> GetEncodingTypes();
        Dictionary<string, IEnumerable<string>> GetLanguageCodes();
        Dictionary<string, RecognitionConfig.Types.AudioEncoding> GetExpectedAudioEncodings();
        RecognitionConfig.Types.AudioEncoding GetAudioEncoding(string contentType);
        Task<RepeatedField<SpeechRecognitionResult>> Transcribe(string storageUri, string lang, CancellationToken cancellationToken = default);
    }
}
