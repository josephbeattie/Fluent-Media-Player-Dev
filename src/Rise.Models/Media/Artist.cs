using Rise.Common.Enums;
using Rise.Common.Interfaces;
using SQLite;
using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents an Artist.
    /// </summary>
    [Table("Artists")]
    public sealed class Artist : DbObject, IEquatable<Artist>, IMatchable<Artist>
    {
        [Column(nameof(Name))]
        public string Name { get; set; }

        [Column(nameof(Picture))]
        public string Picture { get; set; }

        [Column(nameof(SongCount))]
        [NotNull]
        public int SongCount { get; set; }

        [Column(nameof(AlbumCount))]
        [NotNull]
        public int AlbumCount { get; set; }

        /// <summary>
        /// Returns the Album title.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Artist other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public MatchLevel Matches(Artist other)
        {
            if (Name.Equals(other.Name))
            {
                return MatchLevel.Full;
            }

            if (Name.Contains(other.Name))
            {
                return MatchLevel.Partial;
            }

            return MatchLevel.None;
        }
    }
}
