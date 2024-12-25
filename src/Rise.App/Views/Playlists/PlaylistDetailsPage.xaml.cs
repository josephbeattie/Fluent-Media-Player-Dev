using CommunityToolkit.Mvvm.Input;
using Rise.App.Converters;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.Collections;
using Rise.Data.Json;
using Rise.Data.ViewModels;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Rise.App.Views
{
    public sealed partial class PlaylistDetailsPage : MediaPageBase
    {
        private MainViewModel MViewModel => App.MViewModel;
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        private MediaCollectionViewModel VideosViewModel;

        public static readonly DependencyProperty SelectedVideoProperty =
            DependencyProperty.Register("SelectedVideo", typeof(VideoViewModel),
                typeof(PlaylistDetailsPage), new PropertyMetadata(null));

        public SongViewModel SelectedItem
        {
            get => (SongViewModel)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public VideoViewModel SelectedVideo
        {
            get => (VideoViewModel)GetValue(SelectedVideoProperty);
            set => SetValue(SelectedVideoProperty, value);
        }

        private PlaylistViewModel SelectedPlaylist;

        private CompositionPropertySet _propSet;
        private SpriteVisual _backgroundVisual;

        public PlaylistDetailsPage()
            : base(App.MViewModel.Playlists)
        {
            InitializeComponent();

            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;

            PlaylistHelper.AddPlaylistsToSubItem(AddTo, AddSelectedItemToPlaylistCommand);
            PlaylistHelper.AddPlaylistsToSubItem(AddToVideo, AddVideoToPlaylistCommand);
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter is Guid id)
            {
                SelectedPlaylist = MViewModel.Playlists.
                    FirstOrDefault(p => p.Id == id);

                CreateViewModel(string.Empty, SortDirection.Ascending, false, SelectedPlaylist.Songs);
                VideosViewModel = new(string.Empty, SortDirection.Ascending, false, null, SelectedPlaylist.Videos, null, MPViewModel);
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            VideosViewModel.Dispose();
        }

        private void OnMainListLoaded(object sender, RoutedEventArgs e)
        {
            var surface = LoadedImageSurface.StartLoadFromUri(new(SelectedPlaylist.Icon));
            (_propSet, _backgroundVisual) = MainList.CreateParallaxGradientVisual(surface, BackgroundHost);
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (MediaViewModel.Items.Any() && VideosViewModel.Items.Any())
                PlaylistDuration.Text = await Task.Run(() => TimeSpanToString.GetShortFormat(TimeSpan.FromSeconds(MediaViewModel.Items.Cast<SongViewModel>().Select(s => s.Length).Aggregate((t, t1) => t + t1).TotalSeconds) + TimeSpan.FromSeconds(VideosViewModel.Items.Cast<VideoViewModel>().Select(v => v.Length).Aggregate((t, t1) => t + t1).TotalSeconds)));
            else if (!MediaViewModel.Items.Any() && VideosViewModel.Items.Any())
                PlaylistDuration.Text = await Task.Run(() => TimeSpanToString.GetShortFormat(TimeSpan.FromSeconds(VideosViewModel.Items.Cast<VideoViewModel>().Select(v => v.Length).Aggregate((t, t1) => t + t1).TotalSeconds)));
            else if (MediaViewModel.Items.Any() && !VideosViewModel.Items.Any())
                PlaylistDuration.Text = await Task.Run(() => TimeSpanToString.GetShortFormat(TimeSpan.FromSeconds(MediaViewModel.Items.Cast<SongViewModel>().Select(s => s.Length).Aggregate((t, t1) => t + t1).TotalSeconds)));
            else
                PlaylistDuration.Text = TimeSpan.Zero.ToString();
        }
    }

    // Playlists
    public sealed partial class PlaylistDetailsPage
    {
        [RelayCommand]
        private Task AddVideoToPlaylistAsync(PlaylistViewModel playlist)
        {
            if (playlist == null)
                return PlaylistHelper.CreateNewPlaylistAsync(SelectedVideo);

            playlist.AddItem(SelectedVideo);
            return PBackend.SaveAsync();
        }
    }

    // Event handlers
    public sealed partial class PlaylistDetailsPage
    {
        private void MainList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is SongViewModel song)
                MediaViewModel.PlayFromItemCommand.Execute(song);
        }

        private void SongFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainList.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedItem = (SongViewModel)cont;
        }

        private void VideoFlyout_Opening(object sender, object e)
        {
            var fl = sender as MenuFlyout;
            var cont = MainGrid.ItemFromContainer(fl.Target);

            if (cont == null)
                fl.Hide();
            else
                SelectedVideo = (VideoViewModel)cont;
        }

        [RelayCommand]
        private Task RemoveItemAsync(IMediaItem item)
        {
            SelectedPlaylist.RemoveItem(item);
            return PBackend.SaveAsync();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is VideoViewModel video && !KeyboardHelpers.IsCtrlPressed())
            {
                await MPViewModel.PlaySingleItemAsync(video);
                if (Window.Current.Content is Frame rootFrame)
                    rootFrame.Navigate(typeof(NowPlayingPage));
            }
        }

        private async void PlayVideo_Click(object sender, RoutedEventArgs e)
        {
            await MPViewModel.PlaySingleItemAsync(SelectedVideo);
            if (Window.Current.Content is Frame rootFrame)
                _ = rootFrame.Navigate(typeof(NowPlayingPage));
        }

        private void RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;
            SelectedPlaylist.Songs.Remove(song);
        }

        private void MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((SelectedPlaylist.Songs.IndexOf(song) + 1) < SelectedPlaylist.Songs.Count)
            {
                var index = SelectedPlaylist.Songs.IndexOf(song);

                SelectedPlaylist.Songs.Remove(song);
                SelectedPlaylist.Songs.Insert(index + 1, song);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            SongViewModel song = (sender as Button).Tag as SongViewModel;

            if ((SelectedPlaylist.Songs.IndexOf(song) - 1) >= 0)
            {
                var index = SelectedPlaylist.Songs.IndexOf(song);

                SelectedPlaylist.Songs.Remove(song);
                SelectedPlaylist.Songs.Insert(index - 1, song);
            }
        }

        private void BackgroundHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual == null) return;
            _backgroundVisual.Size = new Vector2((float)e.NewSize.Width, (float)BackgroundHost.Height);
        }
    }
}
