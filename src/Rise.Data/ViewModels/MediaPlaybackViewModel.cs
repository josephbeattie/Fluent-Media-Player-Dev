using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;

namespace Rise.Data.ViewModels
{
    /// <summary>
    /// This class contains properties and methods that
    /// help when it comes to controlling media playback.
    /// </summary>
    public sealed partial class MediaPlaybackViewModel : ViewModel
    {
        /// <summary>
        /// List of media items that are currently queued for playback.
        /// </summary>
        public ObservableCollection<MediaPlaybackItem> QueuedItems { get; private set; }
            = new();

        private readonly List<MediaPlayerEffect> _effects = new();
        /// <summary>
        /// Gets the added types of effects.
        /// </summary>
        public IReadOnlyCollection<MediaPlayerEffect> Effects => _effects.AsReadOnly();

        private MediaPlaybackItem _playingItem;
        /// <summary>
        /// Gets the media item that is currently playing.
        /// </summary>
        public MediaPlaybackItem PlayingItem
        {
            get => _playingItem;
            private set
            {
                Set(ref _playingItem, value);
                PlayingItemChanged?.Invoke(this, _playingItem);
            }
        }

        private NowPlayingDisplayProperties _playingItemProperties;
        /// <summary>
        /// Gets the media item that is currently playing.
        /// </summary>
        public NowPlayingDisplayProperties PlayingItemProperties
        {
            get => _playingItemProperties;
            private set
            {
                Set(ref _playingItemProperties, value);
                OnPropertyChanged(nameof(PlayingItemType));
            }
        }

        /// <summary>
        /// Gets the type of item that's currently playing.
        /// </summary>
        public MediaPlaybackType? PlayingItemType
            => PlayingItemProperties?.ItemType;

        private MediaPlayer _player;
        /// <summary>
        /// Gets the current <see cref="MediaPlayer"/> instance.
        /// Lazily instantiated to prevent Windows from showing the
        /// SMTC as soon as the app is opened.
        /// </summary>
        public MediaPlayer Player
        {
            get
            {
                if (_player == null)
                {
                    _player = CreatePlayerInstance();
                    foreach (var eff in _effects)
                        AddEffectInternal(eff);
                }
                return _player;
            }
        }

        private bool _playerCreated;
        /// <summary>
        /// Whether the main player has been created.
        /// </summary>
        public bool PlayerCreated
        {
            get => _playerCreated;
            private set => Set(ref _playerCreated, value);
        }

        /// <summary>
        /// The media playback list. It is permanently associated with
        /// the player, due to the fact that we don't ever dispose it.
        /// </summary>
        public MediaPlaybackList PlaybackList { get; private set; }
            = new();

