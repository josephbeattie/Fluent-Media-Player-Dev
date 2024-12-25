namespace Rise.Data.Navigation
{
    // Default collection of items for this data source
    public sealed partial class NavigationDataSource
    {
        private readonly NavigationItemBase[] _defaultItems = new NavigationItemBase[]
        {
            new NavigationItemDestination()
            {
                Id = "HomePage",
                Group = "GeneralGroup",
                DefaultIcon = "\uE10F",
                Label = "Home",
                AccessKey = "H",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "PlaylistsPage",
                Group = "GeneralGroup",
                DefaultIcon = "\uE8FD",
                Label = "Playlists",
                AccessKey = "P",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemHeader()
            {
                Id = "MusicGroup",
                Group = "MusicGroup",
                Label = "Music"
            },
            new NavigationItemDestination()
            {
                Id = "SongsPage",
                Group = "MusicGroup",
                DefaultIcon = "\uE8D6",
                Label = "Songs",
                AccessKey = "N",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "AlbumsPage",
                Group = "MusicGroup",
                DefaultIcon = "\uE93C",
                Label = "Albums",
                AccessKey = "A",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemDestination()
            {
                Id = "ArtistsPage",
                Group = "MusicGroup",
                DefaultIcon = "\uE125",
                Label = "Artists",
                AccessKey = "T",
                FlyoutId = "DefaultItemFlyout"
            },
            new NavigationItemHeader()
            {
                Id = "VideosGroup",
                Group = "VideosGroup",
                Label = "Videos"
            },
            new NavigationItemDestination()
            {
                Id = "LocalVideosPage",
                Group = "VideosGroup",
                DefaultIcon = "\uE1D3",
                Label = "LocalVideos",
                AccessKey = "L",
                FlyoutId = "RemoveItemFlyout"
            }
        };
    }
}
