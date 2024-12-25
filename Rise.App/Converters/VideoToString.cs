using Rise.App.ViewModels;
using System;
using System.IO;
using Windows.UI.Xaml.Data;

namespace Rise.App.Converters
{
    public sealed class VideoToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is VideoViewModel vid)
            {
                string format = "{0}\n{1} file\n{2}";

                return string.Format(format,
                    vid.Title,
                    Path.GetExtension(vid.Location),
                    TimeSpanToString.GetLongFormat(vid.Length, "D-S"));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
