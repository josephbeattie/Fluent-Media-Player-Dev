using Rise.App.Setup;
using Rise.App.ViewModels;
using Rise.App.Views;
using Rise.Common.Extensions.Markup;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Rise.App.Dialogs
{
    public sealed partial class SetupDialogContent : Page
    {
        private SettingsViewModel ViewModel => App.SViewModel;

        public SetupDialogContent()
        {
            InitializeComponent();
            Navigate(SlideNavigationTransitionEffect.FromRight);
        }
    }

    // Event handlers
    public sealed partial class SetupDialogContent
    {
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetupProgress--;
            Navigate(SlideNavigationTransitionEffect.FromLeft);
        }

        private async void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            int progress = ViewModel.SetupProgress;
            if (progress == 1)
            {
                ViewModel.FetchOnlineData = true;
            }
            else if (progress == 5)
            {
                await FinishSetupAsync(false);
                return;
            }

            ViewModel.SetupProgress++;
            Navigate(SlideNavigationTransitionEffect.FromRight);
        }

        private async void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            int progress = ViewModel.SetupProgress;
            if (progress == 0)
            {
                HideDialog();
            }
            if (progress == 1)
            {
                ViewModel.FetchOnlineData = false;
            }
            else if (progress == 5)
            {
                await FinishSetupAsync(true);
                return;
            }

            ViewModel.SetupProgress++;
            Navigate(SlideNavigationTransitionEffect.FromRight);
        }
    }

    // Navigation
    public sealed partial class SetupDialogContent
    {
        /// <summary>
        /// Navigate between pages of the setup using the
        /// specified effect for the page transition.
        /// </summary>
        private void Navigate(SlideNavigationTransitionEffect effect)
        {
            int progress = ViewModel.SetupProgress;
            if (progress == 0)
            {
                SetupInfo.Text = ResourceHelper.GetString("/Setup/SetupPre");
                PrimaryButton.Content = ResourceHelper.GetString("Accept");
            }
            else
            {
                string format = ResourceHelper.GetString("StepOf");
                SetupInfo.Text = string.Format(format, progress, 5);

                PrimaryButton.Content = ResourceHelper.GetString("Continue");
                BackButton.Visibility = progress > 1 ?
                    Visibility.Visible : Visibility.Collapsed;
            }

            string res = GetSecondaryButtonResource(progress);
            SecondaryButton.Content = ResourceHelper.GetString(res);

            var nextPage = GetCurrentPage(progress);
            var transition = new SlideNavigationTransitionInfo() { Effect = effect };
            _ = SetupFrame.Navigate(nextPage, null, transition);
        }

        private async Task FinishSetupAsync(bool closeApp)
        {
            ViewModel.SetupCompleted = true;
            ViewModel.SetupProgress = 0;

            if (closeApp)
            {
                _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
            }
            else
            {
                HideDialog();

                var rootFrame = Window.Current.Content as Frame;
                _ = rootFrame.Navigate(typeof(MainPage));
            }
        }

        private void HideDialog()
        {
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups)
            {
                if (popup.Child is ContentDialog dialog)
                {
                    dialog.Hide();
                    break;
                }
            }
        }

        private Type GetCurrentPage(int progress) => progress switch
        {
            1 => typeof(ConnectPage),
            2 => typeof(LocalPage),
            3 => typeof(PrivacyPage),
            4 => typeof(AppearancePage),
            5 => typeof(FinishPage),
            _ => typeof(TermsPage)
        };

        private string GetSecondaryButtonResource(int progress) => progress switch
        {
            1 => "OnlyLocal",
            2 => "OnlyStreaming",
            3 => "DecideForMe",
            4 => "DecideForMe",
            5 => "NotNow",
            _ => "Decline"
        };
    }
}
