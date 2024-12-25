using Rise.Common.Enums;
using Rise.Common.Interfaces;
using SQLite;
using System;

namespace Rise.Models
{
    /// <summary>
    /// Represents a genre.
    /// </summary>
    [Table("Genres")]
    public sealed class Genre : DbObject, IEquatable<Genre>, IMatchable<Genre>
    {
        [Column(nameof(Name))]
        public string Name { get; set; }

        /// <summary>
        /// Returns the genre.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Genre other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public MatchLevel Matches(Genre other)
        {
            return Name.Equals(other.Name) ? MatchLevel.Full : Name.Contains(other.Name) ? MatchLevel.Partial : MatchLevel.None;
        }
    }
}
