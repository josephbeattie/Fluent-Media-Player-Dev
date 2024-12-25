using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions.Markup;
using Rise.Common.Interfaces;
using Rise.Data.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Rise.App.Helpers
{
    /// <summary>
    /// A helper class for the Add to Playlist functionality.
    /// </summary>
    public sealed partial class AddToPlaylistHelper
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

        private bool _addSeparator = false;
        private readonly IList<PlaylistViewModel> _items;

        /// <summary>
        /// Initializes the helper with the provided collection.
        /// </summary>
        public AddToPlaylistHelper(IList<PlaylistViewModel> playlists)
        {
            _items = playlists;
            _addSeparator = _items.Count > 0;
        }

        /// <summary>
        /// Adds the current playlists to the flyout subitem, setting
        /// the children commands to <paramref name="addCommand"/>.
        /// </summary>
        public void AddPlaylistsToSubItem(MenuFlyoutSubItem subItem, ICommand addCommand)
        {
            var itms = subItem.Items;
            if (_addSeparator)
                itms.Add(new MenuFlyoutSeparator());

            foreach (var itm in _items)
                itms.Add(CreatePlaylistFlyoutItem(itm, addCommand));
        }

        /// <summary>
        /// Adds the current playlists to the flyout, setting
        /// the children commands to <paramref name="addCommand"/>.
        /// </summary>
        public void AddPlaylistsToFlyout(MenuFlyout flyout, ICommand addCommand)
        {
            var itms = flyout.Items;
            if (_addSeparator)
                itms.Add(new MenuFlyoutSeparator());

            foreach (var itm in _items)
                itms.Add(CreatePlaylistFlyoutItem(itm, addCommand));
        }

        private MenuFlyoutItem CreatePlaylistFlyoutItem(PlaylistViewModel playlist, ICommand addCommand)
        {
            return new MenuFlyoutItem()
            {
                Text = playlist.Title,
                Icon = new FontIcon
                {
                    Glyph = "\uE93F",
                    FontFamily = (FontFamily)Application.Current.Resources["PlayerIconsFontFamily"]
                },
                Command = addCommand,
                CommandParameter = playlist
            };
        }
    }

    // Commands
    public partial class AddToPlaylistHelper
    {
        /// <summary>
        /// Creates a new playlist, adds the specified item to it,
        /// saves the playlist, and adds the flyout items to the
        /// observed objects.
        /// </summary>
        /// <returns>The new playlist.</returns>
        public Task<PlaylistViewModel> CreateNewPlaylistAsync(IMediaItem item)
        {
            return CreateNewPlaylistAsync(new IMediaItem[1] { item });
        }

        /// <summary>
        /// Creates a new playlist, adds the specified items to it,
        /// saves the playlist, and adds the flyout items to the
        /// observed objects.
        /// </summary>
        /// <returns>The new playlist.</returns>
        public async Task<PlaylistViewModel> CreateNewPlaylistAsync(IEnumerable<IMediaItem> items)
        {
            PlaylistViewModel playlist;
            lock (_items)
            {
                playlist = new()
                {
                    Title = ResourceHelper.GetString("UntitledPlaylist"),
                    Description = string.Empty,
                    Icon = URIs.PlaylistThumb
                };

                _addSeparator = true;
            }

            playlist.AddItems(items);

            PBackend.Items.Add(playlist);
            await PBackend.SaveAsync();

            return playlist;
        }
    }
}
