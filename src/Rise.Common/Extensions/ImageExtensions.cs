using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Rise.Common.Extensions
{
    /// <summary>
    /// Contains methods that aid in the creation and
    /// management of bitmaps.
    /// </summary>
    public static partial class ImageExtensions
    {
        /// <summary>
        /// Creates a bitmap source from a <see cref="SoftwareBitmap"/>.
        /// </summary>
        public static async Task<SoftwareBitmapSource> GetSourceAsync(this SoftwareBitmap bitmap)
        {
            if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                bitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied);
            }

            var source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(bitmap);

            return source;
        }

        /// <summary>
        /// Creates a bitmap source from a <see cref="WriteableBitmap"/>.
        /// </summary>
        public static Task<SoftwareBitmapSource> GetSourceAsync(this WriteableBitmap bitmap)
        {
            var bmp = SoftwareBitmap.CreateCopyFromBuffer(
                bitmap.PixelBuffer,
                BitmapPixelFormat.Bgra8,
                bitmap.PixelWidth,
                bitmap.PixelHeight
            );

            bmp = SoftwareBitmap.Convert(bmp, BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied);

            return bmp.GetSourceAsync();
        }
    }

    // Getting from files/streams
    public static partial class ImageExtensions
    {
        /// <summary>
        /// Creates a <see cref="SoftwareBitmap"/> from a <see cref="StorageFile"/>.
        /// </summary>
        public static async Task<SoftwareBitmap> GetBitmapAsync
            (this StorageFile file)
        {
            using var fileStream = await file.OpenReadAsync();
            return await fileStream.GetBitmapAsync();
        }

        /// <summary>
        /// Creates a <see cref="WriteableBitmap"/> from a
        /// <see cref="StorageFile"/> with the specified dimensions.
        /// </summary>
        public static async Task<WriteableBitmap> GetBitmapAsync
            (this StorageFile file, int width, int height)
        {
            using var fileStream = await file.OpenReadAsync();
            return await fileStream.GetBitmapAsync(width, height);
        }

        /// <summary>
        /// Creates a <see cref="SoftwareBitmap"/> from a <see cref="IRandomAccessStream"/>.
        /// </summary>
        public static async Task<SoftwareBitmap> GetBitmapAsync
            (this IRandomAccessStream stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            return await decoder.GetSoftwareBitmapAsync();
        }

        /// <summary>
        /// Creates a <see cref="WriteableBitmap"/> from a
        /// <see cref="IRandomAccessStream"/> with the specified dimensions.
        /// </summary>
        public static async Task<WriteableBitmap> GetBitmapAsync
            (this IRandomAccessStream stream, int width, int height)
        {
            using var memoryStream = new InMemoryRandomAccessStream();

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var transform = new BitmapTransform
            {
                ScaledWidth = (uint)width,
                ScaledHeight = (uint)height,
                InterpolationMode = BitmapInterpolationMode.Cubic
            };

            var pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.ColorManageToSRgb);

            var pixels = pixelData.DetachPixelData();

            var encoder = await BitmapEncoder.CreateAsync(
                BitmapEncoder.PngEncoderId, memoryStream);

            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                (uint)width, (uint)height, 96, 96, pixels);

            await encoder.FlushAsync();
            memoryStream.Seek(0);

            var bitmap = new WriteableBitmap(width, height);
            await bitmap.SetSourceAsync(memoryStream);

            return bitmap;
        }
    }

    // Saving to files
    public static partial class ImageExtensions
    {
        /// <summary>
        /// Saves a <see cref="WriteableBitmap"/> to a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="bitmap"><see cref="SoftwareBitmap"/> to save.</param>
        /// <param name="outputFile"><see cref="StorageFile"/> where the <see cref="SoftwareBitmap"/>
        /// should be stored.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static Task<bool> SaveToFileAsync(this WriteableBitmap bitmap, StorageFile outputFile)
        {
            var bmp = SoftwareBitmap.CreateCopyFromBuffer(bitmap.PixelBuffer,
                BitmapPixelFormat.Bgra8,
                bitmap.PixelWidth,
                bitmap.PixelHeight);

            return bmp.SaveToFileAsync(outputFile);
        }

        /// <summary>
        /// Saves a <see cref="SoftwareBitmap"/> to a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="bitmap"><see cref="SoftwareBitmap"/> to save.</param>
        /// <param name="outputFile"><see cref="StorageFile"/> where the <see cref="SoftwareBitmap"/>
        /// should be stored.</param>
        /// <returns>Whether or not the operation was successful.</returns>
        public static async Task<bool> SaveToFileAsync(this SoftwareBitmap bitmap, StorageFile outputFile)
        {
            using IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite);

            // Create an encoder with the desired format
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            // Set the software bitmap
            encoder.SetSoftwareBitmap(bitmap);
            encoder.IsThumbnailGenerated = true;

            try
            {
                await encoder.FlushAsync();
            }
            catch (Exception err)
            {
                const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                switch (err.HResult)
                {
                    case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                        // If the encoder does not support writing a thumbnail, then try again
                        // but disable thumbnail generation.
                        encoder.IsThumbnailGenerated = false;
                        break;
                    default:
                        return false;
                }
            }

            if (encoder.IsThumbnailGenerated == false)
            {
                await encoder.FlushAsync();
            }

            return true;
        }

        /// <summary>
        /// Converts a <see cref="StorageItemThumbnail"/> to a <see cref="Buffer"/> and saves it.
        /// </summary>
        /// <param name="thumbnail"><see cref="StorageItemThumbnail"/> to convert.</param>
        /// <param name="filename">Filename of output image.</param>
        /// <returns>true if the thumbnail could be saved, false otherwise.</returns>
        public static async Task<bool> SaveToFileAsync(this StorageItemThumbnail thumbnail,
            string filename,
            StorageFolder destination,
            CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting)
        {
            if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            {
                try
                {
                    var destinationFile = await destination.CreateFileAsync(filename, collisionOption);

                    Buffer buffer = new(Convert.ToUInt32(thumbnail.Size));
                    IBuffer iBuf = await thumbnail.ReadAsync(buffer,
                        buffer.Capacity, InputStreamOptions.None);

                    using IRandomAccessStream strm = await destinationFile.OpenAsync(FileAccessMode.ReadWrite);

                    _ = await strm.WriteAsync(iBuf);

                    return true;
                }
                catch (Exception e)
                {
                    e.WriteToOutput();
                }
            }

            return false;
        }
    }
}
