using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppearanceBasePage : Page
    {
        public AppearanceBasePage()
        {
            this.InitializeComponent();

            _ = AppearanceFrame.Navigate(typeof(AppearancePage));
        }

        private void AppearanceNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var selectedItem = args.InvokedItemContainer;
            string selectedItemTag = selectedItem.Tag as string;

            Type page = selectedItemTag switch
            {
                "Window" => typeof(AppearancePage),
                _ => typeof(ComingSoonPage),
            };

            if (AppearanceFrame.CurrentSourcePageType != page)
                _ = AppearanceFrame.Navigate(page, null, args.RecommendedNavigationTransitionInfo);
        }
    }
}
