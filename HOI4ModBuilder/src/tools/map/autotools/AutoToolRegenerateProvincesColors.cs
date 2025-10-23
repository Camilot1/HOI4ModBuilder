using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolRegenerateProvincesColors : AbstractAutoTool
    {
        public enum EnumMode
        {
            RANDOM,
            BASED_ON_STATE_COLOR
        }
        public static void Execute(bool displayResultMessage, EnumMode mode)
        {
            int counter = 0;
            switch (mode)
            {
                case EnumMode.RANDOM: counter = ModeRandom(); break;
                case EnumMode.BASED_ON_STATE_COLOR: counter = ModeBasedOnStateColor(); break;
            }

            if (displayResultMessage)
                PostAction(false, counter);
        }

        private static int ModeRandom()
        {
            var random = new Random(0);
            var regeneratedProvinceToColor = new Dictionary<Province, int>(4096);
            var newColors = new HashSet<int>(4096);
            var adjacentColors = new List<int>(16);

            var modSettings = SettingsManager.Settings.GetModSettings();

            var provinces = new List<Province>(ProvinceManager.GetProvinces());

            RegenerateProvincesColors(
                random,
                provinces,
                provinceChecker: p => true,
                borderProvinceChecker: p => true,
                hsvRangesProvider: p => modSettings.GetProvincesHSVRanges(p.Type), newColors,
                regeneratedProvinceToColor
            );

            ReplaceColors(regeneratedProvinceToColor);

            return newColors.Count;
        }

        private static int ModeBasedOnStateColor()
        {
            var random = new Random(0);
            var modSettings = SettingsManager.Settings.GetModSettings();
            var variation = modSettings.GetStateToProvinceColorVariationHSVRanges();

            var newColors = new HashSet<int>(4096);

            var regeneratedProvinceToColor = new Dictionary<Province, int>(4096);
            List<Province> provinces;

            foreach (var state in StateManager.GetStates())
            {
                Utils.IntToRgb(state.Color, out var r, out var g, out var b);
                ColorUtils.RgbToHsv(r, g, b, out var h, out var s, out var v);
                var hsvRanges = variation.Variate(h, s, v);

                provinces = new List<Province>(state.Provinces);

                RegenerateProvincesColors(
                    random,
                    provinces,
                    provinceChecker: p => p.Type == EnumProvinceType.LAND,
                    borderProvinceChecker: p => p.State == state,
                    hsvRangesProvider: p => hsvRanges,
                    newColors,
                    regeneratedProvinceToColor
                );
            }

            provinces = new List<Province>(ProvinceManager.GetProvinces());
            RegenerateProvincesColors(
                random,
                provinces,
                provinceChecker: p => p.Type != EnumProvinceType.LAND,
                borderProvinceChecker: p => true,
                hsvRangesProvider: p => modSettings.GetProvincesHSVRanges(p.Type),
                newColors,
                regeneratedProvinceToColor
            );

            ReplaceColors(regeneratedProvinceToColor);

            return newColors.Count;
        }

        private static void ReplaceColors(Dictionary<Province, int> regeneratedProvinceToColor)
        {
            var colorsToReplace = new Dictionary<int, int>(regeneratedProvinceToColor.Count);
            foreach (var entry in regeneratedProvinceToColor)
                colorsToReplace.Add(entry.Key.Color, entry.Value);

            MapManager.ReplacePixels(colorsToReplace);
            ProvinceManager.ReinitProvincesByColor(regeneratedProvinceToColor);
        }

        private static void RegenerateProvincesColors(
            Random random,
            List<Province> provinces,
            Func<Province, bool> provinceChecker,
            Func<Province, bool> borderProvinceChecker,
            Func<Province, HSVRanges> hsvRangesProvider,
            HashSet<int> newColors,
            Dictionary<Province, int> regeneratedProvinceToColor
            )
        {
            var modSettings = SettingsManager.Settings.GetModSettings();
            var adjacentColors = new List<int>(16);

            foreach (var province in provinces)
            {
                if (provinceChecker != null && !provinceChecker(province))
                    continue;

                adjacentColors.Clear();
                int newColor;

                province.ForEachBorderProvince(borderProvince =>
                {
                    if (borderProvinceChecker != null && !borderProvinceChecker(borderProvince))
                        return false;
                    if (!regeneratedProvinceToColor.TryGetValue(borderProvince, out newColor))
                        return false;
                    adjacentColors.Add(newColor);
                    return false;
                });

                HSVRanges hsvRanges = null;
                if (hsvRangesProvider == null)
                    hsvRanges = modSettings.GetProvincesHSVRanges(province.Type);
                else
                    hsvRanges = hsvRangesProvider.Invoke(province);

                newColor = ColorUtils.GenerateDistinctColor(
                    random, adjacentColors, hsvRanges, c => !newColors.Contains(c)
                );

                regeneratedProvinceToColor[province] = newColor;
                newColors.Add(newColor);
            }
        }
    }
}
