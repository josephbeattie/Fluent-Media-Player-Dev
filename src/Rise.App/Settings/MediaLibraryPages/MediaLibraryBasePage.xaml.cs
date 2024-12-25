using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaLibraryBasePage : Page
    {
        public MediaLibraryBasePage()
        {
            this.InitializeComponent();

            _ = MediaFrame.Navigate(typeof(MediaLibraryPage));
        }

        private void MediaNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var selectedItem = args.InvokedItemContainer;
            string selectedItemTag = selectedItem.Tag as string;

            Type page = selectedItemTag switch
            {
                "Local" => typeof(MediaLibraryPage),
                "Services" => typeof(OnlineServicesPage),
                "Scanning" => typeof(ScanningPage),
                _ => typeof(ComingSoonPage),
            };

            if (MediaFrame.CurrentSourcePageType != page)
                _ = MediaFrame.Navigate(page, null, args.RecommendedNavigationTransitionInfo);
        }
    }
}
