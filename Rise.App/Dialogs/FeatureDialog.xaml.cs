using Rise.Common.Enums;
using Rise.Common.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class FeatureDialog : ContentDialog
    {
        private readonly ObservableCollection<Feature> _features = new();

        private int _index;
        private int Index
        {
            get => _index;
            set
            {
                _index = value;
                if (_index == 0)
                {
                    Left.IsEnabled = false;
                }
                else
                {
                    Left.IsEnabled = true;
                    if (_index == _features.Count - 1)
                    {
                        Right.IsEnabled = false;
                    }
                    else
                    {
                        Right.IsEnabled = true;
                    }
                }

                RefreshData();
            }
        }

        public FeatureDialog()
        {
            _features.Add(new Feature
            {
                Name = "Albums and Artists",
                Description = "Now you can see 'More albums by this artist' below an album. Quick switching! Albums are now shown on artist pages, meaning no messy song lists!",
                ImageUri = "ms-appx:///Assets/Branding/SettingsBanner.png"
            });

            _features.Add(new Feature
            {
                Name = "Now Playing Bar",
                Description = "The now playing bar is back with a new design! You can now view the song title, artist and thumbnail from the now playing bar and view a song's album in one click!",
                ImageUri = "ms-appx:///Assets/Branding/SettingsBanner.png"
            });

            _features.Add(new Feature
            {
                Name = "Contextual help",
                Description = "Now you can experience Discy in context menus!",
                ImageUri = "ms-appx:///Assets/Branding/SettingsBanner.png"
            });

            _features.Add(new Feature
            {
                Name = "More to come!",
                Description = "Watch this space",
                ImageUri = "ms-appx:///Assets/Branding/SettingsBanner.png"
            });
            InitializeComponent();
        }

        public async Task OpenFeatureAsync(int index)
        {
            Index = index;
            _ = await this.ShowAsync(ExistingDialogOptions.CloseExisting);
        }

        private void RefreshData()
        {
            FeatureName.Text = _features[Index].Name;
            Description.Text = _features[Index].Description;
            Screenshot.UriSource = new Uri(_features[Index].ImageUri);

            CurrentIndex.Text = (Index + 1).ToString();
        }

        private void Left_Click(object sender, RoutedEventArgs e)
            => Index--;

        private void Right_Click(object sender, RoutedEventArgs e)
            => Index++;

        private void Button_Click(object sender, RoutedEventArgs e)
            => Hide();

        private void ContentDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = Window.Current.Bounds.Width;
            double windowHeight = Window.Current.Bounds.Height;

            RootGrid.Width = windowWidth < 498 ? windowWidth - 68 : 498 - 68;

            // The 59 is because for some reason the dialog has a 1px transparent
            // line at the bottom. Don't shoot me, I'm just the messenger.
            RootGrid.Height = windowHeight < 398 ? windowHeight - 59 : 398 - 59;
        }
    }

    public class Feature
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
    }
}
