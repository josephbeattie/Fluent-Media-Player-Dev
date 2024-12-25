using Rise.App.Views;
using Rise.App.Web;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllSettingsPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        public static AllSettingsPage Current;

        public AllSettingsPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            Current = this;

            TitleBar.SetTitleBarForCurrentView();

            _ = SettingsMainFrame.Navigate(typeof(AppearanceBasePage));
            FinishNavigation();
        }

        private async void FeedbackSettings_Click(object sender, RoutedEventArgs e)
        {
            _ = await FeedbackPage.TryShowAsync();
        }

        private void Insider_Click(object sender, RoutedEventArgs e)
        {
            _ = SettingsMainFrame.Navigate(typeof(InsiderPage));
            FinishNavigation();
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            _ = SettingsMainFrame.Navigate(typeof(LanguagePage));
            FinishNavigation();
        }

        private void SettingsSidebar_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args?.InvokedItemContainer?.Tag?.ToString();
            if (tag != null)
            {
                Type page = tag switch
                {
                    "Appearance" => typeof(AppearanceBasePage),
                    "MediaLibrary" => typeof(MediaLibraryBasePage),
                    "Navigation" => typeof(NavigationPage),
                    "Playback" => typeof(PlaybackPage),
                    "Behaviour" => typeof(WindowsBehavioursPage),
                    "About" => typeof(AboutPage),
                    _ => typeof(MediaSourcesPage),
                };

                if (SettingsMainFrame.CurrentSourcePageType != page)
                {
                    _ = SettingsMainFrame.Navigate(page, null, args.RecommendedNavigationTransitionInfo);
                    SettingsMainFrame.BackStack.Clear();

                    FinishNavigation();
                }
            }
        }

        private void FinishNavigation()
        {
            string type = SettingsMainFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            switch (tag)
            {
                case "AppearanceBasePage":
                    MainSettingsHeaderIcon.Glyph = "\uE771";
                    MainSettingsHeader.Text = ResourceHelper.GetString("Appearance");
                    break;
                case "MediaLibraryBasePage":
                    MainSettingsHeaderIcon.Glyph = "\uEA69";
                    MainSettingsHeader.Text = ResourceHelper.GetString("MediaLibrary");
                    break;
                case "NavigationPage":
                    MainSettingsHeaderIcon.Glyph = "\uE8B0";
                    MainSettingsHeader.Text = ResourceHelper.GetString("Navigation");
                    break;
                case "PlaybackPage":
                    MainSettingsHeaderIcon.Glyph = "\uF4C3";
                    MainSettingsHeader.Text = ResourceHelper.GetString("PlaybackAndSound");
                    break;
                case "ComingSoonPage":
                    MainSettingsHeaderIcon.Glyph = "\uE115";
                    MainSettingsHeader.Text = ResourceHelper.GetString("ComingSoon");
                    break;
                case "WindowsBehavioursPage":
                    MainSettingsHeaderIcon.Glyph = "\uEC7A";
                    MainSettingsHeader.Text = ResourceHelper.GetString("SystemBehaviors");
                    break;
                case "AboutPage":
                    MainSettingsHeaderIcon.Glyph = "\uE946";
                    MainSettingsHeader.Text = ResourceHelper.GetString("About");
                    break;
                case "InsiderPage":
                    MainSettingsHeaderIcon.Glyph = "\uF1AD";
                    MainSettingsHeader.Text = ResourceHelper.GetString("InsiderHub");
                    break;
                case "LanguagePage":
                    MainSettingsHeaderIcon.Glyph = "\uE12B";
                    MainSettingsHeader.Text = ResourceHelper.GetString("Language");
                    break;
            }
        }

        private void GoBackAPage_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMainFrame.CanGoBack)
            {
                SettingsMainFrame.GoBack();
                FinishNavigation();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                _ = Frame.Navigate(typeof(MainPage));
        }
    }
}