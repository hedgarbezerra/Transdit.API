using MediaToolkit.Model;
using MediaToolkit;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Transcription;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text;
using Transdit.Core.Domain;

namespace Transdit.Services.Common.Convertion
{
    public class TranscriptionMediaConvertion : ITranscriptionMediaConvertion
    {
        private readonly ILogger<TranscriptionMediaConvertion> _logger;
        private readonly string _ffmpegPath;
        private readonly string _tempFilesFolderPath;
        private readonly string _tempConvertedFilesFolderPath;
        public string TempFolderPath => _tempFilesFolderPath;

        public string TempConvertedFolderPath => _tempConvertedFilesFolderPath;
        public TranscriptionMediaConvertion(string wwwrootFolder, ILogger<TranscriptionMediaConvertion> logger)
        {
            _ffmpegPath = Path.Combine(wwwrootFolder, "FFMpeg", "ffmpeg.exe");
            _tempFilesFolderPath = Path.Combine(wwwrootFolder, "Temp", "Media");
            _tempConvertedFilesFolderPath = Path.Combine(wwwrootFolder, "Temp", "Media", "Converted");

            try
            {
                Directory.CreateDirectory(_tempFilesFolderPath);
                Directory.CreateDirectory(_tempConvertedFilesFolderPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao criar pastas temporarias de conversão: {ex.Message}", ex.StackTrace);
            }
            _logger = logger;
        }

        public async Task<bool> Convert(InputTranscription transcription)
        {
            var outputPath = Path.Combine(_tempConvertedFilesFolderPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".flac");
            try
            {
                string args = GetArguments(transcription, outputPath);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    string errorOutput = await process.StandardError.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        _logger.LogError(errorOutput);
                    }
                }
                transcription.ConvertedPath = outputPath;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.HResult, ex.StackTrace);
                return false;
            }
        }
        public async Task<(bool, string)> Convert(string filePath)
        {
            var outputPath = Path.Combine(_tempConvertedFilesFolderPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".flac");
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = $"-i \"{filePath}\" -c:a flac -ar 16000 -f flac \"{outputPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    string errorOutput = await process.StandardError.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        _logger.LogError(errorOutput);
                    }
                }
                return (true, outputPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.HResult, ex.StackTrace);
                return (false, string.Empty);
            }
        }

        public void GetMetadata(InputTranscription transcription, string path)
        {
            if (!File.Exists(path))
                return;

            var file = new MediaFile(path);

            using (var engine = new Engine(_ffmpegPath))
            {
                engine.GetMetadata(file);
            }
            if (file.Metadata is null)
                return;

            transcription.LengthInSeconds = file.Metadata.Duration.TotalSeconds;
            transcription.ContentType = file.Metadata.AudioData.Format;
            transcription.SambleRate = System.Convert.ToInt16(file.Metadata.AudioData.SampleRate.ToLower().Replace("hz", ""));
            transcription.Channels = file.Metadata.AudioData.ChannelOutput == "stereo" ? 2 : 1;

        }

        private string GetArguments(InputTranscription transcription, string outputPath)
        {
            var argsBuilder = new StringBuilder($"-i \"{transcription.PhysicalPath}\"");
            if (transcription.HasTimeRange)
            {
                if (transcription.StartTime.HasValue && transcription.StartTime.Value.TotalSeconds > 0)
                    argsBuilder.Append($" -ss {transcription.StartTime.Value}");

                if(transcription.EndTime.HasValue && transcription.EndTime.Value.TotalSeconds > 0)
                    argsBuilder.Append($" -to {transcription.EndTime.Value}");
            }
            argsBuilder.Append($" -c:a flac -ar 16000 -f flac \"{outputPath}\"");
            return argsBuilder.ToString();
        }
    }
}