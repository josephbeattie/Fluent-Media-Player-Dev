using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Windows.Globalization;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class LanguagePage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public LanguagePage()
        {
            InitializeComponent();

            string topUserLanguage = GlobalizationPreferences.Languages[0];
            Language sys = new Language(topUserLanguage);
            SysLang.Text = sys.DisplayName;
        }

        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.Translations.LaunchAsync();

        private async void ReportButton_Click(object sender, RoutedEventArgs e)
            => _ = await URLs.TranslationReports.LaunchAsync();
    }
}
