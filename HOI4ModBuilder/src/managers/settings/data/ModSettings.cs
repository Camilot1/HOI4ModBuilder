using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

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

        public HSVRanges provinceLand_HSVRanges = GetDefaultLandHSVRanges();
        public HSVRanges provinceSea_HSVRanges = GetDefaultSeaHSVRanges();
        public HSVRanges provinceLake_HSVRanges = GetDefaultLakeHSVRanges();
        public HSVRanges state_HSVRanges = GetDefaultStateHSVRanges();
        public HSVRanges region_HSVRanges = GetDefaultRegionHSVRanges();

        public VariationHSVRanges stateToProvince_ColorVariation_HSVRanges = GetDefaultStateToProvinceColorVariationHSVRanges();

        public void PostInit()
        {
            if (provinceLand_HSVRanges == null)
                provinceLand_HSVRanges = GetDefaultLandHSVRanges();
            provinceLand_HSVRanges.Validate();

            if (provinceSea_HSVRanges == null)
                provinceSea_HSVRanges = GetDefaultSeaHSVRanges();
            provinceSea_HSVRanges.Validate();

            if (provinceLake_HSVRanges == null)
                provinceLake_HSVRanges = GetDefaultLakeHSVRanges();
            provinceLake_HSVRanges.Validate();

            if (state_HSVRanges == null)
                state_HSVRanges = GetDefaultStateHSVRanges();
            state_HSVRanges.Validate();

            if (region_HSVRanges == null)
                region_HSVRanges = GetDefaultRegionHSVRanges();
            region_HSVRanges.Validate();

            if (stateToProvince_ColorVariation_HSVRanges == null)
                stateToProvince_ColorVariation_HSVRanges = GetDefaultStateToProvinceColorVariationHSVRanges();
            stateToProvince_ColorVariation_HSVRanges.Validate();
        }

        public static HSVRanges GetDefaultLandHSVRanges()
            => new HSVRanges(0.4, double.PositiveInfinity, 0, 1, 0.2, 0.65, 0.75, 1);
        public static HSVRanges GetDefaultSeaHSVRanges()
            => new HSVRanges(0.4, double.PositiveInfinity, 0, 1, 0.4, 1, 0.05, 0.1);
        public static HSVRanges GetDefaultLakeHSVRanges()
            => new HSVRanges(0.4, double.PositiveInfinity, 0, 1, 0.4, 1, 0.05, 0.1);
        public static HSVRanges GetDefaultStateHSVRanges()
            => new HSVRanges(0.4, double.PositiveInfinity, 0, 1, 0.4, 0.8, 0.6, 1);
        public static HSVRanges GetDefaultRegionHSVRanges()
            => new HSVRanges(0.4, double.PositiveInfinity, 0, 1, 0.4, 0.8, 0.6, 1);
        public static VariationHSVRanges GetDefaultStateToProvinceColorVariationHSVRanges()
            => new VariationHSVRanges(0.125, 0.5, -0.05, 0.05, -0.1, 0.15, -0.1, 0.15);

        public HSVRanges GetProvincesHSVRanges(EnumProvinceType type)
        {
            if (type == EnumProvinceType.LAND) return provinceLand_HSVRanges;
            else if (type == EnumProvinceType.SEA) return provinceSea_HSVRanges;
            else if (type == EnumProvinceType.LAKE) return provinceLake_HSVRanges;
            else return new HSVRanges();
        }
        public HSVRanges GetStateHSVRanges() => state_HSVRanges;
        public HSVRanges GetRegionHSVRanges() => region_HSVRanges;
        public VariationHSVRanges GetStateToProvinceColorVariationHSVRanges() => stateToProvince_ColorVariation_HSVRanges;
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

    public class VariationHSVRanges : HSVRanges
    {
        public VariationHSVRanges() : base() { }
        public VariationHSVRanges(double minDif, double maxDif, double minH, double maxH, double minS, double maxS, double minV, double maxV)
            : base(minDif, maxDif, minH, maxH, minS, maxS, minV, maxV)
        { }
        public override void Validate()
        {
            ValidateInternal();
        }

        public HSVRanges Variate(double h, double s, double v)
        {
            double newH, newS, newV;

            if (h < minH)
                newH = minH;
            else if (1 - h < maxH)
                newH = (1 - maxH);
            else
                newH = h;

            if (s < minS)
                newS = minS;
            else if (1 - s < maxS)
                newS = (1 - maxS);
            else
                newS = s;

            if (v < minV)
                newV = minV;
            else if (1 - v < maxV)
                newV = (1 - maxV);
            else
                newV = v;

            var hsvRanges = new HSVRanges()
            {
                minDif = minDif,
                maxDif = maxDif,
                minH = newH + minH,
                maxH = newH + maxH,
                minS = newS + minS,
                maxS = newS + maxS,
                minV = newV + minV,
                maxV = newV + maxV
            };

            return hsvRanges;
        }
    }

    public class HSVRanges
    {
        public double minDif = 0, maxDif = double.PositiveInfinity;
        public double minH = 0, maxH = 1;
        public double minS = 0, maxS = 1;
        public double minV = 0, maxV = 1;

        public HSVRanges() { }
        public HSVRanges(double minDif, double maxDif, double minH, double maxH, double minS, double maxS, double minV, double maxV)
            : this()
        {
            this.minDif = minDif;
            this.maxDif = maxDif;
            this.minH = minH;
            this.maxH = maxH;
            this.minS = minS;
            this.maxS = maxS;
            this.minV = minV;
            this.maxV = maxV;
        }

        public override string ToString()
            => $"HSVRanges(minDif={minDif}; maxDif={maxDif}; minH={minH}; maxH={maxH}; minS={minS}; maxS={maxS}; minV={minV}; maxV={maxV})";

        public bool IsValidHSV(double h, double s, double v)
            => minH <= h && h <= maxH && minS <= s && s <= maxS && minV <= v && v <= maxV;

        public virtual void Validate()
        {
            ColorUtils.ValidateHueRange(ref minH, ref maxH);
            minS = MathUtils.Clamp(minS, 0.0, 1.0);
            maxS = MathUtils.Clamp(maxS, 0.0, 1.0);
            minV = MathUtils.Clamp(minV, 0.0, 1.0);
            maxV = MathUtils.Clamp(maxV, 0.0, 1.0);
            ValidateInternal();
        }

        protected void ValidateInternal()
        {
            if (maxDif < minDif)
                (minDif, maxDif) = (maxDif, minDif);

            if (maxH < minH)
                (minH, maxH) = (maxH, minH);

            if (maxS < minS)
                (minS, maxS) = (maxS, minS);

            if (maxV < minV)
                (minV, maxV) = (maxV, minV);

            if (double.IsNaN(minDif) || double.IsInfinity(minDif) || minDif < 0.0)
                throw new ArgumentOutOfRangeException(nameof(minDif));

            if (double.IsNaN(maxDif) || maxDif < 0.0)
                throw new ArgumentOutOfRangeException(nameof(maxDif));

            maxDif = double.IsInfinity(maxDif) || maxDif == 0.0
                ? double.PositiveInfinity
                : maxDif;

            if (maxDif < minDif)
                (minDif, maxDif) = (maxDif, minDif);
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
