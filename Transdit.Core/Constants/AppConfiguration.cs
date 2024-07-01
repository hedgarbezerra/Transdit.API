using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Constants
{
    [ExcludeFromCodeCoverage]
    public class AppConfiguration
    {
        public string CryptoKey { get; set; } = string.Empty;
        public string WebAppUrl { get; set; } = string.Empty;
        public long FileSizeLimit { get; set; }
        public string CleanUpTaskMinutesSpan{ get; set; } = string.Empty;
        public TimeSpan CleanUpTaskSpan
        {
            get
            {
                if (string.IsNullOrEmpty(CleanUpTaskMinutesSpan))
                    return TimeSpan.Zero;
                var span = double.Parse(CleanUpTaskMinutesSpan);
                return TimeSpan.FromMinutes(span);
            }
        }
        public IEnumerable<string> PermitedFileTypes { get => new List<string>()
        {
            ".avi",
            ".mp4",
            ".wmv",
            ".mov",
            ".mkv",
            ".flv",
            ".m4v",
            ".webm",
            ".ogv",
            ".gifv",
            ".mp3",
            ".wav",
            ".wma",
            ".aac",
            ".flac",
            ".ogg",
            ".m4a",
            ".opus" };
        }
        public static IEnumerable<string> LocalizableLanguages { get; set; } = new[] { "en-US", "pt-BR", "es-ES" };
    }
}
