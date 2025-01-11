using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rise.Models
{
    [XmlRoot(ElementName = "artist")]
    public sealed class ArtistT
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "mbid")]
        public string Mbid { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "image")]
    public sealed class Image
    {
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "track")]
    public sealed class Track
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "playcount")]
        public string Playcount { get; set; }

        [XmlElement(ElementName = "listeners")]
        public string Listeners { get; set; }

        [XmlElement(ElementName = "mbid")]
        public string Mbid { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "streamable")]
        public string Streamable { get; set; }

        [XmlElement(ElementName = "artist")]
        public ArtistT Artist { get; set; }

        [XmlElement(ElementName = "image")]
        public List<Image> Image { get; set; }

        [XmlAttribute(AttributeName = "rank")]
        public string Rank { get; set; }
    }

    [XmlRoot(ElementName = "toptracks")]
    public sealed class Toptracks
    {
        [XmlElement(ElementName = "track")]
        public List<Track> Track { get; set; }

        [XmlAttribute(AttributeName = "artist")]
        public string Artist { get; set; }

        [XmlAttribute(AttributeName = "page")]
        public string Page { get; set; }

        [XmlAttribute(AttributeName = "perPage")]
        public string PerPage { get; set; }

        [XmlAttribute(AttributeName = "totalPages")]
        public string TotalPages { get; set; }

        [XmlAttribute(AttributeName = "total")]
        public string Total { get; set; }
    }

    [XmlRoot(ElementName = "lfm")]
    public sealed class LFM
    {
        [XmlElement(ElementName = "toptracks")]
        public Toptracks Toptracks { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }
}

