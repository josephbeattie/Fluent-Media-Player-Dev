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
            NavigationItemDestination item = (NavigationItemDestination)value;
            return Uri.TryCreate(item.DefaultIcon, UriKind.Absolute, out Uri uri)
                ? new BitmapIcon
                {
                    ShowAsMonochrome = false,
                    UriSource = uri
                }
                : string.IsNullOrEmpty(IconPack)
                ? new FontIcon { Glyph = item.DefaultIcon }
                : new BitmapIcon
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
