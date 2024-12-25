using Rise.Common.Constants;
using Rise.Common.Extensions;
using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;

namespace Rise.Common.Helpers
{
    public static class WebHelpers
    {
        /// <summary>
        /// Checks whether internet access is available.
        /// </summary>
        /// <returns>true if access is available, false otherwise.</returns>
        /// <remarks>This method only checks for the available network resources.</remarks>
        public static bool IsInternetAccessAvailable()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return connectionProfile != null
                && (connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess
                || connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.ConstrainedInternetAccess);
        }

        /// <summary>
        /// Creates a playback item which sources from the provided song URI.
        /// </summary>
        public static async Task<MediaPlaybackItem> GetSongFromUriAsync(Uri uri)
        {
            var source = MediaSource.CreateFromUri(uri);
            await source.OpenAsync();

            var media = new MediaPlaybackItem(source);
            var props = media.GetDisplayProperties();

            props.Type = MediaPlaybackType.Music;
            props.MusicProperties.Title = props.MusicProperties.Title.ReplaceIfNullOrWhiteSpace("Online song");
            props.MusicProperties.Artist = props.MusicProperties.Artist.ReplaceIfNullOrWhiteSpace("UnknownArtistResource");

            source.CustomProperties["Title"] = props.MusicProperties.Title;
            source.CustomProperties["Artists"] = props.MusicProperties.Artist;
            source.CustomProperties["Length"] = source.Duration;

            props.Thumbnail ??= RandomAccessStreamReference.CreateFromUri(new(URIs.MusicThumb));

            media.ApplyDisplayProperties(props);
            return media;
        }

        /// <summary>
        /// Creates a playback item which sources from the provided song URI,
        /// and uses the provided metadata for display properties.
        /// </summary>
        public static Task<MediaPlaybackItem> GetSongFromUriAsync(Uri uri, string title = null, string subtitle = null, string thumbnail = null)
        {
            string actualTitle = title.ReplaceIfNullOrWhiteSpace("Online song");
            string actualSubtitle = subtitle.ReplaceIfNullOrWhiteSpace("UnknownArtistResource");

            var media = GetMediaFromUri(uri, actualTitle, actualSubtitle);
            var props = media.GetDisplayProperties();

            props.Type = MediaPlaybackType.Music;
            props.MusicProperties.Title = actualTitle;
            props.MusicProperties.Artist = actualSubtitle;

            Uri thumb = new(thumbnail.ReplaceIfNullOrWhiteSpace(URIs.MusicThumb));
            props.Thumbnail ??= RandomAccessStreamReference.CreateFromUri(thumb);

            media.ApplyDisplayProperties(props);
            return Task.FromResult(media);
        }

        /// <summary>
        /// Creates a playback item which sources from the provided video URI.
        /// </summary>
        public static async Task<MediaPlaybackItem> GetVideoFromUriAsync(Uri uri)
        {
            var source = MediaSource.CreateFromUri(uri);
            await source.OpenAsync();

            var media = new MediaPlaybackItem(source);
            var props = media.GetDisplayProperties();

            props.Type = MediaPlaybackType.Video;
            props.VideoProperties.Title = props.VideoProperties.Title.ReplaceIfNullOrWhiteSpace("Online video");
            props.VideoProperties.Subtitle = props.VideoProperties.Subtitle.ReplaceIfNullOrWhiteSpace("UnknownArtistResource");

            source.CustomProperties["Title"] = props.VideoProperties.Title;
            source.CustomProperties["Artists"] = props.VideoProperties.Subtitle;
            source.CustomProperties["Length"] = source.Duration;

            props.Thumbnail ??= RandomAccessStreamReference.CreateFromUri(new(URIs.VideoThumb));

            media.ApplyDisplayProperties(props);
            return media;
        }

        /// <summary>
        /// Creates a playback item which sources from the provided video URI,
        /// and uses the provided metadata for display properties.
        /// </summary>
        public static Task<MediaPlaybackItem> GetVideoFromUriAsync(Uri uri, string title = null, string subtitle = null, string thumbnail = null)
        {
            string actualTitle = title.ReplaceIfNullOrWhiteSpace("Online video");
            string actualSubtitle = subtitle.ReplaceIfNullOrWhiteSpace("UnknownArtistResource");

            var media = GetMediaFromUri(uri, actualTitle, actualSubtitle);
            var props = media.GetDisplayProperties();

            props.Type = MediaPlaybackType.Video;
            props.VideoProperties.Title = actualTitle;
            props.VideoProperties.Subtitle = actualSubtitle;

            Uri thumb = new(thumbnail.ReplaceIfNullOrWhiteSpace(URIs.VideoThumb));
            props.Thumbnail ??= RandomAccessStreamReference.CreateFromUri(thumb);

            media.ApplyDisplayProperties(props);
            return Task.FromResult(media);
        }

        private static MediaPlaybackItem GetMediaFromUri(Uri uri, string title, string subtitle)
        {
            var source = MediaSource.CreateFromUri(uri);

            source.CustomProperties["Title"] = title;
            source.CustomProperties["Artists"] = subtitle;
            source.CustomProperties["Length"] = source.Duration;
            source.CustomProperties["Year"] = 0u;

            var media = new MediaPlaybackItem(source);
            return media;
        }
    }
}
