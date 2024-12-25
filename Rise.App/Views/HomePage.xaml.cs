using Rise.App.Dialogs;
using Rise.App.Web;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class HomePage : Page
    {
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        private readonly NavigationHelper _navigationHelper;

        public HomePage()
        {
            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
        }

        private async void SupportButton_Click(object sender, RoutedEventArgs e)
            => await URLs.Support.LaunchAsync();

        private async void FoldersButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = ResourceHelper.GetString("/Settings/MediaLibraryManageFoldersTitle"),
                CloseButtonText = ResourceHelper.GetString("Close"),
                Content = new Settings.MediaSourcesPage()
            };
            _ = await dialog.ShowAsync();
        }

        private async void GlanceManage_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                CloseButtonText = ResourceHelper.GetString("Close"),
                DefaultButton = ContentDialogButton.Primary,
                Content = new WidgetsDialogContent()
            };
            _ = await dialog.ShowAsync();
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

        private void BrowseMedia_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BrowsePage));
        }
    }
}