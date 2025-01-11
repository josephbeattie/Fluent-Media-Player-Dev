namespace Rise.Data.Navigation
{
    /// <summary>
    /// Identifies an icon pack for items in a NavigationView.
    /// </summary>
    public sealed class IconPack
    {
        public IconPack(string id, string name)
        {
            Id = id;
            DisplayName = name;
        }

        /// <summary>
        /// Gets a unique identifier for this icon pack.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Gets the name that should be displayed for
        /// the icon pack.
        /// </summary>
        public string DisplayName { get; init; }
    }
}
