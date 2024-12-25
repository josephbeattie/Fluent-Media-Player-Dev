using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class IntToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => (int)value > 0;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
