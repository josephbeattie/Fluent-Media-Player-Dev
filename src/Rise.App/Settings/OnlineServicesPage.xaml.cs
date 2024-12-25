using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Data.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Rise.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OnlineServicesPage : Page
    {
        private LastFMViewModel ViewModel => App.LMViewModel;

        public OnlineServicesPage()
        {
            InitializeComponent();
        }

        private async void LastFmFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (!WebHelpers.IsInternetAccessAvailable())
                return;

            try
            {
                bool result = await ViewModel.TryAuthenticateAsync();
                LastFMStatus.IsEnabled = !result;

                if (result)
                    ViewModel.SaveCredentialsToVault(LastFM.VaultResource);
            }
            catch (Exception ex)
            {
                ex.WriteToOutput();
            }
        }
    }
}
