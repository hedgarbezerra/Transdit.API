using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Transcriptions
{
    public class OutTranscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string InputedFileName { get; set; } = string.Empty;
        public string StorageFileName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public long LengthInSeconds { get; set; }
        public DateTime Date { get; set; }
    }
}
