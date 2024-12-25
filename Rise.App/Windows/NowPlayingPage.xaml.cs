using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using Rise.App.Helpers;
using Rise.App.UserControls;
using Rise.App.ViewModels;
using Rise.Common.Helpers;
using Rise.Common.Threading;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    /// <summary>
    /// A page that shows the current state of playback.
    /// </summary>
    public sealed partial class NowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;
        private bool FullScreenRequested = false;

        private List<SyncedLyricItem> _lyrics;

        // Used to check when transport controls are hiding or showing
        private DependencyPropertyWatcher<double> PlayerControlsTransformWatcher;

        // Used to handle the sidebar
        private DependencyPropertyWatcher<bool> PlayerControlsLyricsWatcher;
        private DependencyPropertyWatcher<bool> PlayerControlsQueueWatcher;

        public NowPlayingPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            _ = ApplyVisualizer(SViewModel.VisualizerType);
        }

        private void OnMainPlayerLoaded(object sender, RoutedEventArgs e)
        {
            MainPlayer.SetMediaPlayer(MPViewModel.Player);

            var controlGrid = MainPlayer.FindDescendant<Grid>((elm) => elm.Name == "ControlPanelGrid");
            if (controlGrid != null)
            {
                var transform = (TranslateTransform)controlGrid.RenderTransform;

                PlayerControlsTransformWatcher = new(transform, TranslateTransform.YProperty);
                PlayerControlsTransformWatcher.PropertyChanged += OnPlayerControlsTransformChanged;
            }

            // Update the sidebar whenever the queue or lyrics buttons are checked
            PlayerControlsLyricsWatcher = new(PlayerControls, RiseMediaTransportControls.IsLyricsButtonCheckedProperty);
            PlayerControlsQueueWatcher = new(PlayerControls, RiseMediaTransportControls.IsQueueButtonCheckedProperty);

            PlayerControlsLyricsWatcher.PropertyChanged += OnPlayerControlsLyricsToggled;
            PlayerControlsQueueWatcher.PropertyChanged += OnPlayerControlsQueueToggled;
        }

        private void OnPlayerControlsLyricsToggled(DependencyPropertyWatcher<bool> sender, bool newValue)
        {
            if (newValue)
            {
                PlayerControls.IsQueueButtonChecked = false;
                _ = VisualStateManager.GoToState(this, "SidebarLyricsState", true);
            }
            else if (!PlayerControls.IsQueueButtonChecked)
            {
                _ = VisualStateManager.GoToState(this, "SidebarHiddenState", true);
            }
        }

        private void OnPlayerControlsQueueToggled(DependencyPropertyWatcher<bool> sender, bool newValue)
        {
            if (newValue)
            {
                PlayerControls.IsLyricsButtonChecked = false;
                _ = VisualStateManager.GoToState(this, "SidebarQueueState", true);
            }
            else if (!PlayerControls.IsLyricsButtonChecked)
            {
                _ = VisualStateManager.GoToState(this, "SidebarHiddenState", true);
            }
        }

        private void OnPlayerControlsTransformChanged(DependencyPropertyWatcher<double> sender, double newValue)
        {
            TitleAreaTranslate.Y = -newValue;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            goToMiniViewCommand = null;
            toggleFullScreenCommand = null;

            MPViewModel.Player.SeekCompleted -= Player_SeekCompleted;
            MPViewModel.Player.PlaybackSession.PositionChanged -= PlaybackSession_PositionChanged;
            MPViewModel.PlayingItemChanged -= MPViewModel_PlayingItemChanged;

            PlayerControlsTransformWatcher?.Dispose();
            PlayerControlsLyricsWatcher?.Dispose();
            PlayerControlsQueueWatcher?.Dispose();

            Bindings.StopTracking();

            MainPlayer.SetMediaPlayer(null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is bool fs && fs)
                FullScreenRequested = fs;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (FullScreenRequested)
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }
    }

    // Event handlers
    public sealed partial class NowPlayingPage
    {
        [RelayCommand]
        private Task GoToMiniViewAsync()
        {
            return CompactNowPlayingPage.NavigateAsync(Frame);
        }

        [RelayCommand]
        private void ToggleFullScreen()
        {
            var view = ApplicationView.GetForCurrentView();

            if (view.IsFullScreenMode)
                view.ExitFullScreenMode();
            else
                _ = view.TryEnterFullScreenMode();
        }

        private async void OnLyricsListLoaded(object sender, RoutedEventArgs e)
        {
            if (!SViewModel.FetchOnlineData)
            {
                _ = VisualStateManager.GoToState(this, "LyricsUnavailableState", true);
                return;
            }

            await UpdateCurrentLyricsAsync();

            MPViewModel.PlayingItemChanged += MPViewModel_PlayingItemChanged;
            MPViewModel.Player.SeekCompleted += Player_SeekCompleted;
            MPViewModel.Player.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
            => PlayerControls.Show();

        private async void MPViewModel_PlayingItemChanged(object sender, MediaPlaybackItem e)
            => await UpdateCurrentLyricsAsync();

        private bool ApplyVisualizer(int index) => index switch
        {
            1 => VisualStateManager.GoToState(this, "BloomVisualizerState", false),
            _ => VisualStateManager.GoToState(this, "NoVisualizerState", false),
        };
    }

    // Lyrics
    public sealed partial class NowPlayingPage
    {
        private async Task UpdateCurrentLyricsAsync()
        {
            await Dispatcher;
            if (MPViewModel.PlayingItemType == MediaPlaybackType.Video)
            {
                _ = VisualStateManager.GoToState(this, "LyricsUnavailableState", true);
                return;
            }

            _ = VisualStateManager.GoToState(this, "LyricsLoadingState", true);

            await ThreadSwitcher.ResumeBackgroundAsync();
            var lyrics = await FetchLyricsForCurrentItemAsync();

            await Dispatcher;
            if (lyrics?.Any() ?? false)
            {
                _lyrics = lyrics.ToList();
                LyricsList.ItemsSource = _lyrics;

                _ = VisualStateManager.GoToState(this, "LyricsAvailableState", true);
            }
            else
            {
                _lyrics = null;
                _ = VisualStateManager.GoToState(this, "LyricsUnavailableState", true);
            }
        }

        private async Task<IEnumerable<SyncedLyricItem>> FetchLyricsForCurrentItemAsync()
        {
            try
            {
                var props = MPViewModel.PlayingItemProperties;
                var lyrics = await MusixmatchHelper.GetSyncedLyricsAsync(props.Title, props.Artist);

                var body = lyrics?.Message?.Body;
                return body?.Subtitle?.Subtitles?.Where(i => !string.IsNullOrWhiteSpace(i.Text));
            }
            catch { }

            return Enumerable.Empty<SyncedLyricItem>();
        }

        private void LyricItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var syncedLyricItem = (SyncedLyricItem)((LyricItem)sender).DataContext;
            MPViewModel.Player.PlaybackSession.Position = syncedLyricItem.TimeSpan + TimeSpan.FromMilliseconds(150);
        }

        private async void Player_SeekCompleted(MediaPlayer sender, object args)
        {
            await Dispatcher;
            UpdateCurrentLyric(sender.PlaybackSession.Position);
        }

        private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
            await Dispatcher;
            UpdateCurrentLyric(sender.Position);
        }

        private void UpdateCurrentLyric(TimeSpan playerPosition)
        {
            var lyricsItem = _lyrics?.LastOrDefault(item => item.TimeSpan.TotalSeconds < playerPosition.TotalSeconds);

            if (lyricsItem != null && lyricsItem != LyricsList.SelectedItem)
            {
                var currentlySelectedLyric = _lyrics.FirstOrDefault(item => item.IsSelected);

                if (currentlySelectedLyric != null)
                {
                    var currentlySelectedLyricIndex = _lyrics.IndexOf(currentlySelectedLyric);
                    _lyrics[currentlySelectedLyricIndex].IsSelected = false;
                }

                int selectedLyricIndex = _lyrics.IndexOf(lyricsItem);
                _lyrics[selectedLyricIndex].IsSelected = true;

                LyricsList.SelectedIndex = selectedLyricIndex;
                LyricsList.ScrollIntoView(lyricsItem);
            }
        }
    }
}
