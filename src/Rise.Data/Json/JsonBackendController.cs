using Newtonsoft.Json;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rise.Data.Json
{
    /// <summary>
    /// Represents a JSON based data store.
    /// </summary>
    /// <typeparam name="T">Type of items to store.</typeparam>
    public sealed partial class JsonBackendController<T>
    {
        private static readonly Dictionary<string, object> _controllers = new();

        private static StorageFolder _dataFolder;
        private static readonly StorageFolder dataFolder
            = _dataFolder ??= ApplicationData.Current.LocalCacheFolder;

        private readonly StorageFile BackingFile;
        private readonly SemaphoreSlim Semaphore = new(1);

        private JsonBackendController(StorageFile backingFile)
        {
            BackingFile = backingFile;
        }

        /// <summary>
        /// Gets or creates an instance of the backend controller and
        /// returns it.
        /// </summary>
        /// <param name="filename">Filename to use for the backing
        /// JSON file.</param>
        /// <returns>An instance of the backend controller.</returns>
        public static JsonBackendController<T> Get(string filename)
        {
            if (_controllers.ContainsKey(filename))
            {
                var cached = _controllers[filename];
                if (cached is JsonBackendController<T> ctrl)
                    return ctrl;
                throw new InvalidOperationException("The cached instance of this controller uses a different type.");
            }

            var file = dataFolder.CreateFileAsync($"{filename}.json", CreationCollisionOption.OpenIfExists).Get();
            var controller = new JsonBackendController<T>(file);

            var items = controller.GetStoredItems();
            controller.Items = new(items);

            _controllers[filename] = controller;
            return controller;
        }

        /// <summary>
        /// Asynchronously gets or creates an instance of the backend controller
        /// and returns it.
        /// </summary>
        /// <param name="filename">Filename to use for the backing
        /// JSON file.</param>
        /// <returns>An instance of the backend controller.</returns>
        public static async Task<JsonBackendController<T>> GetAsync(string filename)
        {
            if (_controllers.ContainsKey(filename))
            {
                var cached = _controllers[filename];
                if (cached is JsonBackendController<T> ctrl)
                    return ctrl;
                throw new InvalidOperationException("The cached instance of this controller uses a different type.");
            }

            var file = await dataFolder.CreateFileAsync($"{filename}.json", CreationCollisionOption.OpenIfExists);
            var controller = new JsonBackendController<T>(file);

            var items = await controller.GetStoredItemsAsync();
            controller.Items = new(items);

            _controllers[filename] = controller;
            return controller;
        }
    }

    // JSON handling
    public sealed partial class JsonBackendController<T>
    {
        /// <summary>
        /// The collection of items in the controller.
        /// </summary>
        public SafeObservableCollection<T> Items { get; private set; }

        /// <summary>
        /// Gets the items currently saved in the JSON file.
        /// </summary>
        public IEnumerable<T> GetStoredItems()
        {
            var text = FileIO.ReadTextAsync(BackingFile).Get();
            if (!string.IsNullOrWhiteSpace(text))
                return JsonConvert.DeserializeObject<IEnumerable<T>>(text);

            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Gets the items currently saved in the JSON file.
        /// </summary>
        public async Task<IEnumerable<T>> GetStoredItemsAsync()
        {
            var text = await FileIO.ReadTextAsync(BackingFile);
            if (!string.IsNullOrWhiteSpace(text))
                return JsonConvert.DeserializeObject<IEnumerable<T>>(text);

            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Saves the item list to the JSON file.
        /// </summary>
        public void Save()
        {
            Semaphore.Wait();

            string json = JsonConvert.SerializeObject(Items);
            FileIO.WriteTextAsync(BackingFile, json).Get();

            _ = Semaphore.Release();
        }

        /// <summary>
        /// Asynchronously asaves the item list to the JSON file.
        /// </summary>
        /// <returns>A task that represents the save operation.</returns>
        public async Task SaveAsync()
        {
            await Semaphore.WaitAsync();

            string json = JsonConvert.SerializeObject(Items);
            await FileIO.WriteTextAsync(BackingFile, json);

            _ = Semaphore.Release();
        }
    }
}
