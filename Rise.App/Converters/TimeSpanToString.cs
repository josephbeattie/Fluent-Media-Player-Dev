using Rise.Common.Extensions.Markup;
using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed partial class TimeSpanToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var span = value is TimeSpan time ?
                time : TimeSpan.Zero;

            string param = parameter?.ToString();
            if (string.IsNullOrEmpty(param))
                return GetShortFormat(span);

            return GetLongFormat(ref span, param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // Converter logic
    public sealed partial class TimeSpanToString
    {
        public static string GetLongFormat(TimeSpan value, string format)
            => GetLongFormat(ref value, format);

        public static string GetShortFormat(TimeSpan span)
            => GetShortFormat(ref span);

        private static string GetLongFormat(ref TimeSpan value, string format)
        {
            var timeBuilder = new StringBuilder();
            void AppendToBuilder(string resource, int count, bool addComma)
            {
                if (count <= 0) return;

                string txt = ResourceHelper.GetLocalizedCount(resource, count);
                timeBuilder.Append(txt);

                if (addComma)
                    timeBuilder.Append(", ");
            }

            switch (format[0])
            {
                case 'D':
                    AppendToBuilder("Day", value.Days, true);

                    if (format[2] != 'D') goto case 'H';
                    break;

                case 'H':
                    AppendToBuilder("Hour", value.Hours, true);

                    if (format[2] != 'H') goto case 'M';
                    break;

                case 'M':
                    AppendToBuilder("Minute", value.Minutes, true);

                    if (format[2] != 'M') goto case 'S';
                    break;

                case 'S':
                    AppendToBuilder("Second", value.Seconds, false);
                    break;
            }

            return timeBuilder.ToString();
        }

        private static string GetShortFormat(ref TimeSpan span)
        {
            if (span.Days >= 1)
                return span.ToString("d.\\hh\\:mm\\:ss");
            else if (span.Hours >= 1 && span.Hours <= 9)
                return span.ToString("h\\:mm\\:ss");
            else if (span.Hours >= 10)
                return span.ToString("hh\\:mm\\:ss");
            else if (span.Minutes >= 10)
                return span.ToString("mm\\:ss");
            else
                return span.ToString("m\\:ss");
        }
    }
}
