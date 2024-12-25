using CommunityToolkit.Mvvm.Input;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using Rise.Data.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class ArtistsPage : MediaPageBase
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;
        private MainViewModel MViewModel => App.MViewModel;

        public ArtistViewModel SelectedItem
        {
            get => (ArtistViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ArtistsPage()
            : base("GArtistName|ArtistName", SortDirection.Ascending, true, App.MViewModel.Artists, App.MViewModel.Playlists)
        {
            InitializeComponent();

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddToPlaylistCommand);
        }
    }

    // Playlists
    public sealed partial class ArtistsPage
    {
        [RelayCommand]
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            var name = SelectedItem.Name;
            var items = new List<SongViewModel>();

            foreach (var itm in MViewModel.Songs)
                if (itm.Artist == name)
                    items.Add(itm);

            if (playlist == null)
            {
                return PlaylistHelper.CreateNewPlaylistAsync(items);
            }
            else
            {
                playlist.AddItems(items);
                return PBackend.SaveAsync();
            }
        }
    }

    // Event handlers
    public sealed partial class ArtistsPage
    {
        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ArtistViewModel artist && !KeyboardHelpers.IsCtrlPressed())
            {
                _ = Frame.Navigate(typeof(ArtistSongsPage), artist.Model.Id);
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (ArtistViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnArtist.IsOpen = true;
        }

        private async void ChngArtImg_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // If this throws, there's no image to work with
                try
                {
                    var img = await file.GetBitmapAsync();
                    if (img == null)
                        return;

                    img.Dispose();
                }
                catch { return; }

                var artist = SelectedItem;
                artist.Picture = URIs.ArtistThumb;

                string filename = $@"artist-{artist.Model.Id}{file.FileType}";
                _ = await file.CopyAsync(ApplicationData.Current.LocalFolder,
                    filename, NameCollisionOption.ReplaceExisting);

                artist.Picture = $@"ms-appdata:///local/{filename}";
                await artist.SaveAsync();
            }
        }

        private async void AddFolders_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = ResourceHelper.GetString("/Settings/MediaLibraryManageFoldersTitle"),
                CloseButtonText = ResourceHelper.GetString("Close"),
                Content = new Settings.MediaSourcesPage()
            };
            _ = await dialog.ShowAsync();
        }
    }
}
