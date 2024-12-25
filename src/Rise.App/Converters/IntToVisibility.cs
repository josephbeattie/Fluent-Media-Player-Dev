using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class IntToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
