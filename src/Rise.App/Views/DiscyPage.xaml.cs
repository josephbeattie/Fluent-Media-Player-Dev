using Rise.App.Settings;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class DiscyPage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public DiscyPage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
        }

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _navigationHelper.OnNavigatedFrom(e);
        #endregion

        private void Discy_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            DiscyAboutTip.IsOpen = true;
        }

        private async void LearnMoreButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Readme.LaunchAsync();

        private void AppSettingsHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                _ = rootFrame.Navigate(typeof(AllSettingsPage));
            }
        }
    }
}
