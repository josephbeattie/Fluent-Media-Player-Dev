using Rise.App.ViewModels;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Data.Json;
using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class CreatePlaylistDialog : ContentDialog
    {
        private JsonBackendController<PlaylistViewModel> PBackend
            => App.MViewModel.PBackend;

        private readonly PlaylistViewModel NewPlaylist = new()
        {
            Icon = URIs.PlaylistThumb
        };

        public CreatePlaylistDialog()
        {
            InitializeComponent();
        }

        #region Events/Methods

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string title = NewPlaylist.Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                ErrorBlock.Text = ResourceHelper.GetString("TitleNotEmpty");
                ErrorBlock.Visibility = Visibility.Visible;

                args.Cancel = true;
                return;
            }

            if (!PBackend.Items.Any(p => p.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                PBackend.Items.Add(NewPlaylist);
                await PBackend.SaveAsync();
            }
            else
            {
                ErrorBlock.Text = ResourceHelper.GetString("PlaylistAlreadyExists");
                ErrorBlock.Visibility = Visibility.Visible;

                args.Cancel = true;
            }
        }

        private async void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var icon = await ApplicationData.Current.
                LocalFolder.TryGetItemAsync($@"playlist-{NewPlaylist.Id}.png");

            if (icon != null)
                await icon.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        private async void UseCustomImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // If this throws, there's no image to work with
                try
                {
                    var img = await file.GetBitmapAsync();
                    if (img == null)
                        return;

                    img.Dispose();
                }
                catch { return; }

                NewPlaylist.Icon = URIs.PlaylistThumb;

                string filename = $@"artist-{NewPlaylist.Id}{file.FileType}";
                _ = await file.CopyAsync(ApplicationData.Current.LocalFolder,
                    filename, NameCollisionOption.ReplaceExisting);

                NewPlaylist.Icon = $@"ms-appdata:///local/{filename}";
            }
        }

        #endregion
    }
}
