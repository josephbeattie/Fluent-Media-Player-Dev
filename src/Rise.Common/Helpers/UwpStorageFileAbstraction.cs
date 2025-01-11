using System;
using System.IO;
using Windows.Storage;
using static TagLib.File;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// TagLib file abstraction for <see cref="StorageFile"/>s.
    /// </summary>
    public class UwpStorageFileAbstraction : IFileAbstraction
    {
        private readonly StorageFile file;

        public string Name => file.Name;

        public Stream ReadStream
            => file.OpenStreamForReadAsync().Result;

        public Stream WriteStream
            => file.OpenStreamForWriteAsync().Result;

        public UwpStorageFileAbstraction(StorageFile file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public void CloseStream(Stream stream)
        {
            stream?.Dispose();
        }
    }
}
