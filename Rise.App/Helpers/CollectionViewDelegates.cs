using Rise.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization.Collation;

namespace Rise.App.Helpers
{
    /// <summary>
    /// A cache for all delegates related to sorting and grouping.
    /// </summary>
    public static partial class CollectionViewDelegates
    {
        private readonly static Dictionary<string, Func<object, object>> _delegates = new()
        {
            { "SongDisc", s => ((SongViewModel)s).Disc },
            { "SongTrack", s => ((SongViewModel)s).Track },

            { "SongTitle", s => ((SongViewModel)s).Title },
            { "SongAlbum", s => ((SongViewModel)s).Disc },
            { "SongArtist", s => ((SongViewModel)s).Artist },
            { "SongGenres", s => ((SongViewModel)s).Genres },
            { "SongYear", s => ((SongViewModel)s).Year },

            { "GSongTitle", GSongTitle },
            { "GSongAlbum", s => ((SongViewModel)s).Album },
            { "GSongArtist", s => ((SongViewModel)s).Artist },
            { "GSongGenres", s => ((SongViewModel)s).Genres },
            { "GSongYear", s => ((SongViewModel)s).Year },

            { "AlbumTitle", a => ((AlbumViewModel)a).Title },
            { "AlbumArtist", a => ((AlbumViewModel)a).Artist },
            { "AlbumGenres", a => ((AlbumViewModel)a).Genres },
            { "AlbumYear", a => ((AlbumViewModel)a).Year },

            { "GAlbumTitle", GAlbumTitle },
            { "GAlbumArtist", a => ((AlbumViewModel)a).Artist },
            { "GAlbumGenres", a => ((AlbumViewModel)a).Genres },
            { "GAlbumYear", a => ((AlbumViewModel)a).Year },

            { "VideoTitle", v => ((VideoViewModel)v).Title },
            { "VideoYear", v => ((VideoViewModel)v).Year },
            { "VideoLength", v => ((VideoViewModel)v).Length },

            { "PlaylistTitle", p => ((PlaylistViewModel)p).Title },

            { "ArtistName", a => ((ArtistViewModel)a).Name },
            { "GArtistName", GArtistName },
        };

        public static Func<object, object> GetDelegate(string key)
            => _delegates[key];

        public static bool TryGetDelegate(string key, out Func<object, object> del)
            => _delegates.TryGetValue(key, out del);
    }

    // Grouping delegates
    public static partial class CollectionViewDelegates
    {
        /// <summary>
        /// Character groupings for the current language.
        /// </summary>
        public readonly static CharacterGroupings CharacterGroupings = new();

        /// <summary>
        /// Gets all labels for the character groupings.
        /// </summary>
        public readonly static IEnumerable<string> GroupingLabels = CharacterGroupings.Select(g => g.Label);

        private static object GSongTitle(object s)
            => ToGroupHeader(((SongViewModel)s).Title);

        private static object GAlbumTitle(object a)
            => ToGroupHeader(((AlbumViewModel)a).Title);

        private static object GArtistName(object a)
            => ToGroupHeader(((ArtistViewModel)a).Name);

        private static string ToGroupHeader(string text)
        {
            string key = CharacterGroupings.Lookup(text);
            if (!GroupingLabels.Contains(key))
                return string.Empty;

            return key;
        }
    }
}
