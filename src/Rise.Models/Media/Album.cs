using Rise.Common.Enums;
using Rise.Common.Interfaces;
using SQLite;
using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents an album.
    /// </summary>
    [Table("Albums")]
    public sealed class Album : DbObject, IEquatable<Album>, IMatchable<Album>
    {
        [Column(nameof(Title))]
        public string Title { get; set; }

        [Column(nameof(Artist))]
        public string Artist { get; set; }

        [Column(nameof(Genres))]
        public string Genres { get; set; }

        [Column(nameof(TrackCount))]
        public uint TrackCount { get; set; }

        [Column(nameof(Thumbnail))]
        public string Thumbnail { get; set; }

        [Column(nameof(Year))]
        public uint Year { get; set; }

        /// <summary>
        /// Returns the Album title.
        /// </summary>
        public override string ToString()
        {
            return Title;
        }

        public bool Equals(Album other)
        {
            return Title == other.Title;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }

        public MatchLevel Matches(Album other)
        {
            if (Title.Equals(other.Title))
            {
                return MatchLevel.Full;
            }

            if (Title.Contains(other.Title))
            {
                return MatchLevel.Partial;
            }

            return MatchLevel.None;
        }
    }
}
