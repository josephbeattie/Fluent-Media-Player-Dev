namespace Rise.Data.Navigation
{
    /// <summary>
    /// An item that acts as a separator between other items.
    /// </summary>
    public sealed class NavigationItemSeparator : NavigationItemBase
    {
        public NavigationItemSeparator()
            : base(NavigationItemType.Separator) { }
    }
}
