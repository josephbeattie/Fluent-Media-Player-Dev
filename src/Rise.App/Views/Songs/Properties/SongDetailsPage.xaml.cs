using Rise.App.ViewModels;
using Rise.Common.Extensions;
using Rise.Models;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Rise.App.Views
{
    public sealed partial class SongDetailsPage : Page
    {
        private SongPropertiesViewModel Props { get; set; }

        public SongDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is SongPropertiesViewModel props)
            {
                Props = props;
            }

            if (Props.Model.Model.IsLocal)
                LocalExpander.Visibility = Visibility.Visible;
            else
                OnlineExpander.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);
        }

        private void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            LocalTip.IsOpen = true;
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
                var (saved, path) = await Song.TrySaveThumbnailAsync(file, Props.Album.AsValidFileName());
                if (saved)
                    Props.Thumbnail = path;
            }
        }

        private async void ExportAlbumArt_Click(object sender, RoutedEventArgs e)
        {
            var picFile = await StorageFile.GetFileFromApplicationUriAsync(new(Props.Thumbnail));
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

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            OnlineTip.IsOpen = true;
        }
    }
}
