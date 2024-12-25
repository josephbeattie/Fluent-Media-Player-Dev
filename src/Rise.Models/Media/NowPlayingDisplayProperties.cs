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
            Windows.Foundation.Collections.ValueSet customProps = item.Source.CustomProperties;

            uint year = 0;
            if (customProps.TryGetValue("Year", out object yearProp))
            {
                year = (uint)yearProp;
            }

            string location = string.Empty;
            if (customProps.TryGetValue("Location", out object locationProp))
            {
                location = (string)locationProp;
            }

            MediaItemDisplayProperties displayProps = item.GetDisplayProperties();
            if (displayProps.Type == MediaPlaybackType.Music)
            {
                MusicDisplayProperties musicProps = displayProps.MusicProperties;
                return new NowPlayingDisplayProperties(MediaPlaybackType.Music,
                    musicProps.Title,
                    musicProps.Artist,
                    musicProps.AlbumTitle,
                    musicProps.AlbumArtist,
                    location,
                    year,
                    displayProps.Thumbnail);
            }

            VideoDisplayProperties videoProps = displayProps.VideoProperties;
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
