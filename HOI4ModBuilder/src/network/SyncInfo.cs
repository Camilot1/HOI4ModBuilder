using Newtonsoft.Json;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.network
{
    public class SyncInfo
    {
        [JsonProperty("lastVersion")]
        public string LastVersion { get; set; }
        [JsonProperty("shortDescription")]
        public Dictionary<string, string> ShortDescription { get; set; }
        [JsonProperty("links")]
        public Dictionary<string, string> Links { get; set; }

        public SyncInfo() { }
    }
}
