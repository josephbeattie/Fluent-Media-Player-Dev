namespace Rise.Data.Navigation
{
    /// <summary>
    /// Defines the types of item that are allowed inside a NavigationView.
    /// </summary>
    public enum NavigationItemType
    {
        /// <summary>
        /// A header - shows a string of text related to a group of items.
        /// </summary>
        Header = 0,

        /// <summary>
        /// A destination - when clicked, takes the user to a new page.
        /// </summary>
        Destination = 1,

        /// <summary>
        /// A separator - divides items from one another.
        /// </summary>
        Separator = 2
    }
}