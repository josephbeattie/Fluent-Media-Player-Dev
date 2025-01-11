namespace Rise.Common.Enums
{
    /// <summary>
    /// Specifies what to do with already open dialogs.
    /// Used in <see cref="ContentDialogHelpers"/>
    /// </summary>
    public enum ExistingDialogOptions
    {
        /// <summary>
        /// Closes any already open dialog and returns
        /// <see cref="ContentDialogResult.None"/>
        /// </summary>
        CloseExisting,

        /// <summary>
        /// Dialog will be enqueued and shown once all
        /// previous dialogs are closed
        /// </summary>
        Enqueue
    }
}
