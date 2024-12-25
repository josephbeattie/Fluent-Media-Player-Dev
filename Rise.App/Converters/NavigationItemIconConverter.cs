using Rise.Data.Navigation;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    /// <summary>
    /// Gets an icon for the provided <see cref="NavigationItemDestination"/>
    /// based on the current icon pack.
    /// </summary>
    public sealed class NavigationItemIconConverter : IValueConverter
    {
        private readonly string IconPack = App.SViewModel.IconPack;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = (NavigationItemDestination)value;
            if (Uri.TryCreate(item.DefaultIcon, UriKind.Absolute, out var uri))
            {
                return new BitmapIcon
                {
                    ShowAsMonochrome = false,
                    UriSource = uri
                };
            }

            if (string.IsNullOrEmpty(IconPack))
                return new FontIcon { Glyph = item.DefaultIcon };

            return new BitmapIcon
            {
                ShowAsMonochrome = false,
                UriSource = new($"ms-appx:///Assets/NavigationView/{item.Id}/{IconPack}.png")
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
