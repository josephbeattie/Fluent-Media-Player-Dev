using Rise.Common.Constants;
using System;
using System.Collections.Generic;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Rise.Common
{
    /// <summary>
    /// Contains query options used across the app to
    /// scan the music library.
    /// </summary>
    public static class QueryPresets
    {
        private static readonly Lazy<QueryOptions> _songOptions
            = new(() => CreateQueryOptions(SupportedFileTypes.MusicFiles, ThumbnailMode.MusicView, 134));
        public static QueryOptions SongQueryOptions => _songOptions.Value;

        private static readonly Lazy<QueryOptions> _videoOptions
            = new(() => CreateQueryOptions(SupportedFileTypes.VideoFiles, ThumbnailMode.VideosView, 238));
        public static QueryOptions VideoQueryOptions => _videoOptions.Value;

        private static readonly Lazy<QueryOptions> _playlistOptions
            = new(() => CreateQueryOptions(SupportedFileTypes.PlaylistFiles));
        public static QueryOptions PlaylistQueryOptions => _playlistOptions.Value;

        private static QueryOptions CreateQueryOptions(IEnumerable<string> fileTypeFilter)
        {
            return new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter)
            {
                FolderDepth = FolderDepth.Deep,
                IndexerOption = IndexerOption.UseIndexerWhenAvailable
            };
        }

        private static QueryOptions CreateQueryOptions(IEnumerable<string> fileTypeFilter,
            ThumbnailMode prefetchMode,
            uint prefetchSize)
        {
            QueryOptions options = CreateQueryOptions(fileTypeFilter);
            options.SetThumbnailPrefetch(prefetchMode, prefetchSize, ThumbnailOptions.None);
            return options;
        }
    }
}
