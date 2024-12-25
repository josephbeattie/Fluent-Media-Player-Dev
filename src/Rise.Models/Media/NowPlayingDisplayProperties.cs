using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace Rise.Models
{
    /// <summary>
    /// Defines a set of common properties that may be
    /// displayed during media playback.
    /// </summary>
    public record NowPlayingDisplayProperties(MediaPlaybackType ItemType,
        string Title,
        string Artist,
        string Album,
        string AlbumArtist,
        string Location,
        uint Year,
        IRandomAccessStreamReference Thumbnail)
    {
        public static NowPlayingDisplayProperties GetFromPlaybackItem(MediaPlaybackItem item)
        {
            var customProps = item.Source.CustomProperties;

            uint year = 0;
            if (customProps.TryGetValue("Year", out var yearProp))
                year = (uint)yearProp;

            string location = string.Empty;
            if (customProps.TryGetValue("Location", out var locationProp))
                location = (string)locationProp;

            var displayProps = item.GetDisplayProperties();
            if (displayProps.Type == MediaPlaybackType.Music)
            {
                var musicProps = displayProps.MusicProperties;
                return new NowPlayingDisplayProperties(MediaPlaybackType.Music,
                    musicProps.Title,
                    musicProps.Artist,
                    musicProps.AlbumTitle,
                    musicProps.AlbumArtist,
                    location,
                    year,
                    displayProps.Thumbnail);
            }

            var videoProps = displayProps.VideoProperties;
            return new NowPlayingDisplayProperties(MediaPlaybackType.Video,
                videoProps.Title,
                videoProps.Subtitle,
                string.Empty,
                string.Empty,
                location,
                year,
                displayProps.Thumbnail);
        }
    }
}
