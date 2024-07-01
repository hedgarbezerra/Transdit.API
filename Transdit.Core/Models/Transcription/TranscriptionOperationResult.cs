using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Transcription
{
    public class TranscriptionOperationResult : DefaultControllerResponse<TranscriptionList>
    {
        public string FileName { get; set; } = string.Empty;
        public string StorageUri { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class TranscriptionList : List<TranscriptionItem>
    {
        public TranscriptionList()
        {            
        }
    }
    public class TranscriptionItem
    {
        public float Precision { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SpeakerTag { get; set; }
        public double StartTimeSeconds { get; set; }
        public double EndTimeSeconds { get; set; }
    }
}
