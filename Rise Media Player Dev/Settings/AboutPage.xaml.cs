﻿using Rise.App.Common;
using Rise.App.Dialogs;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Settings
{
    public sealed partial class AboutPage : Page
    {
        private readonly DataPackage VersionData = new();

        public AboutPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            VersionData.RequestedOperation = DataPackageOperation.Copy;
            VersionData.SetText("Pre-Alpha 4 - v0.0.14.0");
        }

        private async void ExpanderControl_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.License.LaunchAsync();

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    AllSettingsPage.Current.GOBACKPAGE.Visibility = Visibility.Visible;
                    AllSettingsPage.Current.MainSettingsHeader.Text = "Insider Hub";
                    AllSettingsPage.Current.MainSettingsHeaderIcon.Glyph = "\uECA7";
                    AllSettingsPage.Current.SettingsMainFrame.Navigate(typeof(InsiderPage));
                    break;

                case "Version":
                    vTip.IsOpen = true;
                    break;

            }
        }

        private void VTip_CloseButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
            => Clipboard.SetContent(VersionData);

        private async void VTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
            => await URLs.Releases.LaunchAsync();
    }
}
