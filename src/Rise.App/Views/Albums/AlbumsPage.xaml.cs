using CommunityToolkit.Mvvm.Input;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Enums;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using Rise.Data.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class AlbumsPage : MediaPageBase
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;
        private MainViewModel MViewModel => App.MViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private AlbumViewModel SelectedItem
        {
            get => (AlbumViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public AlbumsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToFlyout(AddToBar, AddToPlaylistCommand);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var (del, direction, alphabetical) = GetSavedSortPreferences("Albums");
            if (!string.IsNullOrEmpty(del))
                CreateViewModel(del, direction, alphabetical, App.MViewModel.Albums);
            else
                CreateViewModel("GAlbumTitle|AlbumTitle", SortDirection.Ascending, true, App.MViewModel.Albums);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            SaveSortingPreferences("Albums");
        }
    }

    // Playlists
    public sealed partial class AlbumsPage
    {
        [RelayCommand]
        private Task AddToPlaylistAsync(PlaylistViewModel playlist)
        {
            var name = SelectedItem.Title;
            var items = new List<SongViewModel>();

            foreach (var itm in MViewModel.Songs)
                if (itm.Album == name)
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
    public sealed partial class AlbumsPage
    {
        [RelayCommand]
        private void UpdateViewMode(AlbumViewMode viewMode)
        {
            SViewModel.AlbumViewMode = viewMode;
        }

        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AlbumViewModel album && !KeyboardHelpers.IsCtrlPressed())
            {
                _ = Frame.Navigate(typeof(AlbumSongsPage), album.Model.Id);
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (AlbumViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnAlbum.IsOpen = true;
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
