using System.Text.Json.Serialization;

namespace Rise.Data.Navigation
{
    /// <summary>
    /// An item that, when clicked, takes the user to a new page.
    /// </summary>
    public sealed class NavigationItemDestination : NavigationItemBase
    {
        public NavigationItemDestination()
            : base(NavigationItemType.Destination) { }

        /// <summary>
        /// Icon to use for this item by default.
        /// </summary>
        public string DefaultIcon { get; init; }

        /// <summary>
        /// The item's text content.
        /// </summary>
        public string Label { get; init; }

        /// <summary>
        /// An access key for this item.
        /// </summary>
        public string AccessKey { get; init; }

        /// <summary>
        /// An identifier for the flyout this item requests.
        /// </summary>
        public string FlyoutId { get; init; }

        /// <summary>
        /// A set of items contained within this item. This property
        /// is not serialized by <see cref="System.Text.Json.JsonSerializer"/>.
        /// </summary>
        [JsonIgnore]
        public object Children { get; set; }
    }
}
