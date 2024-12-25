using Rise.Common.Extensions.Markup;
using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class ResourceFallback : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string param = parameter.ToString();
            if (value is string str && str != param)
                return value;
            return ResourceHelper.GetString(param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
