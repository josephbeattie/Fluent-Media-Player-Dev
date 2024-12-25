using Rise.Common.Constants;
using Rise.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Rise.Common.Extensions
{
    public static class IndexingExtensions
    {
        #region Indexing
        /// <summary>
        /// Indexes a library's contents based on personalized
        /// query options.
        /// </summary>
        /// <param name="library">Library to index.</param>
        /// <param name="queryOptions">Query options.</param>
        /// <param name="prefetchOptions">What options to prefetch.</param>
        /// <param name="extraProps">Extra properties to prefetch.</param>
        public static async IAsyncEnumerable<StorageFile> IndexAsync(this StorageLibrary library,
            QueryOptions queryOptions,
            PropertyPrefetchOptions prefetchOptions = PropertyPrefetchOptions.BasicProperties,
            IEnumerable<string> extraProps = null)
        {
            // Prefetch file properties.
            queryOptions.SetPropertyPrefetch(prefetchOptions, extraProps);

            // Index library.
            foreach (StorageFolder folder in library.Folders)
            {
                await foreach (var file in folder.IndexAsync(queryOptions).ConfigureAwait(false))
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// Indexes a folder's contents based on personalized
        /// query options.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        /// <param name="options">Query options.</param>
        /// <param name="prefetchOptions">What options to prefetch.</param>
        /// <param name="extraProps">Extra properties to prefetch.</param>
        /// <param name="stepSize">The step size. This allows for
        /// the files to be indexed and processed in batches. Must
        /// be 1 or greater.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// an invalid <paramref name="stepSize"/> is specified.</exception>
        public static IAsyncEnumerable<StorageFile> IndexWithPrefetchAsync(this StorageFolder folder,
            QueryOptions queryOptions,
            PropertyPrefetchOptions prefetchOptions = PropertyPrefetchOptions.BasicProperties,
            IEnumerable<string> extraProps = null,
            uint stepSize = 50)
        {
            queryOptions.SetPropertyPrefetch(prefetchOptions, extraProps);
            return IndexAsync(folder, queryOptions, stepSize);
        }

        /// <summary>
        /// Indexes a folder's contents based on personalized
        /// query options.
        /// </summary>
        /// <param name="folder">Folder to index.</param>
        /// <param name="options">Query options.</param>
        /// <param name="stepSize">The step size. This allows for
        /// the files to be indexed and processed in batches. Must
        /// be 1 or greater.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// an invalid <paramref name="stepSize"/> is specified.</exception>
        public static async IAsyncEnumerable<StorageFile> IndexAsync(this StorageFolder folder,
            QueryOptions options,
            uint stepSize = 50)
        {
            if (stepSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stepSize));
            }

            int indexedFiles = 0;

            // Prepare the query
            StorageFileQueryResult folderQueryResult = folder.CreateFileQueryWithOptions(options);

            // Index by steps
            uint index = 0;

            IReadOnlyList<StorageFile> fileList = await folderQueryResult.GetFilesAsync(index, stepSize);
            index += stepSize;

            // Start crawling data
            while (fileList.Count != 0)
            {
                // Process files
                foreach (StorageFile file in fileList)
                {
                    indexedFiles++;
                    yield return file;
                }

                fileList = await folderQueryResult.GetFilesAsync(index, stepSize).AsTask().ConfigureAwait(false);
                index += stepSize;
            }
        }
        #endregion

        #region Tracking
        public static async Task<StorageLibraryChangeResult> GetLibraryChangesAsync(this StorageLibrary library)
        {
            var changeTracker = library.ChangeTracker;

            // Ensure that the change tracker is always enabled.
            changeTracker.Enable();

            var changeReader = changeTracker.GetChangeReader();
            var changes = await changeReader.ReadBatchAsync();

            var addedItems = new List<StorageFile>();
            var removedItems = new List<string>();

            if (ApiInformation.IsMethodPresent(typeof(StorageLibraryChangeReader).FullName, "GetLastChangeId"))
            {
                ulong lastChangeId = changeReader.GetLastChangeId();
                if (lastChangeId == StorageLibraryLastChangeId.Unknown)
                {
                    changeTracker.Reset();
                    return new StorageLibraryChangeResult(StorageLibraryChangeStatus.Unknown);
                }
            }

            foreach (StorageLibraryChange change in changes)
            {
                if (change.ChangeType == StorageLibraryChangeType.ChangeTrackingLost || change.IsOfType(StorageItemTypes.None))
                {
                    changeTracker.Reset();
                    return new StorageLibraryChangeResult(StorageLibraryChangeStatus.Unknown);
                }

                switch (change.ChangeType)
                {
                    case StorageLibraryChangeType.MovedIntoLibrary:
                    case StorageLibraryChangeType.Created:
                        {
                            var item = await change.GetStorageItemAsync();
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var file = (StorageFile)item;
                                if (!SupportedFileTypes.MediaFiles.Contains(file.FileType.ToLowerInvariant()))
                                    continue;

                                addedItems.Add(file);
                            }
                            break;
                        }

                    case StorageLibraryChangeType.MovedOutOfLibrary:
                    case StorageLibraryChangeType.Deleted:
                        {
                            removedItems.Add(change.Path);
                            break;
                        }

                    case StorageLibraryChangeType.MovedOrRenamed:
                    case StorageLibraryChangeType.ContentsChanged:
                    case StorageLibraryChangeType.ContentsReplaced:
                        {
                            var item = await change.GetStorageItemAsync();
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var file = (StorageFile)item;
                                if (!SupportedFileTypes.MediaFiles.Contains(file.FileType.ToLowerInvariant()))
                                    continue;

                                string changePath = change.PreviousPath.ReplaceIfNullOrWhiteSpace(file.Path);

                                removedItems.Add(changePath);
                                addedItems.Add(file);
                            }
                            break;
                        }

                    case StorageLibraryChangeType.IndexingStatusChanged:
                    case StorageLibraryChangeType.EncryptionChanged:
                    case StorageLibraryChangeType.ChangeTrackingLost:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new StorageLibraryChangeResult(changeReader, addedItems, removedItems);
        }

        /// <summary>
        /// Registers a background <see cref="StorageLibraryChangeTracker"/>.
        /// </summary>
        /// <param name="library">Library to track.</param>
        /// <param name="taskName">Preferred background task name.</param>
        /// <param name="entryPoint">The <see cref="BackgroundTaskBuilder.TaskEntryPoint"/>.
        /// If not provided, the single process model will be used for the background
        /// task.</param>
        /// <returns>A <see cref="BackgroundTaskRegistrationStatus" /> which represents the status.</returns>
        public static async Task<BackgroundTaskRegistrationStatus> TrackBackgroundAsync(this StorageLibrary library,
            string taskName, string entryPoint = null)
        {
            // Check if there's access to the background.
            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (!(requestStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                requestStatus == BackgroundAccessStatus.AlwaysAllowed))
            {
                return BackgroundTaskRegistrationStatus.NotAllowed;
            }

            library.ChangeTracker.Enable();
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                    return BackgroundTaskRegistrationStatus.AlreadyExists;
            }

            // Build up the trigger to fire when something changes in the library.
            var builder = new BackgroundTaskBuilder();
            builder.Name = taskName;

            if (entryPoint != null)
                builder.TaskEntryPoint = entryPoint;

            var libraryTrigger = StorageLibraryContentChangedTrigger.Create(library);

            builder.SetTrigger(libraryTrigger);
            _ = builder.Register();

            return BackgroundTaskRegistrationStatus.Successful;
        }
        #endregion
    }
}
