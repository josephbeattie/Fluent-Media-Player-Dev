using Rise.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class PlaylistSongsPropertiesPage : Page
    {
        private PlaylistViewModel Playlist;

        public PlaylistSongsPropertiesPage()
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
            SongViewModel song = (sender as Button).Tag as SongViewModel;
            Playlist.Songs.Remove(song);
        }

        private void MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((Playlist.Songs.IndexOf(song) + 1) < Playlist.Songs.Count)
            {
                var index = Playlist.Songs.IndexOf(song);

                Playlist.Songs.Remove(song);
                Playlist.Songs.Insert(index + 1, song);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((Playlist.Songs.IndexOf(song) - 1) >= 0)
            {
                var index = Playlist.Songs.IndexOf(song);

                Playlist.Songs.Remove(song);
                Playlist.Songs.Insert(index - 1, song);
            }
        }
    }
}
