using Rise.Common.Extensions.Markup;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Settings
{
    public sealed partial class InsiderWallpapers : Page
    {
        private readonly ObservableCollection<Wallpaper> Walls = new();

        public InsiderWallpapers()
        {
            InitializeComponent();

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Blue",
                ShortDescription = "Light Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm blue colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/LightBlue.png"
            });

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Blue",
                ShortDescription = "Dark Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm blue colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/DarkBlue.png"
            });

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Pink",
                ShortDescription = "Light Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm pink colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/LightPink.png"
            });

            Walls.Add(new Wallpaper
            {
                Name = "Abstract Pink",
                ShortDescription = "Dark Mode Version",
                Description = "Show off the Rise Media Player logo whilst " +
                "enjoying abstract shapes and a calm pink colour. Available in light " +
                "and dark variants.",
                Source = "ms-appx:///Assets/Wallpapers/DarkPink.png"
            });
        }

        private void WallsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = WallsView.SelectedIndex;
            if (index > -1)
            {
                string format = ResourceHelper.GetString("XofY");
                SelectedWall.Text = string.Format(format, index + 1, Walls.Count);

                var selected = Walls[index];
                WallName.Text = selected.Name;
                WallShortDesc.Text = selected.ShortDescription;
                WallDesc.Text = selected.Description;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = WallsView.SelectedItem as Wallpaper;
            var picFile = await StorageFile.
                GetFileFromApplicationUriAsync(new(item.Source));

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = item.Name
            };

            string fileFormat = ResourceHelper.GetString("Image");
            savePicker.FileTypeChoices[fileFormat] = new List<string>() { ".png" };

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
                await picFile.CopyAndReplaceAsync(file);
        }
    }
}
