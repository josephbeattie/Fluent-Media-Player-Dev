namespace Rise.Common.Enums
{
    /// <summary>
    /// Defines the available window glaze settings.
    /// </summary>
    public enum GlazeTypes
    {
        /// <summary>
        /// No glaze is used.
        /// </summary>
        None,

        /// <summary>
        /// The system accent is used for glaze.
        /// </summary>
        AccentColor,

        /// <summary>
        /// A custom user-defined color is used for glaze.
        /// </summary>
        CustomColor,

        /// <summary>
        /// The current media thumbnail's dominant color is
        /// used for glaze.
        /// </summary>
        MediaThumbnail
    }
}
