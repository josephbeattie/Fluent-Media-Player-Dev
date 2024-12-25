using SQLite;
using System;

namespace Rise.Models
{
    /// <summary>
    /// Base class for database entities.
    /// </summary>
    public class DbObject
    {
        /// <summary>
        /// Gets a unique identifier for the item.
        /// </summary>
        [PrimaryKey]
        [Column(nameof(Id))]
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
