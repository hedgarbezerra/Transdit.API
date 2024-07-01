using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Enums;

namespace Transdit.Core.Models.Transcription
{
    public class ExportInput
    {
        public string Content { get; set; } = string.Empty;
        public EFileConvertionTarget Format { get; set; }
    }
}
