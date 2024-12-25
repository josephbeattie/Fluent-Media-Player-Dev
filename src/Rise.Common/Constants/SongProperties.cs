using System.Collections.Generic;

namespace Rise.Common.Constants
{
    public class SongProperties
    {
        /// <summary>
        /// List of properties that may contain disc numbers.
        /// </summary>
        public static readonly List<string> DiscProperties = new()
        {
            SystemMusic.DiscNumber,
            SystemMusic.PartOfSet,
        };

        public static readonly List<string> ViewModelProperties = new()
        {
            "System.Title",
            SystemMusic.Artist,
            SystemMusic.TrackNumber,
            SystemMusic.DiscNumber,
            SystemMusic.AlbumTitle,
            SystemMusic.AlbumArtist,
            SystemMusic.Genre,
            "System.Media.Year",
            "System.Rating"
        };
    }
}
