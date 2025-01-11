using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class StringToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "0")
                {
                    return (value is string val && val.Length > 0) ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return (value is string str && str.Length > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("Use the boolean to visibility converter in this situation. " +
                "A string is very likely unnecessary in this case.");
        }
    }
}
