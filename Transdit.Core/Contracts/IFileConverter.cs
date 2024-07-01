using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Enums;
using Transdit.Core.Models.Transcription;

namespace Transdit.Core.Contracts
{
    public interface IFileConverter
    {
        public MemoryStream Convert(string content, EFileConvertionTarget format, bool detailed = false);
        public MemoryStream Convert(TranscriptionOperationResult file, EFileConvertionTarget format, bool detailed = false);
    }
}
