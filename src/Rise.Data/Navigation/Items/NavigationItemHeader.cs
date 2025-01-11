namespace Rise.Data.Navigation
{
    /// <summary>
    /// An item that contains a string of text related to a group of items.
    /// </summary>
    public sealed class NavigationItemHeader : NavigationItemBase
    {
        public NavigationItemHeader()
            : base(NavigationItemType.Header) { }

        /// <summary>
        /// The header's text content.
        /// </summary>
        public string Label { get; init; }

        private bool _isGroupVisible = true;
        /// <summary>
        /// Whether any item in this header's group is visible.
        /// </summary>
        public bool IsGroupVisible
        {
            get => _isGroupVisible;
            set => Set(ref _isGroupVisible, value);
        }
    }
}
