using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Logging;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.AccessControl;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Transcription.Google;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Transdit.Services.Common
{

    public class GoogleBucketClient : IGoogleBucket
    {
        private readonly ILogger<GoogleBucketClient> _logger;
        private readonly StorageClient _client;
        private readonly Bucket _transditBucket;

        private readonly IProgress<IUploadProgress> _uploadProgressTracker;
        private readonly IProgress<IDownloadProgress> _downloadProgressTracker;
        public GoogleBucketClient(GoogleSettings settings, ILogger<GoogleBucketClient> logger)
        {
            var builder = new StorageClientBuilder() { JsonCredentials = settings.Key };
            _client = builder.Build();
            _transditBucket = _client.GetBucket(settings.BucketName);
            _logger = logger;

            _uploadProgressTracker = new Progress<IUploadProgress>(p => _logger.LogInformation($"bytes: {p.BytesSent}, status: {p.Status}"));
            _downloadProgressTracker = new Progress<IDownloadProgress>(p => _logger.LogInformation($"bytes: {p.BytesDownloaded}, status: {p.Status}"));
        }

        public async Task<Object> Upload(StorageUploadItem item, CancellationToken cancellationToken = default)
        {
            var result = await _client.UploadObjectAsync(_transditBucket.Name, item.Name, item.ContentType, item.Content, cancellationToken: cancellationToken, progress: _uploadProgressTracker);

            return result;
        }

        public async Task<Object> Upload(string path, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return default;

            FileInfo fileInfo = new FileInfo(path);
            var fileName = WebUtility.HtmlEncode(fileInfo.Name);
            var contentType = fileInfo.Extension;
            Object? result = null;
            using (var content = fileInfo.Open(FileMode.Open)) 
            {
                result = await _client.UploadObjectAsync(_transditBucket.Name, fileName, contentType, content, cancellationToken: cancellationToken, progress: _uploadProgressTracker);
            }
            return result;
        }

        public async Task Delete(string objectName, CancellationToken cancellationToken = default) =>
            await _client.DeleteObjectAsync(_transditBucket.Name, objectName, cancellationToken: cancellationToken);

        public async Task Delete(Object deletionObject, CancellationToken cancellationToken = default)
            => await _client.DeleteObjectAsync(deletionObject, cancellationToken: cancellationToken);

        public async Task<Stream> Download(string fileName, CancellationToken cancellationToken = default)
        {
            MemoryStream ms = new MemoryStream();

            var result = await _client.DownloadObjectAsync(_transditBucket.Name, fileName, ms, cancellationToken: cancellationToken, progress: _downloadProgressTracker);

            return ms;
        }
        public async Task<Object> Download(string fileName, string savingPath, CancellationToken cancellationToken = default)
        {
            Object result;
            using (var stream = File.OpenWrite(savingPath))
            {
                result = await _client.DownloadObjectAsync(_transditBucket.Name, fileName, stream, cancellationToken: cancellationToken, progress: _downloadProgressTracker);
            }
            return result;
        }
        public async Task<Object> Get(string fileName, CancellationToken cancellationToken = default)
        {
            var result = await _client.GetObjectAsync(_transditBucket.Name, fileName, cancellationToken: cancellationToken);
            return result;
        }
        public string GetUri(Object file)
        {
            var gsUri = $"gs://{_transditBucket.Name}/{file.Name}";
            return gsUri;
        }

        public IEnumerable<Objects> List(string filter = "")
        {
            var result = _client.ListObjects(_transditBucket.Name, filter);

            return result.AsRawResponses();
        }
    }
}
