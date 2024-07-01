using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Transdit.Core.Domain
{
    public class Transcription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string InputedFileName { get; set; } = string.Empty;
        public string StorageFileName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public TimeSpan Usage { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
    }
}
