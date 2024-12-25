using Rise.Common.Extensions.Markup;
using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class ResourceToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null)
            {
                string loader = parameter.ToString();
                return ResourceHelper.GetString($"/{loader}/{value}");
            }

            return ResourceHelper.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
