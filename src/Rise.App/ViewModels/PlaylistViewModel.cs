using Newtonsoft.Json;
using Rise.Common;
using Rise.Common.Constants;
using Rise.Common.Extensions;
using Rise.Common.Extensions.Markup;
using Rise.Common.Helpers;
using Rise.Common.Interfaces;
using Rise.Data.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using WindowsPlaylist = Windows.Media.Playlists.Playlist;
using System.Threading.Tasks;
using Windows.Storage;
using System.Xml;
using System.Linq;
using Windows.Storage.Streams;
using Windows.Storage.Search;

namespace Rise.App.ViewModels
{
    /// <summary>
    /// Represents a playlist with songs and videos.
    /// </summary>
    public sealed partial class PlaylistViewModel : ViewModel
    {
        /// <summary>
        /// A unique identifier for the playlist.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        private string _title;
        /// <summary>
        /// Gets or sets the playlist title.
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _icon;
        /// <summary>
        /// Gets or sets an icon for the playlist.
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        private string _description;
        /// <summary>
        /// Gets or sets the playlist title.
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        /// <summary>
        /// Gets the combined duration of the items
        /// in the playlist.
        /// </summary>
        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
    }

    // UI related properties
    public sealed partial class PlaylistViewModel
    {
        [JsonIgnore]
        public string LocalizedSongCount
            => ResourceHelper.GetLocalizedCount("Song", Songs.Count);

        [JsonIgnore]
        public string LocalizedVideoCount
            => ResourceHelper.GetLocalizedCount("Video", Videos.Count);

        [JsonIgnore]
        public string LocalizedSongsAndVideos
            => $"{LocalizedSongCount}, {LocalizedVideoCount}";
    }

    // Item management
    public sealed partial class PlaylistViewModel
    {
        public SafeObservableCollection<SongViewModel> Songs { get; init; } = new();
        public SafeObservableCollection<VideoViewModel> Videos { get; init; } = new();

        /// <summary>
        /// Adds a <see cref="IMediaItem" /> to the playlist.
        /// </summary>
        public void AddItem(IMediaItem item)
        {
            if (item is SongViewModel song)
                Songs.Add(song);
            else if (item is VideoViewModel video)
                Videos.Add(video);
        }

        /// <summary>
        /// Removes a <see cref="IMediaItem" /> from the playlist.
        /// </summary>
        public void RemoveItem(IMediaItem item)
        {
            if (item is SongViewModel song)
                _ = Songs.Remove(song);
            else if (item is VideoViewModel video)
                _ = Videos.Remove(video);
        }

        /// <summary>
        /// Adds multiple <see cref="IMediaItem"/>s to the playlist.
        /// </summary>
        public void AddItems(IEnumerable<IMediaItem> items)
        {
            foreach (IMediaItem item in items)
            {
                if (item is SongViewModel song)
                    Songs.Add(song);
                else if (item is VideoViewModel video)
                    Videos.Add(video);
            }
        }

        /// <summary>
        /// Removes multiple <see cref="IMediaItem"/>s from the playlist.
        /// </summary>
        public void RemoveItems(IEnumerable<IMediaItem> items)
        {
            foreach (IMediaItem item in items)
            {
                if (item is SongViewModel song)
                    _ = Songs.Remove(song);
                else if (item is VideoViewModel video)
                    _ = Videos.Remove(video);
            }
        }
    }

    // Playlist importing (requires improvements)
    public sealed partial class PlaylistViewModel
    {
        /// <summary>
        /// Creates a <see cref="PlaylistViewModel"/> based on a <see cref="IStorageFile"/>.
        /// </summary>
        /// <param name="file">Playlist file.</param>
        /// <returns>A playlist based on the file.</returns>
        public static async Task<PlaylistViewModel> GetFromFileAsync(IStorageFile file)
        {
            try
            {
                // Read playlist file
                switch (file.FileType)
                {
                    case ".wpl":
                    case ".zpl":
                        return await ParseWMPPlaylistAsync(file);
                    case ".m3u":
                    case ".m3u8":
                        var lines = await FileIO.ReadLinesAsync(file, UnicodeEncoding.Utf8);
                        return await ParseM3UAsync(lines, file.Path);
                }
            }
            catch (Exception e)
            {
                e.WriteToOutput();
            }

            return null;
        }

        public static async Task<PlaylistViewModel> GetFromFolderAsync(StorageFolder folder)
        {
            PlaylistViewModel playlist = new()
            {
                Title = folder.Name.ReplaceIfNullOrWhiteSpace(ResourceHelper.GetString("UntitledPlaylist")),
                Description = string.Empty,
                Icon = URIs.PlaylistThumb
            };

            var songs = await folder.CreateFileQueryWithOptions(QueryPresets.SongQueryOptions).GetFilesAsync();
            var videos = await folder.CreateFileQueryWithOptions(QueryPresets.VideoQueryOptions).GetFilesAsync();

            foreach (var item in songs)
                playlist.AddItem(new SongViewModel(await Song.GetFromFileAsync(item)));

            foreach (var item in videos)
                playlist.AddItem(new VideoViewModel(await Video.GetFromFileAsync(item)));

            return playlist;
        }

        private static async Task<PlaylistViewModel> ParseWMPPlaylistAsync(IStorageFile file)
        {
            PlaylistViewModel playlist = new()
            {
                Title = file.Name.ReplaceIfNullOrWhiteSpace(ResourceHelper.GetString("UntitledPlaylist")),
                Description = string.Empty,
                Icon = URIs.PlaylistThumb
            };

            WindowsPlaylist winrtPlaylist = await WindowsPlaylist.LoadAsync(file);
            var text = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);

            XmlDocument document = new();
            document.LoadXml(text);

            var head = document.SelectSingleNode("/smil/head");

