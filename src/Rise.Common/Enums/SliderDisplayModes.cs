namespace Rise.Common.Enums
{
    /// <summary>
    /// Defines the different way transport control sliders
    /// can be displayed.
    /// </summary>
    public enum SliderDisplayModes
    {
        /// <summary>
        /// Neither the slider nor its associated labels
        /// are displayed.
        /// </summary>
        Hidden,

        /// <summary>
        /// Only the slider bar is shown, no thumb. Associated
        /// labels are not displayed.
        /// </summary>
        Minimal,

        /// <summary>
        /// Only the slider bar and its thumb is shown.
        /// Associated labels are not displayed.
        /// </summary>
        SliderOnly,

        /// <summary>
        /// The slider and any associated labels are
        /// displayed.
        /// </summary>
        Full,
    }
}
