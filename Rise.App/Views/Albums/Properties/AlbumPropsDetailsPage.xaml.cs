using Rise.App.ViewModels;
using Rise.Models;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views.Albums.Properties
{
    public sealed partial class AlbumPropsDetailsPage : Page
    {
        private AlbumViewModel Album;

        public AlbumPropsDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Album = e.Parameter as AlbumViewModel;
        }

        private async void EditArtButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            FileOpenPicker picker = new()
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
                var (saved, path) = await Song.TrySaveThumbnailAsync(file, Album.Model.Id.ToString());
                if (saved)
                    Album.Thumbnail = path;
            }
        }

        private async void ExportAlbumArt_Click(object sender, RoutedEventArgs e)
        {
            var picFile = await StorageFile.GetFileFromApplicationUriAsync(new(Album.Thumbnail));
            FileSavePicker savePicker = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            savePicker.FileTypeChoices.Add("PNG Image", new string[] { ".png" });
            savePicker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
                await picFile.CopyAndReplaceAsync(file);
        }
    }
}
