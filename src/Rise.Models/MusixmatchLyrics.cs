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
    public sealed partial class MusixmatchLyrics
    {
        [JsonProperty("message")]
        public LyricsMessage Message { get; set; }
    }

    public sealed partial class LyricsMessage
    {
        [JsonProperty("header")]
        public LyricsHeader Header { get; set; }

        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public LyricsBody Body { get; set; }
    }

    public sealed partial class LyricsBody
    {
        [JsonProperty("lyrics", NullValueHandling = NullValueHandling.Ignore)]
        public Lyrics Lyrics { get; set; }
    }

    public sealed partial class Lyrics
    {
        [JsonProperty("lyrics_id")]
        public long LyricsId { get; set; }

        [JsonProperty("can_edit")]
        public long CanEdit { get; set; }

        [JsonProperty("locked")]
        public long Locked { get; set; }

        [JsonProperty("published_status")]
        public long PublishedStatus { get; set; }

        [JsonProperty("action_requested")]
        public string ActionRequested { get; set; }

        [JsonProperty("verified")]
        public long Verified { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }

        [JsonProperty("instrumental")]
        public long Instrumental { get; set; }

        [JsonProperty("explicit")]
        public long Explicit { get; set; }

        [JsonProperty("lyrics_body")]
        public string LyricsBody { get; set; }

        [JsonProperty("lyrics_language")]
        public string LyricsLanguage { get; set; }

        [JsonProperty("lyrics_language_description")]
        public string LyricsLanguageDescription { get; set; }

        [JsonProperty("script_tracking_url")]
        public Uri ScriptTrackingUrl { get; set; }

        [JsonProperty("pixel_tracking_url")]
        public Uri PixelTrackingUrl { get; set; }

        [JsonProperty("html_tracking_url")]
        public Uri HtmlTrackingUrl { get; set; }

        [JsonProperty("lyrics_copyright")]
        public string LyricsCopyright { get; set; }

        [JsonProperty("writer_list")]
        public LyricsWriterList[] WriterList { get; set; }

        [JsonProperty("publisher_list")]
        public object[] PublisherList { get; set; }

        [JsonProperty("backlink_url")]
        public Uri BacklinkUrl { get; set; }

        [JsonProperty("updated_time")]
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public sealed partial class LyricsWriterList
    {
        [JsonProperty("writer")]
        public LyricsWriter Writer { get; set; }
    }

    public sealed partial class LyricsWriter
    {
        [JsonProperty("writer_id")]
        public long WriterId { get; set; }

        [JsonProperty("writer_name")]
        public string WriterName { get; set; }

        [JsonProperty("writer_vanity_id")]
        public string WriterVanityId { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }
    }

    public sealed partial class LyricsHeader
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("execute_time")]
        public double ExecuteTime { get; set; }

        [JsonProperty("hint", NullValueHandling = NullValueHandling.Ignore)]
        public string Hint { get; set; }
    }

    public sealed partial class MusixmatchLyrics
    {
        public static MusixmatchLyrics FromJson(string json) => JsonConvert.DeserializeObject<MusixmatchLyrics>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MusixmatchLyrics self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
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
