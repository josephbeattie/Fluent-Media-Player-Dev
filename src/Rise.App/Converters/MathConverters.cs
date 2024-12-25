using Rise.Common.Extensions.Markup;
using System;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class DecimalPointToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double val = Math.Floor((double)value * 100);
            return parameter is string param && param == "WithPercentage" ? val + "%" : (object)val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class UintToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            uint actualValue = (uint)value;
            string str = actualValue == 0 ? ResourceHelper.GetString("Unknown") : actualValue.ToString();
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class FormatNumber
    {
        public static string Format(long num)
        {
            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long)Math.Pow(10, (int)Math.Max(0, Math.Log10(num) - 2));
            num = num / i * i;

            return num >= 1000000000
                ? (num / 1000000000D).ToString("0.##") + "B"
                : num >= 1000000
                ? (num / 1000000D).ToString("0.##") + "M"
                : num >= 1000 ? (num / 1000D).ToString("0.##") + "K" : num.ToString("#,0");
        }
    }
}
