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
    public sealed partial class UserToken
    {
        [JsonProperty("message")]
        public TokenMessage Message { get; set; }
    }

    public sealed class TokenMessage
    {
        [JsonProperty("header")]
        public TokenHeader Header { get; set; }

        [JsonProperty("body")]
        public TokenBody Body { get; set; }
    }

    public sealed class TokenBody
    {
        [JsonProperty("user_token")]
        public string Token { get; set; }

        [JsonProperty("app_config")]
        public TokenAppConfig AppConfig { get; set; }

        [JsonProperty("location")]
        public TokenLocation Location { get; set; }
    }

    public sealed class TokenAppConfig
    {
        [JsonProperty("languages")]
        public string[] Languages { get; set; }

        [JsonProperty("trial")]
        public bool Trial { get; set; }

        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty("cluster")]
        public string Cluster { get; set; }

        [JsonProperty("event_map")]
        public TokenEventMap[] EventMap { get; set; }
    }

    public sealed class TokenEventMap
    {
        [JsonProperty("regex")]
        public string Regex { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("piggyback", NullValueHandling = NullValueHandling.Ignore)]
        public TokenPiggyback Piggyback { get; set; }
    }

    public sealed class TokenPiggyback
    {
        [JsonProperty("server_weight")]
        public long ServerWeight { get; set; }
    }

    public sealed class TokenLocation
    {
        [JsonProperty("GEOIP_CITY_COUNTRY_CODE")]
        public string GeoipCityCountryCode { get; set; }

        [JsonProperty("GEOIP_CITY_COUNTRY_CODE3")]
        public string GeoipCityCountryCode3 { get; set; }

        [JsonProperty("GEOIP_CITY_COUNTRY_NAME")]
        public string GeoipCityCountryName { get; set; }

        [JsonProperty("GEOIP_CITY")]
        public string GeoipCity { get; set; }

        [JsonProperty("GEOIP_CITY_CONTINENT_CODE")]
        public string GeoipCityContinentCode { get; set; }

        [JsonProperty("GEOIP_LATITUDE")]
        public double GeoipLatitude { get; set; }

        [JsonProperty("GEOIP_LONGITUDE")]
        public double GeoipLongitude { get; set; }

        [JsonProperty("GEOIP_AS_ORG")]
        public string GeoipAsOrg { get; set; }

        [JsonProperty("GEOIP_ORG")]
        public string GeoipOrg { get; set; }

        [JsonProperty("GEOIP_ISP")]
        public string GeoipIsp { get; set; }

        [JsonProperty("GEOIP_NET_NAME")]
        public string GeoipNetName { get; set; }

        [JsonProperty("BADIP_TAGS")]
        public object[] BadipTags { get; set; }
    }

    public sealed class TokenHeader
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("execute_time")]
        public double ExecuteTime { get; set; }

        [JsonProperty("pid")]
        public long Pid { get; set; }

        [JsonProperty("surrogate_key_list")]
        public object[] SurrogateKeyList { get; set; }
    }

    public sealed partial class UserToken
    {
        public static UserToken FromJson(string json) => JsonConvert.DeserializeObject<UserToken>(json, TokenConverter.Settings);
    }

    internal static class TokenConverter
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
