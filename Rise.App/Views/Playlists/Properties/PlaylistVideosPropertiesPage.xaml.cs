using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class PlaylistVideosPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;

        public PlaylistVideosPropertiesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Playlist = e.Parameter as PlaylistViewModel;
        }

        private void RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            VideoViewModel video = (sender as Button).Tag as VideoViewModel;
            Playlist.Videos.Remove(video);
        }

        private void MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            VideoViewModel video = (sender as Button).Tag as VideoViewModel;

            if ((Playlist.Videos.IndexOf(video) + 1) < Playlist.Songs.Count)
            {
                var index = Playlist.Videos.IndexOf(video);

                Playlist.Videos.Remove(video);
                Playlist.Videos.Insert(index + 1, video);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            VideoViewModel video = (sender as Button).Tag as VideoViewModel;

            if ((Playlist.Videos.IndexOf(video) - 1) >= 0)
            {
                var index = Playlist.Videos.IndexOf(video);

                Playlist.Videos.Remove(video);
                Playlist.Videos.Insert(index - 1, video);
            }
        }
    }
}
