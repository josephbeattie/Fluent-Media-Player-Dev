namespace Rise.Common.Enums
{
    /// <summary>
    /// A simplified enumeration that represents possible results for
    /// background task registration.
    /// </summary>
    public enum BackgroundTaskRegistrationStatus
    {
        /// <summary>
        /// Indicates that the background task was registered
        /// successfully.
        /// </summary>
        Successful = 0,

        /// <summary>
        /// The background task cannot be registered because
        /// of set policies.
        /// </summary>
        NotAllowed = 1,

        /// <summary>
        /// The background task was not registered successfully.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// The background task is already registered.
        /// </summary>
        AlreadyExists = 3
    }
}