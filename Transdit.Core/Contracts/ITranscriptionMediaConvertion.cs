using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Transcription;

namespace Transdit.Core.Contracts
{
    public interface ITranscriptionMediaConvertion
    {
        public string TempFolderPath { get; }
        public string TempConvertedFolderPath { get; }
        Task<bool> Convert(InputTranscription transcription);
        Task<(bool, string)> Convert(string filePath);
        void GetMetadata(InputTranscription transcription, string path);
    }

}
