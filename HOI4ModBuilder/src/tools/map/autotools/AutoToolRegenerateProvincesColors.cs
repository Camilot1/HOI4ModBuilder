using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
            var task = new Task<Dictionary<Province, int>>(() =>
            {
                switch (mode)
                {
                    case EnumMode.RANDOM: return ModeRandom();
                    case EnumMode.BASED_ON_STATE_COLOR: return ModeBasedOnStateColor();
                }
                return null;
            });

            task.ContinueWith(obj =>
            {
                var result = obj.Result;
                int counter = 0;
                if (result != null)
                {
                    ReplaceColors(result);
                    counter = result.Count;
                }

                if (displayResultMessage)
                    PostAction(false, counter);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            task.Start();

        }

        private static void ReplaceColors(Dictionary<Province, int> regeneratedProvinceToColor)
        {
            var colorsToReplace = new Dictionary<int, int>(regeneratedProvinceToColor.Count);
            foreach (var entry in regeneratedProvinceToColor)
                colorsToReplace.Add(entry.Key.Color, entry.Value);

            MapManager.ReplacePixels(colorsToReplace);
            ProvinceManager.ReinitProvincesByColor(regeneratedProvinceToColor);

            MainForm.DisplayProgress(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_COMPLETED, 0);
        }

        private static Dictionary<Province, int> ModeRandom()
        {
            var random = new Random(0);
            var regeneratedProvinceToColor = new Dictionary<Province, int>(4096);
            var newColors = new HashSet<int>(4096);
            var adjacentColors = new List<int>(16);

            var modSettings = SettingsManager.Settings.GetModSettings();

            var provinces = new List<Province>(ProvinceManager.GetProvinces());
            var progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_PROVINCES);

            RegenerateProvincesColors(
                random,
                provinces,
                provinceChecker: p => true,
                borderProvinceChecker: p => true,
                hsvRangesProvider: p => modSettings.GetProvincesHSVRanges(p.Type), newColors,
                regeneratedProvinceToColor,
                progressCallBack: (cur, max) => progressCallback.Execute(cur, max)
            );

            return regeneratedProvinceToColor;
        }

        private static Dictionary<Province, int> ModeBasedOnStateColor()
        {
            var random = new Random(0);
            var modSettings = SettingsManager.Settings.GetModSettings();
            var variation = modSettings.GetStateToProvinceColorVariationHSVRanges();

            var newColors = new HashSet<int>(4096);

            var regeneratedProvinceToColor = new Dictionary<Province, int>(4096);
            List<Province> provinces;
            ProgressCallback progressCallback;

            progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_STATES);
            var states = StateManager.GetStates();
            int counter = 0;
            foreach (var state in states)
            {
                counter++;
                progressCallback.Execute(counter, states.Count);

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
                    regeneratedProvinceToColor,
                    progressCallBack: null
                );
            }

            provinces = new List<Province>(ProvinceManager.GetProvinces());
            progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_PROVINCES);
            RegenerateProvincesColors(
                random,
                provinces,
                provinceChecker: p => p.Type != EnumProvinceType.LAND,
                borderProvinceChecker: p => true,
                hsvRangesProvider: p => modSettings.GetProvincesHSVRanges(p.Type),
                newColors,
                regeneratedProvinceToColor,
                progressCallBack: (cur, max) => progressCallback.Execute(cur, max)
            );

            return regeneratedProvinceToColor;
        }

        public class ProgressCallback
        {
            public readonly Stopwatch stopwatch = Stopwatch.StartNew();
            public EnumLocKey displayLocKey;

            public ProgressCallback(EnumLocKey displayLocKey)
            {
                this.displayLocKey = displayLocKey;
            }

            public void Execute(int cur, int max)
            {
                float currentProgress = (cur / (float)max);
                if (cur == max || stopwatch.ElapsedMilliseconds > 100)
                {
                    stopwatch.Restart();
                    MainForm.DisplayProgress(
                        displayLocKey,
                        new Dictionary<string, string> { { "{current}", "" + cur }, { "{max}", "" + max } },
                        currentProgress
                    );
                }
            }
        }

        private static void RegenerateProvincesColors(
            Random random,
            List<Province> provinces,
            Func<Province, bool> provinceChecker,
            Func<Province, bool> borderProvinceChecker,
            Func<Province, HSVRanges> hsvRangesProvider,
            HashSet<int> newColors,
            Dictionary<Province, int> regeneratedProvinceToColor,
            Action<int, int> progressCallBack
            )
        {
            var modSettings = SettingsManager.Settings.GetModSettings();
            var adjacentColors = new List<int>(16);

            int counter = 0;
            int maxCount = provinces.Count;
            foreach (var province in provinces)
            {
                counter++;
                progressCallBack?.Invoke(counter, maxCount);

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
