using Rise.App.Dialogs;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class AboutPage : Page
    {
        private readonly DataPackage VersionData = new();

        public AboutPage()
        {
            InitializeComponent();

            vTip.Subtitle = string.Format(ResourceHelper.GetString("VersionTemplate"), AppVersion.VersionName, AppVersion.Version);

            VersionData.RequestedOperation = DataPackageOperation.Copy;
            VersionData.SetText($"{AppVersion.VersionName} - {AppVersion.Version}");
        }

        private async void ExpanderControl_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.License.LaunchAsync();

        private void CommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Insider":
                    _ = Frame.Navigate(typeof(InsiderPage));
                    AllSettingsPage.Current.MainSettingsHeaderIcon.Glyph = "\uF1AD";
                    AllSettingsPage.Current.MainSettingsHeader.Text = ResourceHelper.GetString("InsiderHub");
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
