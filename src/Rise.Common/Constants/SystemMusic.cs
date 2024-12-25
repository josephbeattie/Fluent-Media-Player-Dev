namespace Rise.Common.Constants
{
    /// <summary>
    /// The list of properties found in the System.Music namespace.
    /// </summary>
    public class SystemMusic
    {
        public const string AlbumArtist =
            "System.Music.AlbumArtist";

        /// <summary>
        /// This optional string value allows for overriding the standard
        /// sort order of <see cref="AlbumArtist"/>.
        /// </summary>
        /// <remarks>This is very important for proper sorting of music files
        /// in Japanese which cannot be correctly sorted phonetically (the
        /// user-expected ordering) without this field. It can also be used for
        /// customizing sorting in non-East Asian scenarios,such as allowing
        /// the user to remove articles for sorting purposes.</remarks>
        public const string AlbumArtistSortOverride =
            "System.Music.AlbumArtistSortOverride";

        /// <summary>
        /// This property differentiates albums with the same title from
        /// different artists. It is the concatenation of <see cref="AlbumArtist"/>
        /// and <see cref="AlbumTitle"/>.
        /// </summary>
        public const string AlbumID =
            "System.Music.AlbumID";

        public const string AlbumTitle =
            "System.Music.AlbumTitle";

        /// <summary>
        /// This optional string value allows for overriding the standard sort order
        /// of <see cref="AlbumTitle"/>.
        /// </summary>
        /// <remarks>This is very important for proper sorting of music files in
        /// Japanese which cannot becorrectly sorted phonetically (the user-expected
        /// ordering) without this field. It can also be used for customizing sorting
        /// in non-East Asian scenarios, such as allowing the user to remove articles
        /// for sorting purposes.</remarks>
        public const string AlbumTitleSortOverride =
            "System.Music.AlbumTitleSortOverride";

        public const string Artist =
            "System.Music.Artist";

        /// <summary>
        /// This optional string value allows for overriding the standard sort order of
        /// <see cref="Artist"/>.
        /// </summary>
        /// <remarks>This is very important for proper sorting of music files in Japanese
        /// which cannot becorrectly sorted phonetically (the user-expected ordering) without
        /// this field. It can also be used for customizing sorting in non-East Asian scenarios,
        /// such as allowing the user to remove articles for sorting purposes.</remarks>
        public const string ArtistSortOverride =
            "System.Music.ArtistSortOverride";

        public const string BeatsPerMinute =
            "System.Music.BeatsPerMinute";

        public const string Composer =
            "System.Music.Composer";

        /// <summary>
        /// This optional string value allows for overriding the standard sort order of
        /// <see cref="Composer"/>.
        /// </summary>
        /// <remarks>This is very important for proper sorting of music files in Japanese
        /// which cannot becorrectly sorted phonetically (the user-expected ordering) without
        /// this field. It can also be used for customizing sorting in non-East Asian scenarios,
        /// such as allowing the user to remove articles for sorting purposes.</remarks>
        public const string ComposerSortOverride =
            "System.Music.ComposerSortOverride";

        public const string Conductor =
            "System.Music.Conductor";

        public const string ContentGroupDescription =
            "System.Music.ContentGroupDescription";

        public const string DiscNumber =
            "System.Music.DiscNumber";

        /// <summary>
        /// This property returns the best representation of the album artist for a specific
        /// music file based upon <see cref="AlbumArtist"/>, <see cref="Artist"/>, and
        /// <see cref="IsCompilation"/> information.
        /// </summary>
        public const string DisplayArtist =
            "System.Music.DisplayArtist";

        public const string Genre =
            "System.Music.Genre";

        public const string InitialKey =
            "System.Music.InitialKey";

        /// <summary>
        /// Indicates whether the music file is part of a compilation.
        /// </summary>
        public const string IsCompilation =
            "System.Music.IsCompilation";

        public const string Lyrics =
            "System.Music.Lyrics";

        public const string Mood =
            "System.Music.Mood";

        public const string PartOfSet =
            "System.Music.PartOfSet";

        public const string Period =
            "System.Music.Period";

        public const string SynchronizedLyrics =
            "System.Music.SynchronizedLyrics";

        public const string TrackNumber =
            "System.Music.TrackNumber";
    }
}
