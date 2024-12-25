using Rise.Common.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.Common
{
    public sealed class StorageLibraryChangeResult : IAsyncDisposable
    {
        public StorageLibraryChangeStatus Status { get; }
        public IReadOnlyList<StorageFile> AddedItems { get; }
        public IReadOnlyList<string> RemovedItems { get; }

        private readonly StorageLibraryChangeReader changeReader;

        public StorageLibraryChangeResult(StorageLibraryChangeReader changeReader, IReadOnlyList<StorageFile> addedItems, IReadOnlyList<string> removedItems)
        {
            this.changeReader = changeReader;
            AddedItems = addedItems;
            RemovedItems = removedItems;

            Status = addedItems.Count > 0 || removedItems.Count > 0
                ? StorageLibraryChangeStatus.HasChange
                : StorageLibraryChangeStatus.NoChange;
        }

        public StorageLibraryChangeResult(StorageLibraryChangeStatus status)
        {
            AddedItems = new List<StorageFile>();
            RemovedItems = new List<string>();
            Status = status;
        }

        public async ValueTask DisposeAsync()
        {
            if (changeReader != null)
                await changeReader.AcceptChangesAsync();
        }
    }
}
