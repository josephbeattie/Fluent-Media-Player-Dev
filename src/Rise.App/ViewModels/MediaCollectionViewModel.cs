using CommunityToolkit.Mvvm.Input;
using Rise.App.Helpers;
using Rise.Common.Extensions;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.Collections;
using Rise.Data.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    /// <summary>
    /// A ViewModel for collections of items that can
    /// be played.
    /// </summary>
    public sealed partial class MediaCollectionViewModel : ViewModel, IDisposable
    {
        private readonly MediaPlaybackViewModel _player;
        private readonly IList<SongViewModel> _songs;

        /// <summary>
        /// The items in the collection.
        /// </summary>
        public GroupedCollectionView Items { get; }

        private MediaCollectionViewModel(IList<SongViewModel> songs, MediaPlaybackViewModel pvm)
        {
            _songs = songs;
            _player = pvm;
        }

        /// <summary>
        /// Initializes a new instance of this ViewModel.
        /// </summary>
        /// <param name="delegateKey">Key for the delegate to use for sorting. Must
        /// be present in <see cref="CollectionViewDelegates"/>.</param>
        /// <param name="itemSource">Source of items for the underlying
        /// ViewModel.</param>
        /// <param name="songs">The collection of songs to use. Only needed
        /// for albums, artists, and genres.</param>
        /// <param name="pvm">An instance of <see cref="MediaPlaybackViewModel"/>
        /// responsible for playback management.</param>
        public MediaCollectionViewModel(string delegateKey,
            SortDirection direction,
            bool groupAlphabetically,
            Predicate<object> filter,
            IList itemSource,
            IList<SongViewModel> songs,
            MediaPlaybackViewModel pvm)
            : this(songs, pvm)
        {
            _groupingAlphabetically = groupAlphabetically;
            if (string.IsNullOrEmpty(delegateKey))
            {
                var (items, defer) = GroupedCollectionView.CreateDeferred();
                items.Source = itemSource;
                items.Filter = filter;
                defer.Complete();

                Items = items;
            }
            else if (groupAlphabetically)
            {
                Items = CreateGroupedAlphabetically(delegateKey, direction, filter, itemSource);
            }
            else
            {
                Items = CreateSorted(delegateKey, direction, filter, itemSource);
            }
        }

        private GroupedCollectionView CreateGroupedAlphabetically(string delegateKey,
            SortDirection direction,
            Predicate<object> filter,
            IList itemSource)
        {
            var items = CreateSorted(delegateKey, direction, filter, itemSource);
            _ = items.AddCollectionGroups(CollectionViewDelegates.GroupingLabels);

            return items;
        }

        private GroupedCollectionView CreateSorted(string delegateKey,
            SortDirection direction,
            Predicate<object> filter,
            IList itemSource)
        {
            var (items, defer) = GroupedCollectionView.CreateDeferred();
            items.Source = itemSource;
            items.Filter = filter;

            Sort(items, delegateKey, direction);

            defer.Complete();
            return items;
        }

        public void Dispose()
            => Items.Dispose();
    }

    // Sorting
    public partial class MediaCollectionViewModel
    {
        private bool _groupingAlphabetically;
        /// <summary>
        /// Whether the <see cref="Items"/> collection is
        /// grouped alphabetically.
        /// </summary>
        public bool GroupingAlphabetically
        {
            get => _groupingAlphabetically;
            private set => Set(ref _groupingAlphabetically, value);
        }

        private string _currentDelegate;
        /// <summary>
        /// Gets the key for the delegate currently used for sorting.
        /// </summary>
        public string CurrentDelegate => _currentDelegate;

        private SortDirection _currentDirection = SortDirection.Ascending;
        /// <summary>
        /// Gets the current sort direction.
        /// </summary>
        public SortDirection CurrentSortDirection => _currentDirection;

        [RelayCommand]
        public void GroupAlphabetically(string delegateKey)
        {
            GroupingAlphabetically = true;

            Sort(Items, delegateKey, _currentDirection);
            _ = Items.AddCollectionGroups(CollectionViewDelegates.GroupingLabels);
        }

        [RelayCommand]
        public void SortBy(string delegateKey)
        {
            GroupingAlphabetically = false;
            Sort(Items, delegateKey, _currentDirection);
        }

        [RelayCommand]
        public void UpdateSortDirection(SortDirection direction)
        {
            Sort(Items, _currentDelegate, direction);
            if (_groupingAlphabetically)
                _ = Items.AddCollectionGroups(CollectionViewDelegates.GroupingLabels);
        }

        private void Sort(GroupedCollectionView items, string delegateKey, SortDirection direction)
        {
            _currentDelegate = delegateKey;
            _currentDirection = direction;

            var defer = items.DeferRefresh();

            items.GroupDescription = null;
            items.SortDescriptions.Clear();

            bool grouped = false;
            string[] keys = delegateKey.Split('|');

            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                var sortDel = CollectionViewDelegates.GetDelegate(key);

                if (!grouped && key[0] == 'G')
                {
                    grouped = true;
                    if (_groupingAlphabetically)
                        items.GroupDescription = new(direction, sortDel, GroupingLabelComparer.Default);
                    else
                        items.GroupDescription = new(direction, sortDel);
                }
                else
                {
                    items.SortDescriptions.Add(new(direction, sortDel));
                }
            }

            defer.Complete();
        }
    }

    // Playback
    public partial class MediaCollectionViewModel
    {
        /// <summary>
        /// Helps cancel a group of Tasks that starts playback.
        /// </summary>
        private readonly CancellableTaskHelper PlaybackCancelHelper = new();

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task PlayFromItemAsync(object parameter)
        {
            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(new CancellationToken());
                await PlaybackCancelHelper.RunAsync(GetPlayFromTask(parameter));
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task PlaySingleItemAsync(object parameter)
        {
            if (parameter == null)
            {
                await PlayFromItemAsync(parameter);
                return;
            }

            try
            {
                await PlaybackCancelHelper.CompletePendingAsync(new CancellationToken());
                await PlaybackCancelHelper.RunAsync(GetPlaySingleTask(parameter));
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private Task ShuffleFromItemAsync(object parameter)
        {
            _player.ShuffleEnabled = true;
            return PlayFromItemAsync(parameter);
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private Task ShuffleSingleItemAsync(object parameter)
        {
            _player.ShuffleEnabled = true;
            return PlaySingleItemAsync(parameter);
        }

        private Task GetPlaySingleTask(object parameter)
        {
            if (parameter is AlbumViewModel album)
                return PlaySingleAlbumAsync(album, PlaybackCancelHelper.Token);
            else if (parameter is ArtistViewModel artist)
                return PlaySingleArtistAsync(artist, PlaybackCancelHelper.Token);
            else if (parameter is GenreViewModel genre)
                return PlaySingleGenreAsync(genre, PlaybackCancelHelper.Token);
            else if (parameter is PlaylistViewModel playlist)
                return PlaySinglePlaylistAsync(playlist, PlaybackCancelHelper.Token);

            return PlaySingleItemAsync((IMediaItem)parameter, PlaybackCancelHelper.Token);
        }

        private Task GetPlayFromTask(object parameter)
        {
            if (parameter is IMediaItem item)
                return PlayFromItemAsync(item, PlaybackCancelHelper.Token);
            else if (parameter is AlbumViewModel album)
                return PlayFromAlbumAsync(album, PlaybackCancelHelper.Token);
            else if (parameter is ArtistViewModel artist)
                return PlayFromArtistAsync(artist, PlaybackCancelHelper.Token);
            else if (parameter is GenreViewModel genre)
                return PlayFromGenreAsync(genre, PlaybackCancelHelper.Token);
            else if (parameter is PlaylistViewModel playlist)
                return PlayFromPlaylistAsync(playlist, PlaybackCancelHelper.Token);

            return GetDefaultPlayFromTask();
        }

        private Task GetDefaultPlayFromTask()
        {
            var type = Items.First().GetType();
            if (type == typeof(AlbumViewModel))
                return PlayFromAlbumAsync(null, PlaybackCancelHelper.Token);
            else if (type == typeof(ArtistViewModel))
                return PlayFromArtistAsync(null, PlaybackCancelHelper.Token);
            else if (type == typeof(GenreViewModel))
                return PlayFromGenreAsync(null, PlaybackCancelHelper.Token);
            else if (type == typeof(PlaylistViewModel))
                return PlayFromPlaylistAsync(null, PlaybackCancelHelper.Token);

            return PlayFromItemAsync(null, PlaybackCancelHelper.Token);
        }
    }

    // PlaySingle* methods
    public partial class MediaCollectionViewModel
    {
        public Task PlaySingleItemAsync(IMediaItem item, CancellationToken token)
        {
            return _player.PlaySingleItemAsync(item, token);
        }

        public Task PlaySingleAlbumAsync(AlbumViewModel album, CancellationToken token)
        {
            var filtered = _songs.Where(s => s.Album == album.Title);
            token.ThrowIfCancellationRequested();

            var toPlay = filtered.OrderBy(s => s.Disc).ThenBy(s => s.Track);
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlaySingleArtistAsync(ArtistViewModel artist, CancellationToken token)
        {
            string name = artist.Name;
            var filtered = _songs.Where(s => s.Artist == name || s.AlbumArtist == name);
            token.ThrowIfCancellationRequested();

            var toPlay = filtered.OrderBy(s => s.Title);
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlaySingleGenreAsync(GenreViewModel genre, CancellationToken token)
        {
            var filtered = _songs.Where(s => s.Genres == genre.Name);
            token.ThrowIfCancellationRequested();

            var toPlay = filtered.OrderBy(s => s.Disc).ThenBy(s => s.Track);
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlaySinglePlaylistAsync(PlaylistViewModel playlist, CancellationToken token)
        {
            var toPlay = playlist.Songs.ToList<IMediaItem>();
            token.ThrowIfCancellationRequested();

            toPlay.AddRange(playlist.Videos);
            token.ThrowIfCancellationRequested();

            return _player.PlayItemsAsync(toPlay, token);
        }
    }

    // PlayFrom* methods
    public partial class MediaCollectionViewModel
    {
        public Task PlayFromItemAsync(IMediaItem item, CancellationToken token)
        {
            var items = Items.CloneList<object, IMediaItem>();
            token.ThrowIfCancellationRequested();

            if (item != null)
            {
                int index = items.IndexOf(item);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(items, token);
        }

        public Task PlayFromAlbumAsync(AlbumViewModel album, CancellationToken token)
        {
            var items = Items.CloneList<object, AlbumViewModel>();
            var songs = _songs.CloneList<object, SongViewModel>().
                OrderBy(s => s.Disc).ThenBy(s => s.Track);

            token.ThrowIfCancellationRequested();

            var toPlay = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            if (album != null)
            {
                int index = items.IndexOf(album);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(songs.Where(s => s.Album == itm.Title));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlayFromArtistAsync(ArtistViewModel artist, CancellationToken token)
        {
            var items = Items.CloneList<object, ArtistViewModel>();
            var songs = _songs.CloneList<object, SongViewModel>().OrderBy(s => s.Title);

            token.ThrowIfCancellationRequested();

            var toPlay = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            if (artist != null)
            {
                int index = items.IndexOf(artist);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(songs.Where(s => s.Artist == itm.Name || s.AlbumArtist == itm.Name));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlayFromGenreAsync(GenreViewModel genre, CancellationToken token)
        {
            var items = Items.CloneList<object, GenreViewModel>();
            var songs = _songs.CloneList<object, SongViewModel>().OrderBy(s => s.Title);

            token.ThrowIfCancellationRequested();

            var toPlay = new List<SongViewModel>();
            token.ThrowIfCancellationRequested();

            if (genre != null)
            {
                int index = items.IndexOf(genre);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(songs.Where(s => s.Genres == itm.Name));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }

        public Task PlayFromPlaylistAsync(PlaylistViewModel playlist, CancellationToken token)
        {
            var items = Items.CloneList<object, PlaylistViewModel>();
            token.ThrowIfCancellationRequested();

            var toPlay = new List<IMediaItem>();
            token.ThrowIfCancellationRequested();

            if (playlist != null)
            {
                int index = items.IndexOf(playlist);
                token.ThrowIfCancellationRequested();

                items.MoveRangeToEnd(0, index - 1);
                token.ThrowIfCancellationRequested();
            }

            foreach (var itm in items)
            {
                toPlay.AddRange(itm.Songs.OrderBy(s => s.Title));
                token.ThrowIfCancellationRequested();

                toPlay.AddRange(itm.Videos.OrderBy(v => v.Title));
                token.ThrowIfCancellationRequested();
            }

            return _player.PlayItemsAsync(toPlay, token);
        }
    }
}