            // Nodes must not be null to fetch info.
            if (head != null)
            {
                foreach (XmlNode node in head.ChildNodes)
                {
                    if (node.Name == "meta" && node.Attributes["name"].Value == "Subtitle")
                        playlist.Description = node.Attributes["content"].InnerText;
                    else if (node.Name == "title")
                        playlist.Title = node.InnerText;
                }
            }
            else
            {
                // TODO: error or something.
            }

            foreach (var playlistFile in winrtPlaylist.Files)
            {
                IMediaItem item = default;

                if (SupportedFileTypes.MusicFiles.Contains(playlistFile.FileType))
                    item = new SongViewModel(await Song.GetFromFileAsync(playlistFile));
                else if (SupportedFileTypes.VideoFiles.Contains(playlistFile.FileType))
                    item = new VideoViewModel(await Video.GetFromFileAsync(playlistFile));

                playlist.AddItem(item);
            }

            return playlist;
        }

        private static async Task<PlaylistViewModel> ParseM3UAsync(IList<string> lines, string baseFilePath)
        {
            PlaylistViewModel playlist = new()
            {
                Title = Path.GetFileName(baseFilePath),
                Description = string.Empty,
                Icon = URIs.PlaylistThumb
            };

            var trimmedLines = lines.Select(l => l.Trim()).ToList();

            // Check if linked to directory
            if (trimmedLines.Count == 1 && Uri.TryCreate(trimmedLines[0], UriKind.RelativeOrAbsolute, out var refUri))
            {
                Uri baseUri = new(Path.GetDirectoryName(baseFilePath));

                if (baseUri.AbsoluteUri.StartsWith("http") || baseUri.AbsoluteUri.StartsWith("https"))
                {
                    SongViewModel song = new()
                    {
                        Title = "Title",
                        Track = 0,
                        Disc = 0,
                        Album = "UnknownAlbumResource",
                        Artist = "UnknownArtistResource",
                        AlbumArtist = "UnknownArtistResource",
                        Location = baseUri.AbsoluteUri,
                        Thumbnail = URIs.MusicThumb
                    };

                    playlist.AddItem(song);

                    goto done;
                }

                string dirPath = refUri.ToAbsoluteUri(baseUri).AbsolutePath;

                if (dirPath.EndsWith(".m3u") || dirPath.EndsWith(".m3u8"))
                {
                    StorageFile linkedPlaylistFile = await StorageFile.GetFileFromPathAsync(dirPath);
                    return await GetFromFileAsync(linkedPlaylistFile);
                }

                foreach (var format in QueryPresets.SongQueryOptions.FileTypeFilter)
                {
                    if (dirPath.EndsWith(format))
                    {
                        playlist.Songs.Add(new SongViewModel()
                        {
                            Location = new Uri(dirPath).ToAbsoluteUri(baseUri).AbsolutePath,
                        });

                        goto done;
                    }
                }

                foreach (var songPath in Directory.EnumerateFiles(dirPath))
                {
                    playlist.Songs.Add(new SongViewModel()
                    {
                        Location = new Uri(songPath).ToAbsoluteUri(baseUri).AbsolutePath,
                    });
                }
                goto done;
            }

            // Get details
            string title = null, artist = null, icon = null;
            for (int i = 0; i < trimmedLines.Count; i++)
            {
                var line = trimmedLines[i];

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#"))
                {
                    int splitIdx = line.IndexOf(':');
                    string prop;
                    string value = null;
                    if (splitIdx >= 0)
                    {
                        prop = line.Substring(0, splitIdx).Trim();
                        value = line.Substring(splitIdx + 1).Trim();
                    }
                    else
                    {
                        prop = line;
                    }

                    if (prop == "#EXTINF")
                    {
                        string[] inf = value.Split(new[] { ',', '-' }, 3);
                        artist = inf[1].Trim();
                        title = inf[2].Trim();
                    }
                    else if (prop == "#EXTDESC" || prop == "#DESCRIPTION")
                    {
                        playlist.Description = value;
                    }
                    else if (prop == "#EXTIMG")
                    {
                        playlist.Icon = trimmedLines[++i];
                    }
                    else if (prop == "#PLAYLIST")
                    {
                        playlist.Title = value;
                    }

                    // Otherwise, we skip this line because we don't want anything from it
                    // or it's a whitespace
                }
                else
                {
                    SongViewModel song;

                    try
                    {
                        StorageFile songFile = await StorageFile.GetFileFromPathAsync(line);

                        if (songFile != null)
                            song = new(await Song.GetFromFileAsync(songFile));
                        else
                            song = new()
                            {
                                Title = "Title",
                                Track = 0,
                                Disc = 0,
                                Album = "UnknownAlbumResource",
                                Artist = "UnknownArtistResource",
                                AlbumArtist = "UnknownArtistResource",
                                Location = line,
                                Thumbnail = URIs.MusicThumb
                            };
                    }
                    catch (Exception e)
                    {
                        e.WriteToOutput();

                        song = new()
                        {
                            Title = "Title",
                            Track = 0,
                            Disc = 0,
                            Album = "UnknownAlbumResource",
                            Artist = "UnknownArtistResource",
                            AlbumArtist = "UnknownArtistResource",
                            Location = line,
                            Thumbnail = URIs.MusicThumb
                        };
                    }

                    if (song != null)
                    {
                        // If the playlist entry includes track info, override the tag data
                        if (title != null)
                        {
                            song.Title = title;
                            title = null;
                        }
                        if (artist != null)
                        {
                            song.Artist = artist;
                            artist = null;
                        }
                        if (icon != null)
                        {
                            song.Thumbnail = icon;
                            icon = null;
                        }

                        playlist.AddItem(song);
                    }
                }
            }

            done:
            return playlist;
        }
    }
}
