using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Transdit.Services.Common
{

    public class YoutubeDownloader : IYoutubeDownloader
    {
        private readonly YoutubeClient _youtubeClient = new YoutubeClient();

        public async Task<Video> GetVideoAsync(string url, CancellationToken cancellationToken = default)
        {
            var video = await _youtubeClient.Videos.GetAsync(url, cancellationToken);

            return video;
        }

        public async Task<StreamManifest> GetVideoManifest(VideoId videoId, CancellationToken cancellationToken = default)
        {
            var videoManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoId, cancellationToken);
            return videoManifest;
        }
        public async Task<StreamManifest> GetVideoManifest(string url, CancellationToken cancellationToken = default)
        {
            var videoManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(url, cancellationToken);
            return videoManifest;
        }
        public async Task<string> DownloadAudioAsync(string url, string path, bool highestQuality = false, CancellationToken cancellationToken = default)
        {
            var video = await GetVideoAsync(url, cancellationToken);
            var videoManifest = await GetVideoManifest(video.Id, cancellationToken);

            var audioStreamInfo = videoManifest.GetAudioOnlyStreams().Where(a => a.AudioCodec.Contains("opus"));
            var highestQualityAudio = highestQuality 
                ? audioStreamInfo.OrderByDescending(a => a.Bitrate).FirstOrDefault() 
                : audioStreamInfo.OrderBy(a => a.Bitrate).FirstOrDefault();
            
            await _youtubeClient.Videos.Streams.DownloadAsync(highestQualityAudio, path, cancellationToken: cancellationToken);

            return video.Title;
        }

        public async Task<string> DownloadVideoAsync(string url, string path, bool highestQuality = false, CancellationToken cancellationToken = default)
        {
            var video = await GetVideoAsync(url, cancellationToken);
            var videoManifest = await GetVideoManifest(video.Id, cancellationToken);

            var streamInfo = videoManifest.GetMuxedStreams();
            var highestQualityMidia = highestQuality 
                ? streamInfo.OrderByDescending(a => a.VideoQuality.MaxHeight).FirstOrDefault() 
                : streamInfo.OrderBy(a => a.VideoQuality.MaxHeight).FirstOrDefault();

            await _youtubeClient.Videos.DownloadAsync(highestQualityMidia?.Url, path, cancellationToken: cancellationToken);

            return video.Title;
        }
    }
}
