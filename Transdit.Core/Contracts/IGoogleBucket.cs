using Google.Apis.Storage.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Transcription.Google;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Transdit.Core.Contracts
{
    public interface IGoogleBucket
    {
        Task Delete(Object deletionObject, CancellationToken cancellationToken = default);
        Task Delete(string objectName, CancellationToken cancellationToken = default);
        Task<Stream> Download(string fileName, CancellationToken cancellationToken = default);
        Task<Object> Download(string fileName, string savingPath, CancellationToken cancellationToken = default);
        Task<Object> Get(string fileName, CancellationToken cancellationToken = default);
        string GetUri(Object file);
        IEnumerable<Objects> List(string filter = "");
        Task<Object> Upload(StorageUploadItem item, CancellationToken cancellationToken = default);
        Task<Object> Upload(string path, CancellationToken cancellationToken = default);
    }
}
