using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class AdditionSubtraction : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is int add && value is int val)
            {
                return (val + add).ToString();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
