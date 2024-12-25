using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Enums;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Data.Json;
using Rise.Data.Messages;
using Rise.Data.ViewModels;
using Rise.Models;
using Rise.NewRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Rise.App.ViewModels
{
    public sealed partial class MainViewModel : ViewModel
    {
        private SafeObservableCollection<SongViewModel> _songs;
        /// <summary>
        /// The collection of songs in the list.
        /// </summary>
        public SafeObservableCollection<SongViewModel> Songs
            => _songs ??= CreateCollection<Song, SongViewModel>((s) => new(s));

        private SafeObservableCollection<AlbumViewModel> _albums;
        /// <summary>
        /// The collection of albums in the list.
        /// </summary>
        public SafeObservableCollection<AlbumViewModel> Albums
            => _albums ??= CreateCollection<Album, AlbumViewModel>((a) => new(a));

        private SafeObservableCollection<ArtistViewModel> _artists;
        /// <summary>
        /// The collection of artists in the list.
        /// </summary>
        public SafeObservableCollection<ArtistViewModel> Artists
            => _artists ??= CreateCollection<Artist, ArtistViewModel>((a) => new(a));

        private SafeObservableCollection<GenreViewModel> _genres;
        /// <summary>
        /// The collection of genres in the list.
        /// </summary>
        public SafeObservableCollection<GenreViewModel> Genres
            => _genres ??= CreateCollection<Genre, GenreViewModel>((g) => new(g));

        private SafeObservableCollection<VideoViewModel> _videos;
        /// <summary>
        /// The collection of videos in the list.
        /// </summary>
        public SafeObservableCollection<VideoViewModel> Videos
            => _videos ??= CreateCollection<Video, VideoViewModel>((v) => new(v));

        private JsonBackendController<PlaylistViewModel> _pBackend;
        public JsonBackendController<PlaylistViewModel> PBackend
            => _pBackend ??= JsonBackendController<PlaylistViewModel>.Get("SavedPlaylists");
        public SafeObservableCollection<PlaylistViewModel> Playlists => PBackend.Items;

        private JsonBackendController<BasicNotification> _nBackend;
        public JsonBackendController<BasicNotification> NBackend
            => _nBackend ??= JsonBackendController<BasicNotification>.Get("Messages");

        private static SafeObservableCollection<TOutput> CreateCollection<TEntity, TOutput>(Converter<TEntity, TOutput> converter)
            where TEntity : DbObject, new()
        {
            var items = Repository.GetItems<TEntity>();
            if (!items.Any())
                return new();

            return new(items.ConvertAll(converter));
        }
    }

    // Indexing libraries
    public sealed partial class MainViewModel
    {
        public event EventHandler IndexingStarted;
        public event EventHandler MetadataFetchingStarted;
        public event EventHandler<IndexingFinishedEventArgs> IndexingFinished;

        private bool _isScanning;

        /// <summary>
        /// Whether the app is currently looking for new media.
        /// </summary>
        public bool IsScanning
        {
            get => _isScanning;
            private set => Set(ref _isScanning, value);
        }

        private uint _indexedMedia = 0;
        /// <summary>
        /// The media indexed so far.
        /// </summary>
        public uint IndexedMedia
        {
            get => _indexedMedia;
            set => Set(ref _indexedMedia, value);
        }

        // Amount of indexed items. These are used to provide data to the
        // IndexingFinished event.
        private uint _indexedSongs = 0;
        private uint _indexedVideos = 0;

        /// <summary>
        /// Helps cancel indexing related Tasks.
        /// </summary>
        private readonly CancellableTaskHelper IndexingCancelHelper = new();

        public async Task StartFullCrawlAsync()
        {
            try
            {
                await StartFullCrawlAsync(new CancellationToken());
            }
            catch (OperationCanceledException) { }
        }

        public Task StartFullCrawlAsync(CancellationToken token)
        {
            return IndexingCancelHelper.CompletePendingAsync(token)
                .ContinueWith(async _ => await IndexingCancelHelper.RunAsync(StartFullCrawlImpl(IndexingCancelHelper.Token)));
        }

        private async Task StartFullCrawlImpl(CancellationToken token)
        {
            IsScanning = true;
            IndexingStarted?.Invoke(this, EventArgs.Empty);

            await IndexLibrariesAsync(token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            await Task.WhenAll(
                CheckMusicLibraryDuplicatesAsync(),
                CheckVideoLibraryDuplicatesAsync(token)
            );

            await HandleLibraryChangesAsync(ChangedLibraryType.Both, true);

            await Repository.UpsertQueuedAsync();
            await Repository.DeleteQueuedAsync();

            if (App.SViewModel.FetchOnlineData)
            {
                MetadataFetchingStarted?.Invoke(this, EventArgs.Empty);
                await FetchArtistsArtAsync(token);
            }

            IndexingFinished?.Invoke(this, new(_indexedSongs, _indexedVideos));
            IsScanning = false;

            _indexedSongs = 0;
            _indexedVideos = 0;
            IndexedMedia = 0;
        }

        private async Task IndexLibrariesAsync(CancellationToken token)
        {
            var songsTask = Task.Run(async () =>
            {
                await foreach (var song in KnownFolders.MusicLibrary.IndexWithPrefetchAsync(QueryPresets.SongQueryOptions,
                    PropertyPrefetchOptions.MusicProperties, SongProperties.DiscProperties))
                {
                    if (await SaveMusicModelsAsync(song, true).ConfigureAwait(false))
                        this._indexedSongs++;

                    IndexedMedia++;
                }
            }, token);

            var videosTask = Task.Run(async () =>
            {
                await foreach (var video in KnownFolders.VideosLibrary.IndexWithPrefetchAsync(QueryPresets.VideoQueryOptions,
                    PropertyPrefetchOptions.VideoProperties))
                {
                    if (await SaveVideoModelAsync(video, true).ConfigureAwait(false))
                        this._indexedVideos++;

                    IndexedMedia++;
                }
            }, token);

            await Task.WhenAll(songsTask, videosTask);
        }
    }

    // Artist images
    public sealed partial class MainViewModel
    {
        public async Task FetchArtistsArtAsync(CancellationToken token = default)
        {
            using var wc = new HttpClient(new HttpClientHandler()
            {
                Proxy = null,
                UseProxy = false
            });

            if (!WebHelpers.IsInternetAccessAvailable())
                return;

            foreach (var artist in Artists)
            {
                if (token != null && token.IsCancellationRequested)
                    return;

                // The ms-appx prefix is used for files within the app
                // bundle, and if it isn't present, it means a custom
                // image has already been applied
                if (!artist.Picture.StartsWith("ms-appx"))
                    return;

                if (!WebHelpers.IsInternetAccessAvailable())
                    return;

                string pic = await GetArtistImageAsync(artist.Name, wc).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(pic))
                    artist.Picture = pic;
            }
        }

        /// <summary>
        /// Gets artist image using the artist name.
        /// </summary>
        /// <param name="artist">The artist name to look up.</param>
        /// <param name="wc">The <see cref="HttpClient"/> used for the operation.</param>
        /// <returns>A string that represents the URL of the artist image if found, otherwise <see cref="string.Empty"/></returns>
        public async Task<string> GetArtistImageAsync(string artist, HttpClient wc)
        {
            if (artist != ResourceHelper.GetString("UnknownArtistResource"))
            {
                try
                {
                    string m_strFilePath = URLs.Deezer + "/search/artist/?q=" + artist + "&output=xml";

                    wc ??= new HttpClient(new HttpClientHandler()
                    {
                        Proxy = null,
                        UseProxy = false
                    });

                    string xmlStr = await wc.GetStringAsync(m_strFilePath);

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlStr);

                    var node = xmlDoc.DocumentElement.SelectSingleNode("/root/data/artist/picture_medium");
                    if (node != null)
                        return node.InnerText.Replace("<![CDATA[ ", string.Empty).Replace(" ]]>", string.Empty);
                }
                catch { }
            }

            return string.Empty;
        }
    }

    // Adding items
    public sealed partial class MainViewModel
    {
        /// <summary>
        /// Saves a song to the repository and ViewModel.
        /// </summary>
        /// <param name="file">Song file to add.</param>
        /// <returns>true if the song didn't exist beforehand,
        /// otherwise false.</returns>
        public async Task<bool> SaveMusicModelsAsync(StorageFile file, bool queue = false)
        {
            var song = await Song.GetFromFileAsync(file, false);

            // Check if song exists.
            bool songExists = Songs.Any(s => s.Model.Equals(song));

            // Check if album exists.
            var album = Albums.FirstOrDefault(a => a.Model.Title == song.Album);

            // Check if artist exists.
            bool artistExists = Artists.Any(a => a.Model.Name == song.Artist);

            // Check if genre exists.
            bool genreExists = Genres.Any(g => g.Model.Name == song.Genres);

            string albumArtist = song.AlbumArtist.ReplaceIfNullOrWhiteSpace(song.Artist);

            List<Task> tasks = new();

            // If album isn't there already, add it to the database.
            if (album == null)
            {
                album = new()
                {
                    Title = song.Album,
                    Artist = albumArtist,
                    Genres = song.Genres,
                    Thumbnail = URIs.AlbumThumb,
                    Year = song.Year
                };

                string filename = album.Model.Id.ToString();
                var (saved, path) = await Song.TrySaveThumbnailAsync(file, filename);

                song.Thumbnail = path;
                if (saved)
                    album.Thumbnail = path;

                tasks.Add(album.SaveAsync(queue));
            }
            else
            {
                // Update album information, in case previous songs didn't have it
                bool save = false;

                if (album.Model.Artist == "UnknownArtistResource" &&
                    albumArtist != "UnknownArtistResource")
                {
                    album.Model.Artist = albumArtist;
                    save = true;
                }

                if (album.Year == 0 && song.Year != 0)
                {
                    album.Year = song.Year;
                    save = true;
                }

                if (album.Thumbnail != URIs.AlbumThumb)
                {
                    song.Thumbnail = album.Thumbnail;
                }
                else
                {
                    string filename = album.Model.Id.ToString();
                    var (saved, path) = await Song.TrySaveThumbnailAsync(file, filename);

                    song.Thumbnail = path;
                    if (saved)
                    {
                        album.Thumbnail = path;
                        save = true;
                    }
                }

                if (save)
                    tasks.Add(album.SaveAsync(queue));
            }

            // If artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new()
                {
                    Name = song.Artist,
                    Picture = URIs.ArtistThumb
                };

                tasks.Add(arvm.SaveAsync(queue));
            }

            // Check for the album artist as well.
            artistExists = Artists.
                Any(a => a.Model.Name == song.Artist || a.Model.Name == song.AlbumArtist);

            // If album artist isn't there already, add it to the database.
            if (!artistExists)
            {
                ArtistViewModel arvm = new()
                {
                    Name = song.AlbumArtist,
                    Picture = URIs.ArtistThumb
                };

                tasks.Add(arvm.SaveAsync(queue));
            }

            // If genre isn't there already, add it to the database.
            if (!genreExists)
            {
                GenreViewModel gvm = new()
                {
                    Name = song.Genres
                };

                tasks.Add(gvm.SaveAsync(queue));
            }

            // If song isn't there already, add it to the database
            if (!songExists)
            {
                SongViewModel svm = new(song);
                tasks.Add(svm.SaveAsync(queue));
            }

            await Task.WhenAll(tasks);

            return !songExists;
        }

        /// <summary>
        /// Saves a video to the repository and ViewModel.
        /// </summary>
        /// <param name="queue">Whether to queue database operations. If set to
        /// true, you must call <see cref="Repository.UpsertQueuedAsync"/> to
        /// commit the changes to the database.</param>
        /// <param name="file">Video file to add.</param>
        /// <returns>true if the video didn't exist beforehand,
        /// otherwise false.</returns>
        public async Task<bool> SaveVideoModelAsync(StorageFile file, bool queue = false)
        {
            var video = await Video.GetFromFileAsync(file);
            bool videoExists = Videos.
                Any(v => v.Model.Equals(video));

            if (!videoExists)
            {
                VideoViewModel vid = new(video);
                await vid.SaveAsync(queue);
            }

            return !videoExists;
        }
    }

    // Removing items
    public sealed partial class MainViewModel
    {
        /// <summary>
        /// Removes the provided song from the database and songs collection.
        /// This method also checks if its album and artist can be removed.
        /// </summary>
        /// <param name="queue">Whether to queue database operations. If set to
        /// true, you must call <see cref="Repository.DeleteQueuedAsync"/> to
        /// commit the changes to the database.</param>
        public async Task RemoveSongAsync(SongViewModel song, bool queue)
        {
            _ = Songs.Remove(song);
            if (queue)
                _ = Repository.QueueRemove(song.Model);
            else
                _ = await Repository.DeleteAsync(song.Model);

            var artist = Artists.FirstOrDefault(a => a.Model.Name == song.Model.Artist);
            _ = await TryRemoveArtistAsync(artist, queue);

            var album = Albums.FirstOrDefault(a => a.Model.Title == song.Model.Album);
            bool removedAlbum = await TryRemoveAlbumAsync(album, queue);

            if (removedAlbum)
            {
                var albumArtist = Artists.FirstOrDefault(a => a.Model.Name == song.Model.AlbumArtist);
                _ = await TryRemoveArtistAsync(albumArtist, queue);
            }
        }

        /// <summary>
        /// Removes the provided album from the database and albums collection,
        /// only if there are no songs with the album.
        /// </summary>
        /// <param name="queue">Whether to queue database operations. If set to
        /// true, you must call <see cref="Repository.DeleteQueuedAsync"/> to
        /// commit the changes to the database.</param>
        /// <returns>true if the album was removed, false otherwise.</returns>
        public async Task<bool> TryRemoveAlbumAsync(AlbumViewModel album, bool queue)
        {
            bool hasNoTracks = Songs.Count(s => s.Model.Album == album.Model.Title) == 0;
            if (hasNoTracks)
            {
                _ = Albums.Remove(album);
                if (queue)
                    _ = Repository.QueueRemove(album.Model);
                else
                    _ = await Repository.DeleteAsync(album.Model);
            }

            return hasNoTracks;
        }

        /// <summary>
        /// Removes the provided artist from the database and artists collection,
        /// only if there are no songs or albums with the artist.
        /// </summary>
        /// <param name="queue">Whether to queue database operations. If set to
        /// true, you must call <see cref="Repository.DeleteQueuedAsync"/> to
        /// commit the changes to the database.</param>
        /// <returns>true if the artist was removed, false otherwise.</returns>
        public async Task<bool> TryRemoveArtistAsync(ArtistViewModel artist, bool queue)
        {
            var songCount = Songs.Count(s => s.Artist == artist.Model.Name);
            var albumCount = Albums.Count(a => a.Artist == artist.Model.Name);

            bool hasNoMedia = songCount == 0 && albumCount == 0;
            if (hasNoMedia)
            {
                _ = Artists.Remove(artist);
                if (queue)
                    _ = Repository.QueueRemove(artist.Model);
                else
                    _ = await Repository.DeleteAsync(artist.Model);
            }

            return hasNoMedia;
        }
    }

    // Change handling
    public sealed partial class MainViewModel
    {
        public async Task HandleLibraryChangesAsync(ChangedLibraryType type, bool queue = false)
        {
            switch (type)
            {
                case ChangedLibraryType.Music:
                    await HandleMusicLibraryChangesAsync(queue);
                    break;
                case ChangedLibraryType.Videos:
                    await HandleVideoLibraryChangesAsync(queue);
                    break;
                case ChangedLibraryType.Both:
                    await Task.WhenAll(
                        HandleMusicLibraryChangesAsync(queue),
                        HandleVideoLibraryChangesAsync(queue)
                    );
                    break;
                default:
                    throw new NotSupportedException($"The provided ChangedLibraryType ({type}) is not supported by this method.");
            }
        }

        private async Task HandleMusicLibraryChangesAsync(bool queue = false)
        {
            await using var changes = await App.MusicLibrary.GetLibraryChangesAsync();

            if (changes.Status != StorageLibraryChangeStatus.HasChange)
                return;

            foreach (var addedItem in changes.AddedItems)
            {
                _ = await SaveMusicModelsAsync(addedItem, queue);
            }

            foreach (var removedItemPath in changes.RemovedItems)
            {
                if (string.IsNullOrEmpty(removedItemPath))
                    continue;

                var song = Songs.FirstOrDefault(s => s.Location.Equals(removedItemPath, StringComparison.OrdinalIgnoreCase));
                if (song != null)
                    await RemoveSongAsync(song, queue);
            }
        }

        private async Task HandleVideoLibraryChangesAsync(bool queue = false)
        {
            await using var changes = await App.VideoLibrary.GetLibraryChangesAsync();

            if (changes.Status != StorageLibraryChangeStatus.HasChange)
                return;

            foreach (var addedItem in changes.AddedItems)
            {
                _ = await SaveVideoModelAsync(addedItem, queue);
            }

            foreach (var removedItemPath in changes.RemovedItems)
            {
                if (string.IsNullOrEmpty(removedItemPath))
                    continue;

                var video = App.MViewModel.Videos.FirstOrDefault(v => v.Location.Equals(removedItemPath, StringComparison.OrdinalIgnoreCase));

                if (video == null)
                    continue;

                await video.DeleteAsync(queue);
            }
        }

        /// <summary>
        /// Checks for duplicates in the music library.
        /// </summary>
        public async Task CheckMusicLibraryDuplicatesAsync()
        {
            List<SongViewModel> songDuplicates = new();
            List<ArtistViewModel> artistDuplicates = new();
            List<AlbumViewModel> albumDuplicates = new();
            List<GenreViewModel> genreDuplicates = new();

            // Check for duplicates and remove if any duplicate is found.
            for (int i = 0; i < Songs.Count; i++)
            {
                for (int j = i + 1; j < Songs.Count; j++)
                {
                    if (Songs[i].Location == Songs[j].Location)
                        songDuplicates.Add(Songs[j]);
                }
            }

            for (int i = 0; i < Artists.Count; i++)
            {
                for (int j = i + 1; j < Artists.Count; j++)
                {
                    if (Artists[i].Name.Equals(Artists[j].Name))
                        artistDuplicates.Add(Artists[j]);
                }
            }

            for (int i = 0; i < Albums.Count; i++)
            {
                for (int j = i + 1; j < Albums.Count; j++)
                {
                    if (Albums[i].Title.Equals(Albums[j].Title))
                        albumDuplicates.Add(Albums[j]);
                }
            }

            for (int i = 0; i < Genres.Count; i++)
            {
                for (int j = i + 1; j < Genres.Count; j++)
                {
                    if (Genres[i].Name.Equals(Genres[j].Name))
                        genreDuplicates.Add(Genres[j]);
                }
            }

            foreach (var song in songDuplicates)
                await RemoveSongAsync(song, true);

            foreach (var artist in artistDuplicates)
            {
                _ = Artists.Remove(artist);
                _ = Repository.QueueRemove(artist.Model);
            }

            foreach (var album in albumDuplicates)
            {
                _ = Albums.Remove(album);
                _ = Repository.QueueRemove(album.Model);
            }

            foreach (var genre in genreDuplicates)
            {
                _ = Genres.Remove(genre);
                _ = Repository.QueueRemove(genre.Model);
            }
        }

        /// <summary>
        /// Checks for duplicates in the video library.
        /// </summary>
        public async Task CheckVideoLibraryDuplicatesAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return;

            List<VideoViewModel> duplicates = new();

            // Check for duplicates and remove if any duplicate is found.
            for (int i = 0; i < Videos.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                for (int j = i + 1; j < Videos.Count; j++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (Videos[i].Location == Videos[j].Location)
                    {
                        duplicates.Add(Videos[j]);
                    }
                }
            }

            foreach (VideoViewModel video in duplicates)
            {
                if (token.IsCancellationRequested)
                    return;

                await video.DeleteAsync(true);
            }
        }
    }

    public sealed class IndexingFinishedEventArgs : EventArgs
    {
        public uint IndexedSongs { get; private set; }
        public uint IndexedVideos { get; private set; }

        public IndexingFinishedEventArgs(uint songs, uint videos)
        {
            IndexedSongs = songs;
            IndexedVideos = videos;
        }
    }
}
