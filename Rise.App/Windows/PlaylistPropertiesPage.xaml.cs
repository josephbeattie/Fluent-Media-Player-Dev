using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Data.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class PlaylistPropertiesPage : Page
    {
        private JsonBackendController<PlaylistViewModel> Controller
            => App.MViewModel.PBackend;

        private PlaylistViewModel Playlist;
        private readonly ApplicationView View;

        public PlaylistPropertiesPage()
        {
            InitializeComponent();
            View = ApplicationView.GetForCurrentView();

            TitleBar.SetTitleBarForCurrentView();
            Controller.Items.CollectionChanged += OnPlaylistCollectionChanged;
            View.Consolidated += OnViewConsolidated;
        }

        public static Task<bool> TryShowAsync(PlaylistViewModel playlist)
            => ViewHelpers.OpenViewAsync<PlaylistPropertiesPage>(playlist, new(380, 500));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Playlist = e.Parameter as PlaylistViewModel;
        }

        private async void OnPlaylistCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Controller.Items.CollectionChanged -= OnPlaylistCollectionChanged;
                await View.TryConsolidateAsync();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                bool hasRemoved = e.OldItems.Contains(Playlist);
                if (hasRemoved)
                {
                    Controller.Items.CollectionChanged -= OnPlaylistCollectionChanged;
                    await View.TryConsolidateAsync();
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
            => await FinishEditsAsync(true);

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
            => await FinishEditsAsync(false);

        private async Task FinishEditsAsync(bool save)
        {
            Controller.Items.CollectionChanged -= OnPlaylistCollectionChanged;
            if (!save)
            {
                var items = await Controller.GetStoredItemsAsync();
                var item = items.FirstOrDefault(i => i.Id == Playlist.Id);

                Controller.Items.Remove(Playlist);
                Controller.Items.Add(item);
            }

            await View.TryConsolidateAsync();
            await Controller.SaveAsync();
        }

        private async void OnViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            View.Consolidated -= OnViewConsolidated;
            if (args.IsUserInitiated)
                await Controller.SaveAsync();
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
                        _ = PropsFrame.Navigate(typeof(PlaylistDetailsPropertiesPage), Playlist);
                        break;

                    case "SongsItem":
                        _ = PropsFrame.Navigate(typeof(PlaylistSongsPropertiesPage), Playlist);
                        break;

                    case "VideosItem":
                        _ = PropsFrame.Navigate(typeof(PlaylistVideosPropertiesPage), Playlist);
                        break;

                    default:
                        break;
                }

            }
        }
    }
}
