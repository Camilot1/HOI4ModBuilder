using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var stopwatch = Stopwatch.StartNew();
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
                Logger.Log($"Color regeneration: {mode} = {stopwatch.ElapsedMilliseconds} ms");
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
            var regeneratedProvinceToColor = new Dictionary<Province, int>(4096);
            var newColors = new HashSet<int>(4096);
            var adjacentColors = new List<int>(16);

            var modSettings = SettingsManager.Settings.GetModSettings();

            var provinces = AssembleProvinces(ProvinceManager.GetProvinces());
            var progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_PROVINCES);

            RegenerateProvincesColors(
                provinces,
                provinceChecker: p => true,
                borderProvinceChecker: p => true,
                hsvRangesProvider: p => modSettings.GetProvincesHSVRanges(p.Type), newColors,
                regeneratedProvinceToColor,
                progressCallBack: (cur, max) => progressCallback.Execute(cur, max)
            );

            return regeneratedProvinceToColor;
        }

        public static List<Province> AssembleProvinces(ICollection<Province> provinces)
            => AssembleProvinces(provinces, o => true, o => true);
        public static List<Province> AssembleProvinces(ICollection<Province> provinces, Func<Province, bool> checker)
            => AssembleProvinces(provinces, checker, checker);
        public static List<Province> AssembleProvinces(
            ICollection<Province> provinces, Func<Province, bool> checker, Func<Province, bool> borderChecker
        ) => Assemble(
            provinces, checker, borderChecker,
            (o, borderAction) => o.ForEachBorderProvince(borderO => borderAction(borderO))
        );


        public static List<State> AssembleStates(ICollection<State> states)
            => AssembleStates(states, o => true, o => true);
        public static List<State> AssembleStates(ICollection<State> states, Func<State, bool> checker)
            => AssembleStates(states, checker, checker);
        public static List<State> AssembleStates(
            ICollection<State> states, Func<State, bool> checker, Func<State, bool> borderChecker
        ) => Assemble(
            states, checker, borderChecker,
            (o, borderAction) => o.ForEachBorderState(borderO => borderAction(borderO))
        );

        public static List<StrategicRegion> AssembleRegions(ICollection<StrategicRegion> regions)
            => AssembleStates(regions, o => true, o => true);
        public static List<StrategicRegion> AssembleRegions(ICollection<StrategicRegion> regions, Func<StrategicRegion, bool> checker)
            => AssembleStates(regions, checker, checker);
        public static List<StrategicRegion> AssembleStates(
            ICollection<StrategicRegion> regions, Func<StrategicRegion, bool> checker, Func<StrategicRegion, bool> borderChecker
        ) => Assemble(
            regions, checker, borderChecker,
            (o, borderAction) => o.ForEachBorderRegion(borderO => borderAction(borderO))
        );


        public static List<T> Assemble<T>(ICollection<T> values, Func<T, bool> checker, Func<T, bool> borderChecker, Action<T, Action<T>> ForEachBorder)
        {
            var used = new HashSet<T>(values.Count);
            var bordersQueue = new Queue<T>(values.Count / 4);
            var assembleQueue = new Queue<T>(values.Count / 4);
            var list = new List<T>(values.Count);

            if (checker == null)
                checker = o => true;
            if (borderChecker == null)
                borderChecker = o => true;

            foreach (var o in values)
            {
                if (used.Contains(o) || !checker(o))
                    continue;
                used.Add(o);
                bordersQueue.Enqueue(o);

                DoBordersQueue();
                DoAssembleQueue();
            }

            void DoBordersQueue()
            {
                while (bordersQueue.Count > 0)
                {
                    var o = bordersQueue.Dequeue();
                    assembleQueue.Enqueue(o);

                    ForEachBorder(o, borderO =>
                    {
                        if (used.Contains(borderO) || !borderChecker(borderO))
                            return;
                        used.Add(borderO);
                        bordersQueue.Enqueue(borderO);
                    });
                }
            }

            void DoAssembleQueue()
            {
                while (assembleQueue.Count > 0)
                    list.Add(assembleQueue.Dequeue());
            }

            return list;
        }


        private static Dictionary<Province, int> ModeBasedOnStateColor()
        {
            var modSettings = SettingsManager.Settings.GetModSettings();
            var variation = modSettings.GetStateToProvinceColorVariationHSVRanges();

            var newColors = new HashSet<int>(4096);

            var regeneratedProvinceToColor = new Dictionary<Province, int>(4096);
            List<Province> provinces;
            ProgressCallback progressCallback;

            progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_STATES);
            var states = AssembleStates(StateManager.GetStates());
            int counter = 0;
            foreach (var state in states)
            {
                counter++;
                progressCallback.Execute(counter, states.Count);

                Utils.IntToRgb(state.Color, out var r, out var g, out var b);
                ColorUtils.RgbToHsv(r, g, b, out var h, out var s, out var v);
                var hsvRanges = variation.Variate(h, s, v);

                provinces = new List<Province>(state.Provinces);
                state.ForEachOtherBorderProvince(p => provinces.Add(p));

                RegenerateProvincesColors(
                    provinces,
                    provinceChecker: p => p.State == state && p.Type == EnumProvinceType.LAND,
                    borderProvinceChecker: p => p.State == state,
                    hsvRangesProvider: p => hsvRanges,
                    newColors,
                    regeneratedProvinceToColor,
                    progressCallBack: null
                );
            }

            provinces = AssembleProvinces(ProvinceManager.GetProvinces(), p => p.Type != EnumProvinceType.LAND);
            progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_PROVINCES);
            RegenerateProvincesColors(
                provinces,
                provinceChecker: p => true,
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

            if (provinceChecker == null)
                provinceChecker = p => true;
            if (borderProvinceChecker == null)
                borderProvinceChecker = p => true;

            int counter = 0;
            int maxCount = provinces.Count;
            foreach (var province in provinces)
            {
                counter++;
                progressCallBack?.Invoke(counter, maxCount);

                if (!provinceChecker(province))
                    continue;

                adjacentColors.Clear();
                int newColor;

                province.ForEachBorderProvince(borderProvince =>
                {
                    if (!borderProvinceChecker(borderProvince))
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
                    new Random(province.Id), adjacentColors, hsvRanges, c => !newColors.Contains(c)
                );

                regeneratedProvinceToColor[province] = newColor;
                newColors.Add(newColor);
            }
        }
    }
}
