using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class BoolToRoundedCorners : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string param)
            {
                if (param == "ForAlbum" && ((bool)value))
                {
                    return new CornerRadius(8);
                }
            }

            return new CornerRadius(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}
