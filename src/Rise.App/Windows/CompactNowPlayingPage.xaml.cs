using Rise.Common.Threading;
using Rise.Data.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Rise.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompactNowPlayingPage : Page
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;

        public CompactNowPlayingPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();

            _ = VisualStateManager.GoToState(this, nameof(PointerOutState), false);
        }

        private void OnPlayerLoaded(object sender, RoutedEventArgs e)
        {
            MainPlayer.SetMediaPlayer(MPViewModel.Player);
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            MainPlayer.SetMediaPlayer(null);
        }

        public static async Task NavigateAsync(Frame frame)
        {
            _ = await ApplicationView.GetForCurrentView().
                TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);

            _ = frame.Navigate(typeof(CompactNowPlayingPage), null, new SuppressNavigationTransitionInfo());
        }
    }

    // Event handlers
    public sealed partial class CompactNowPlayingPage
    {
        private async void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            _ = await ApplicationView.GetForCurrentView().
                TryEnterViewModeAsync(ApplicationViewMode.Default);

            Frame.GoBack();
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _ = VisualStateManager.GoToState(this, nameof(PointerInState), true);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _ = VisualStateManager.GoToState(this, nameof(PointerOutState), true);
        }
    }
}
