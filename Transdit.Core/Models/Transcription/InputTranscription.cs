using AngleSharp.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Transcription
{
    public class InputTranscription
    {
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string YoutubeUrl { get; set; } = string.Empty;
        public int Speakers { get; set; }
        public float HintsImpact { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public IEnumerable<string> Hints { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AdditionalLanguages { get; set; } = Enumerable.Empty<string>();
        public bool Save { get; set; }
        public bool IsConverted { get; set; }
        public double LengthInSeconds { get; set; }
        [JsonIgnore]
        public string ContentType { get; set; } = string.Empty;
        [JsonIgnore]
        public int Channels { get; set; }
        [JsonIgnore]
        public int SambleRate { get; set; }
        [JsonIgnore]
        public string StorageUri { get; set; } = string.Empty;
        [JsonIgnore]
        public string StorageFileName { get; set; } = string.Empty;
        [JsonIgnore]
        public string PhysicalPath { get; set; } = string.Empty;
        [JsonIgnore]
        public string ConvertedPath { get; set; } = string.Empty;

        [JsonIgnore]
        public bool HasTimeRange { get => StartTime.HasValue || EndTime.HasValue; }
        [JsonIgnore]
        public bool IsYoutubeMidia { get 
            {
                if (string.IsNullOrEmpty(YoutubeUrl))
                    return false;

                bool isUri = Uri.TryCreate(YoutubeUrl, UriKind.Absolute, out Uri uri);
                if (!isUri)
                    return false;
                return uri.Host.ToLower() == "www.youtube.com";
            } }
        [JsonIgnore]
        public bool IsLongRunning { get => TimeSpan.FromSeconds(LengthInSeconds) > TimeSpan.FromMinutes(1); }
        [JsonIgnore]
        public bool MultipleSpeakers { get => Speakers > 1; }
        [JsonIgnore]
        public bool HasHints { get => Hints?.Count() >= 1; }
        [JsonIgnore]
        public bool HasAdditionalLanguages { get => AdditionalLanguages?.Count() >= 1; }
    }
}
