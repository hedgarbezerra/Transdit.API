using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Transdit.Core.Contracts
{
    public interface IYoutubeDownloader
    {
        Task<string> DownloadAudioAsync(string url, string path, bool highestQuality = false, CancellationToken cancellationToken = default);
        Task<string> DownloadVideoAsync(string url, string path, bool highestQuality = false, CancellationToken cancellationToken = default);
        Task<Video> GetVideoAsync(string url, CancellationToken cancellationToken = default);
        Task<StreamManifest> GetVideoManifest(string url, CancellationToken cancellationToken = default);
        Task<StreamManifest> GetVideoManifest(VideoId videoId, CancellationToken cancellationToken = default);
    }
}
