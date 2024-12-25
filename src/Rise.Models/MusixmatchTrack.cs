using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Models
{
    public sealed partial class MusixmatchTrack
    {
        [JsonProperty("message")]
        public TrackMessage Message { get; set; }
    }

    public sealed class TrackMessage
    {
        [JsonProperty("header")]
        public TrackHeader Header { get; set; }

        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public TrackBody Body { get; set; }
    }

    public sealed class TrackBody
    {
        [JsonProperty("track")]
        public Track1 Track { get; set; }
    }

    public sealed class Track1
    {
        [JsonProperty("track_id")]
        public long TrackId { get; set; }

        [JsonProperty("track_mbid")]
        public string TrackMbid { get; set; }

        [JsonProperty("track_isrc")]
        public string TrackIsrc { get; set; }

        [JsonProperty("commontrack_isrcs")]
        public string[][] CommontrackIsrcs { get; set; }

        [JsonProperty("track_spotify_id")]
        public string TrackSpotifyId { get; set; }

        [JsonProperty("commontrack_spotify_ids")]
        public string[] CommontrackSpotifyIds { get; set; }

        [JsonProperty("track_soundcloud_id")]
        public long TrackSoundcloudId { get; set; }

        [JsonProperty("track_xboxmusic_id")]
        public string TrackXboxmusicId { get; set; }

        [JsonProperty("track_name")]
        public string TrackName { get; set; }

        [JsonProperty("track_name_translation_list")]
        public TrackNameTranslationList[] TrackNameTranslationList { get; set; }

        [JsonProperty("track_rating")]
        public long TrackRating { get; set; }

        [JsonProperty("track_length")]
        public long TrackLength { get; set; }

        [JsonProperty("commontrack_id")]
        public long CommontrackId { get; set; }

        [JsonProperty("instrumental")]
        public long Instrumental { get; set; }

        [JsonProperty("explicit")]
        public long Explicit { get; set; }

        [JsonProperty("has_lyrics")]
        public long HasLyrics { get; set; }

        [JsonProperty("has_lyrics_crowd")]
        public long HasLyricsCrowd { get; set; }

        [JsonProperty("has_subtitles")]
        public long HasSubtitles { get; set; }

        [JsonProperty("has_richsync")]
        public long HasRichsync { get; set; }

        [JsonProperty("num_favourite")]
        public long NumFavourite { get; set; }

        [JsonProperty("lyrics_id")]
        public long LyricsId { get; set; }

        [JsonProperty("subtitle_id")]
        public long SubtitleId { get; set; }

        [JsonProperty("album_id")]
        public long AlbumId { get; set; }

        [JsonProperty("album_name")]
        public string AlbumName { get; set; }

        [JsonProperty("artist_id")]
        public long ArtistId { get; set; }

        [JsonProperty("artist_mbid")]
        public Guid ArtistMbid { get; set; }

        [JsonProperty("artist_name")]
        public string ArtistName { get; set; }

        [JsonProperty("album_coverart_100x100")]
        public Uri AlbumCoverart100X100 { get; set; }

        [JsonProperty("album_coverart_350x350")]
        public Uri AlbumCoverart350X350 { get; set; }

        [JsonProperty("album_coverart_500x500")]
        public Uri AlbumCoverart500X500 { get; set; }

        [JsonProperty("album_coverart_800x800")]
        public Uri AlbumCoverart800X800 { get; set; }

        [JsonProperty("track_share_url")]
        public Uri TrackShareUrl { get; set; }

        [JsonProperty("track_edit_url")]
        public Uri TrackEditUrl { get; set; }

        [JsonProperty("commontrack_vanity_id")]
        public string CommontrackVanityId { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }

        [JsonProperty("first_release_date")]
        public DateTimeOffset FirstReleaseDate { get; set; }

        [JsonProperty("updated_time")]
        public DateTimeOffset UpdatedTime { get; set; }

        [JsonProperty("primary_genres")]
        public TrackGenres PrimaryGenres { get; set; }

        [JsonProperty("secondary_genres")]
        public TrackGenres SecondaryGenres { get; set; }
    }

    public sealed class TrackGenres
    {
        [JsonProperty("music_genre_list")]
        public TrackMusicGenreList[] MusicGenreList { get; set; }
    }

    public sealed class TrackMusicGenreList
    {
        [JsonProperty("music_genre")]
        public TrackMusicGenre MusicGenre { get; set; }
    }

    public sealed class TrackMusicGenre
    {
        [JsonProperty("music_genre_id")]
        public long MusicGenreId { get; set; }

        [JsonProperty("music_genre_parent_id")]
        public long MusicGenreParentId { get; set; }

        [JsonProperty("music_genre_name")]
        public string MusicGenreName { get; set; }

        [JsonProperty("music_genre_name_extended")]
        public string MusicGenreNameExtended { get; set; }

        [JsonProperty("music_genre_vanity")]
        public string MusicGenreVanity { get; set; }
    }

    public sealed class TrackNameTranslationList
    {
        [JsonProperty("track_name_translation")]
        public TrackNameTranslation TrackNameTranslation { get; set; }
    }

    public sealed class TrackNameTranslation
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("translation")]
        public string Translation { get; set; }
    }

    public sealed class TrackHeader
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("execute_time")]
        public double ExecuteTime { get; set; }

        [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
        public long? Confidence { get; set; }

        [JsonProperty("mode", NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty("cached", NullValueHandling = NullValueHandling.Ignore)]
        public long? Cached { get; set; }

        [JsonProperty("hint", NullValueHandling = NullValueHandling.Ignore)]
        public string Hint { get; set; }
    }

    public sealed partial class MusixmatchTrack
    {
        public static MusixmatchTrack FromJson(string json) => JsonConvert.DeserializeObject<MusixmatchTrack>(json, TrackConverter.Settings);
    }

    internal static class TrackConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
