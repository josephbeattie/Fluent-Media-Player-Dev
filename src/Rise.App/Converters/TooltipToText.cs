using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class TooltipToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ToolTip tooltip)
                return tooltip.Content;

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
