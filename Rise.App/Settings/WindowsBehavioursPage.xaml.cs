using Microsoft.Toolkit.Uwp.Helpers;
using Rise.App.ViewModels;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class WindowsBehavioursPage : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public WindowsBehavioursPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, GetInfoBarState(), false);
            VisualStateManager.GoToState(this, GetVersionState(SystemInformation.Instance.OperatingSystemVersion.Build), false);
        }

        private async void OpenAtStartup_Toggled(object sender, RoutedEventArgs e)
        {
            await ViewModel.OpenAtStartupAsync();
            VisualStateManager.GoToState(this, GetInfoBarState(), false);
        }

        private async void InfoBarStartupLink_Click(object sender, RoutedEventArgs e)
            => _ = await Launcher.LaunchUriAsync(new Uri(@"ms-settings:startupapps"));

        private async void Update_Click(object sender, RoutedEventArgs e)
            => _ = await Launcher.LaunchUriAsync(new Uri(@"ms-settings:windowsupdate"));
    }

    public partial class WindowsBehavioursPage
    {
        private string GetInfoBarState() => ViewModel.FLGStartupTask switch
        {
            1 => "DisabledByPolicy",
            2 => "DisabledByUser",
            3 => "EnabledByPolicy",
            _ => "NoRestrictions",
        };

        private string GetVersionState(ushort version) => version switch
        {
            >= 22000 => "Windows11State",
            >= 21996 => "LeakedBuildState",
            _ => "Windows10State",
        };
    }
}