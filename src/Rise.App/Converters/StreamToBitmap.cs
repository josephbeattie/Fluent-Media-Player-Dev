using Rise.Common.Extensions;
using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Rise.App.Converters
{
    /// <summary>
    /// Creates asynchronously loaded bitmap images from random access streams.
    /// </summary>
    public sealed class StreamToBitmap : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IRandomAccessStream strm = value is IRandomAccessStream rand
                ? rand.CloneStream()
                : value is IRandomAccessStreamReference randRef
                ? (IRandomAccessStream)randRef.OpenReadAsync().Get()
                : throw new ArgumentException($"The provided value must be of type {typeof(IRandomAccessStream)}.", nameof(value));
            BitmapImage img = new();
            void OnImageOpened(object sender, RoutedEventArgs e)
            {
                strm.Dispose();
                img.ImageOpened -= OnImageOpened;
            }

            img.ImageOpened += OnImageOpened;

            _ = img.SetSourceAsync(strm);
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
