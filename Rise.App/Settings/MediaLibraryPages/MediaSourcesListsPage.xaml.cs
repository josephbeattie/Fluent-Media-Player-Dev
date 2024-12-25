using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class MediaSourcesListsPage : Page
    {
        private string _currTag = "AllMedia";

        private StorageLibrary MusicLibrary => App.MusicLibrary;
        private StorageLibrary VideoLibrary => App.VideoLibrary;

        public MediaSourcesListsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string param)
                _currTag = param;

            VisualStateManager.GoToState(this, $"{_currTag}State", false);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currTag == "Music")
                _ = await MusicLibrary.RequestAddFolderAsync();
            else if (_currTag == "Videos")
                _ = await VideoLibrary.RequestAddFolderAsync();
            else
                AddFlyout.ShowAt(AddButton);
        }

        private async void AddMusicFolder_Click(object sender, RoutedEventArgs e)
            => _ = await MusicLibrary.RequestAddFolderAsync();

        private async void AddVideoFolder_Click(object sender, RoutedEventArgs e)
            => _ = await VideoLibrary.RequestAddFolderAsync();

        private async void RemoveMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            var elm = (FrameworkElement)sender;
            if (elm.DataContext is StorageFolder folder)
                _ = await MusicLibrary.RequestRemoveFolderAsync(folder);
        }

        private async void RemoveVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            var elm = (FrameworkElement)sender;
            if (elm.DataContext is StorageFolder folder)
                _ = await VideoLibrary.RequestRemoveFolderAsync(folder);
        }

        private async void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var elm = (FrameworkElement)sender;
            if (elm.DataContext is StorageFolder folder)
                _ = await Launcher.LaunchFolderAsync(folder);
        }
    }
}