        /// <summary>
        /// Whether the items in the playback list are played in a random
        /// order.
        /// </summary>
        public bool ShuffleEnabled
        {
            get => PlaybackList.ShuffleEnabled;
            set
            {
                PlaybackList.ShuffleEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    // Events
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Occurs when the media player is disposed and recreated. Also
        /// gets fired when the initial lazy instance takes place.
        /// </summary>
        public event EventHandler<MediaPlayer> MediaPlayerRecreated;

        /// <summary>
        /// Occurs when the currently playing item changes.
        /// </summary>
        public event EventHandler<MediaPlaybackItem> PlayingItemChanged;
    }

    // Methods, Event handling
    public partial class MediaPlaybackViewModel
    {
        /// <summary>
        /// Helps cancel a group of Tasks that starts playback.
        /// </summary>
        private readonly CancellableTaskHelper PlaybackCancelHelper = new();

        /// <summary>
        /// Adds the specified effect to the player.
        /// </summary>
        /// <remarks>If the player hasn't yet been created, the effect will still
        /// be applied as soon as it is.</remarks>
        public void AddEffect(MediaPlayerEffect effect)
        {
            AddEffectInternal(effect);
            _effects.Add(effect);
        }

        private void AddEffectInternal(MediaPlayerEffect effect)
        {
            if (_player != null)
            {
                if (effect.IsAudioEffect)
                    _player.AddAudioEffect(effect.EffectClassType.FullName, effect.IsOptional, effect.Configuration);
                else
                    _player.AddVideoEffect(effect.EffectClassType.FullName, effect.IsOptional, effect.Configuration);
            }
        }

        /// <summary>
        /// Removes all the effects from the player.
        /// </summary>
        public void ClearEffects()
        {
            if (_playerCreated)
                _player.RemoveAllEffects();

            _effects.Clear();
        }

        /// <summary>
        /// Begins playback of an <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="item">Item to play.</param>
        /// <remarks>This method will automatically be canceled if necessary
        /// without throwing <see cref="OperationCanceledException"/>.</remarks>
        public Task PlaySingleItemAsync(IMediaItem item)
        {
            return PlayItemsAsync(new IMediaItem[] { item });
        }

        /// <summary>
        /// Begins playback of an <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="item">Item to play.</param>
        public Task PlaySingleItemAsync(IMediaItem item, CancellationToken token)
        {
            return PlayItemsAsync(new IMediaItem[] { item }, token);
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="items">Items to play.</param>
        /// <remarks>This method will automatically be canceled if necessary
        /// without throwing <see cref="OperationCanceledException"/>.</remarks>
        public async Task PlayItemsAsync(IEnumerable<IMediaItem> items)
        {
            try
            {
                await PlayItemsAsync(items, CancellationToken.None);
            }
            catch (OperationCanceledException) { }
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="IMediaItem"/>.
        /// </summary>
        /// <param name="items">Items to play.</param>
        public async Task PlayItemsAsync(IEnumerable<IMediaItem> items, CancellationToken token)
        {
            await PlaybackCancelHelper.CompletePendingAsync(token);
            await PlaybackCancelHelper.RunAsync(PlayItemsImpl(items, PlaybackCancelHelper.Token));
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="files">Files to play.</param>
        /// <remarks>This method will automatically be canceled if necessary
        /// without throwing <see cref="OperationCanceledException"/>.</remarks>
        public async Task PlayFilesAsync(IEnumerable<StorageFile> files)
        {
            try
            {
                await PlayFilesAsync(files, CancellationToken.None);
            }
            catch (OperationCanceledException) { }
        }

        /// <summary>
        /// Begins playback of a collection of <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="files">Files to play.</param>
        public async Task PlayFilesAsync(IEnumerable<StorageFile> files, CancellationToken token)
        {
            await PlaybackCancelHelper.CompletePendingAsync(token);
            await PlaybackCancelHelper.RunAsync(PlayFilesImpl(files, PlaybackCancelHelper.Token));
        }

        private async Task PlayItemsImpl(IEnumerable<IMediaItem> items, CancellationToken token)
        {
            ResetPlayback();

            int i = 0;
            foreach (var item in items)
            {
                token.ThrowIfCancellationRequested();

                var playItem = await item.AsPlaybackItemAsync();
                PlaybackList.Items.Add(playItem);

                // Start playback right after adding the first item...
                if (i == 0)
                {
                    token.ThrowIfCancellationRequested();
                    Player.Play();

                    // ...and never again.
                    i++;
                }
            }
        }

        private async Task PlayFilesImpl(IEnumerable<StorageFile> files, CancellationToken token)
        {
            int i = 0;
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                string extension = file.FileType;

                MediaPlaybackItem itm = null;
                if (SupportedFileTypes.MusicFiles.Contains(extension))
                    itm = await file.GetSongAsync();
                else if (SupportedFileTypes.VideoFiles.Contains(extension))
                    itm = await file.GetVideoAsync();

                token.ThrowIfCancellationRequested();
                if (itm != null)
                {
                    // Start playback right after adding the first item...
                    if (i == 0)
                    {
                        token.ThrowIfCancellationRequested();

                        ResetPlayback();
                        Player.Play();

                        // ...and never again.
                        i++;
                    }

                    PlaybackList.Items.Add(itm);
                }
            }
        }

        /// <summary>
        /// Adds the provided item to the queue.
        /// </summary>
        public void AddSingleItemToQueue(MediaPlaybackItem item)
        {
            AddItemsToQueue(new MediaPlaybackItem[] { item });
        }

        /// <summary>
        /// Adds the provided items to the queue.
        /// </summary>
        public void AddItemsToQueue(IEnumerable<MediaPlaybackItem> items)
        {
            foreach (var itm in items)
                PlaybackList.Items.Add(itm);
        }

        /// <summary>
        /// Forces completion of all pending operations.
        /// </summary>
        public async Task CompletePendingAsync()
        {
            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(CancellationToken.None);
            }
            catch (OperationCanceledException) { }
        }

        /// <summary>
        /// Fully resets playback by completing all pending operations,
        /// clearing lists, and setting the current item to null.
        /// </summary>
        public async Task ResetPlaybackAsync()
        {
            await CompletePendingAsync();
            ResetPlayback();
        }

        /// <summary>
        /// Fully resets playback by clearing lists and setting the current
        /// item to null.
        /// </summary>
        private void ResetPlayback()
        {
            PlaybackList.Items.Clear();
            PlayingItem = null;
        }

        private void OnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (args.Reason == MediaPlaybackItemChangedReason.Error)
            {
                System.Diagnostics.Debug.WriteLine("Error when going to next item.");
                return;
            }

            var itm = args.NewItem;
            if (itm != null)
            {
                PlayingItemProperties = NowPlayingDisplayProperties.GetFromPlaybackItem(itm);
                PlayingItem = itm;
            }
        }

        private void OnItemsVectorChanged(IObservableVector<MediaPlaybackItem> sender, IVectorChangedEventArgs args)
        {
            int index = (int)args.Index;
            switch (args.CollectionChange)
            {
                case CollectionChange.ItemInserted:
                    var itm = sender[index];
                    QueuedItems.Insert(index, itm);
                    break;

                case CollectionChange.ItemRemoved:
                    QueuedItems.RemoveAt(index);
                    break;

                case CollectionChange.ItemChanged:
                    QueuedItems[index] = sender[index];
                    break;

                case CollectionChange.Reset:
                    QueuedItems.Clear();
                    break;
            }
        }
    }

    // Constructors, Initializers
    public partial class MediaPlaybackViewModel
    {
        public MediaPlaybackViewModel()
        {
            PlaybackList.CurrentItemChanged += OnCurrentItemChanged;
            PlaybackList.Items.VectorChanged += OnItemsVectorChanged;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MediaPlayer"/>,
        /// invokes the <see cref="MediaPlayerRecreated"/> event and returns
        /// the new instance.
        /// </summary>
        private MediaPlayer CreatePlayerInstance()
        {
            var player = new MediaPlayer();
            player.Source = PlaybackList;

            PlayerCreated = true;
            MediaPlayerRecreated?.Invoke(this, player);

            return player;
        }
    }
}
