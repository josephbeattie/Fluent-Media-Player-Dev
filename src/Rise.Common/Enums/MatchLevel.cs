namespace Rise.Common.Enums
{
    /// <summary>
    /// Represents how closely differents objects match.
    /// </summary>
    public enum MatchLevel
    {
        /// <summary>
        /// The specified object does not match
        /// the other object.
        /// </summary>
        None,

        /// <summary>
        /// The specified object partially matches
        /// the other object.
        /// </summary>
        Partial,

        /// <summary>
        /// The specified object fully matches
        /// the other object.
        /// </summary>
        Full
    }
}
