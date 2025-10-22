using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.settings
{
    public class ModSettings
    {
        public bool useCustomSavePatterns = true;
        public bool exportRiversMapWithWaterPixels = true;
        public bool generateNormalMap = false;

        [JsonProperty("normalMapStrengthV2")]
        public float normalMapStrength = 25f; //recomended 20-25
        [JsonProperty("normalMapBlur")]
        public float normalMapBlur = 0.8f; //recommended 0.5-0.8

        public HashSet<string> wipsEnabled = new HashSet<string>(StringComparer.Ordinal);

        public float MAP_SCALE_PIXEL_TO_KM = 7.114f;
        public float WATER_HEIGHT = 9.5f;
        [JsonProperty("WATER_HEIGHT_minLandOffsetV2")]
        public float WATER_HEIGHT_minLandOffset = -0.5f;
        [JsonProperty("WATER_HEIGHT_maxWaterOffsetV2")]
        public float WATER_HEIGHT_maxWaterOffset = 0.5f;

        public bool CheckWips(EnumWips enumWips) => wipsEnabled.Contains(enumWips.ToString());
        public void SetWips(EnumWips enumWips, bool value)
        {
            var key = enumWips.ToString();

            if (value)
                wipsEnabled.Add(key);
            else
                wipsEnabled.Remove(key);
        }
    }

    public enum EnumWips
    {
        SUB_UNITS,
        DIVISIONS_NAMES_GROUPS,
        OOBS,
        EQUIPMENTS
    }
}
