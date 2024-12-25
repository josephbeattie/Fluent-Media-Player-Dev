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
            TimeSpan span = value is TimeSpan time ?
                time : TimeSpan.Zero;

            string param = parameter?.ToString();
            return string.IsNullOrEmpty(param) ? GetShortFormat(span) : (object)GetLongFormat(ref span, param);
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
        {
            return GetLongFormat(ref value, format);
        }

        public static string GetShortFormat(TimeSpan span)
        {
            return GetShortFormat(ref span);
        }

        private static string GetLongFormat(ref TimeSpan value, string format)
        {
            StringBuilder timeBuilder = new();
            void AppendToBuilder(string resource, int count, bool addComma)
            {
                if (count <= 0)
                {
                    return;
                }

                string txt = ResourceHelper.GetLocalizedCount(resource, count);
                _ = timeBuilder.Append(txt);

                if (addComma)
                {
                    _ = timeBuilder.Append(", ");
                }
            }

            switch (format[0])
            {
                case 'D':
                    AppendToBuilder("Day", value.Days, true);

                    if (format[2] != 'D')
                    {
                        goto case 'H';
                    }

                    break;

                case 'H':
                    AppendToBuilder("Hour", value.Hours, true);

                    if (format[2] != 'H')
                    {
                        goto case 'M';
                    }

                    break;

                case 'M':
                    AppendToBuilder("Minute", value.Minutes, true);

                    if (format[2] != 'M')
                    {
                        goto case 'S';
                    }

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
            {
                return span.ToString("d.\\hh\\:mm\\:ss");
            }
            else
            {
                return span.Hours is >= 1 and <= 9
                    ? span.ToString("h\\:mm\\:ss")
                    : span.Hours >= 10 ? span.ToString("hh\\:mm\\:ss") : span.Minutes >= 10 ? span.ToString("mm\\:ss") : span.ToString("m\\:ss");
            }
        }
    }
}
