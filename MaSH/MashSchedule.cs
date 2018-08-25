using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MaSH
{
    public partial class MashSchedule
    {
        [JsonProperty("Updated")]
        public DateTimeOffset Updated { get; set; }

        [JsonProperty("MaSHVersion")]
        public string MaShVersion { get; set; }

        [JsonProperty("Apps")]
        public List<MashApp> Apps { get; set; }
    }

    public partial class MashApp
    {
        [JsonProperty("Id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Command")]
        public string Command { get; set; }

        [JsonProperty("Params")]
        public string Params { get; set; }

        [JsonProperty("Delay")]
        public int Delay { get; set; } = 1;

        public override string ToString()
        {
            return this.Name;
        }
    }

    public partial class MashSchedule
    {
        public static MashSchedule FromJson(string json) => JsonConvert.DeserializeObject<MashSchedule>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MashSchedule self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
