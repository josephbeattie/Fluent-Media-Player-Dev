using Rise.App.Dialogs;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Collections;
using Rise.Data.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Views
{
    public sealed partial class LocalVideosPage : MediaPageBase
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        public VideoViewModel SelectedItem
        {
            get => (VideoViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public LocalVideosPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var (del, direction, alphabetical) = GetSavedSortPreferences("Videos");
            if (!string.IsNullOrEmpty(del))
                CreateViewModel(del, direction, alphabetical, App.MViewModel.Videos);
            else
                CreateViewModel("VideoTitle", SortDirection.Ascending, false, App.MViewModel.Videos);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            SaveSortingPreferences("Videos");
        }
    }

    // Event handlers
    public sealed partial class LocalVideosPage
    {
        private async void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is VideoViewModel video && !KeyboardHelpers.IsCtrlPressed())
            {
                await MPViewModel.PlaySingleItemAsync(video);
                if (Window.Current.Content is Frame rootFrame)
                    rootFrame.Navigate(typeof(NowPlayingPage));
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (VideoViewModel)cont;
        }

        private void AskDiscy_Click(object sender, RoutedEventArgs e)
        {
            DiscyOnVideo.IsOpen = true;
        }

        private async void PlayFromUrl_Click(object sender, RoutedEventArgs e)
        {
            _ = await new VideoStreamingDialog().ShowAsync();
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
