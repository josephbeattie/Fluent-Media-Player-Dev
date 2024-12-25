using Rise.App.ViewModels;
using Rise.Common.Extensions;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongPropertiesPage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public SongPropertiesPage()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        public static Task<bool> TryShowAsync(SongPropertiesViewModel props)
            => ViewHelpers.OpenViewAsync<SongPropertiesPage>(props, new(380, 500));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
            => _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = await Props.SaveChangesAsync();

            if (result)
            {
                _ = await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                return;
            }

            ErrorTip.IsOpen = true;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = selectedItem.Tag as string;
                switch (selectedItemTag)
                {
                    case "DetailsItem":
                        _ = PropsFrame.Navigate(typeof(SongDetailsPage), Props);
                        break;

                    case "LyricsItem":
                        _ = PropsFrame.Navigate(typeof(SongLyricsPage), Props);
                        break;

                    case "FileItem":
                        _ = PropsFrame.Navigate(typeof(SongFilePage), Props);
                        break;

                    default:
                        break;
                }

            }
        }
    }
}
