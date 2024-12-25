using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class StringToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Reversed result
            if (parameter is string param)
            {
                if (param == "Reverse")
                {
                    return !(value is string val) || val.Length == 0;
                }
            }

            return value is string str && str.Length > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
