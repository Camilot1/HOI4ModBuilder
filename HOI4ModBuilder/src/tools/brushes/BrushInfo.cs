using Newtonsoft.Json;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.tools.brushes
{
    public class BrushInfo
    {
        [JsonProperty("localization")] public Dictionary<string, string> Localization { get; set; }

        public BrushInfo() { }
        public BrushInfo(string name)
        {
            Localization = new Dictionary<string, string>();

            foreach (var language in SettingsManager.SUPPORTED_LANGUAGES)
                Localization[language] = name;
        }
    }
}
