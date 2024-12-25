using Rise.Common.Enums;

namespace Rise.Common.Interfaces
{
    /// <summary>
    /// Defines a generalized type-specific comparison method
    /// that a value type or class implements to check how well
    /// it matches a specified query.
    /// </summary>
    public interface IMatchable<T>
    {
        /// <summary>
        /// Checks if this class matches the specified other
        /// value.
        /// </summary>
        /// <param name="other">An object to compare with this
        /// object.</param>
        /// <returns>How close the match is.</returns>
        MatchLevel Matches(T other);
    }
}
