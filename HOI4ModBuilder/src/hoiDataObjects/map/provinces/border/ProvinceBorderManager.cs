using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.provinces.border;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.borders;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    public class ProvinceBorderManager
    {
        private const int ParallelMinBorderData = 256;

        private static Value2S _size;
        private static int[,] _pixels;
        public static int ProvinceBorderCount { get; private set; } = 0;

        private static BordersAssembler _bordersAssembler = new BordersAssembler();
        private struct BorderBuildInfo
        {
            public readonly int MinColor;
            public readonly int MaxColor;
            public readonly List<Value2S> Pixels;

            public BorderBuildInfo(int minColor, int maxColor, List<Value2S> pixels)
            {
                MinColor = minColor;
                MaxColor = maxColor;
                Pixels = pixels;
            }
        }

        public static void Init(int[] values, short width, short height)
        {
            foreach (var p in ProvinceManager.GetValues())
                p.ClearBorders();
            ProvinceBorderCount = 0;

            _bordersAssembler.Reset();

            MainForm.ExecuteActions(new (EnumLocKey, Action)[]
            {
                (EnumLocKey.MAP_TAB_PROGRESSBAR_PROVINCES_BORDERS_ASSEMBLE, () => InitPixels(values, width, height)),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_PROVINCES_BORDERS_ASSEMBLE, () => InitBordersPixelsTask()),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_PROVINCES_BORDERS_ASSEMBLE, () => AssembleBorderTask()),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_STATES_BORDERS_ASSEMBLE, () => StateManager.InitBorders()),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => StrategicRegionManager.InitBorders()),
            });
        }

        private static void InitPixels(int[] values, short width, short height)
        {
            _size.x = width;
            _size.y = height;
            _pixels = new int[width, height];

            int pixelCount = width * height;
            int x, y;

            for (int i = 0; i < pixelCount; i++)
            {
                //Вычисляем координаты пикселя
                x = i % width;
                y = i / width;

                _pixels[x, y] = values[i];
            }
        }

        private static void InitBordersPixelsTask()
        {
            int maxX = _size.x - 1;
            int maxY = _size.y - 1;
            var pixels = _pixels;

            //Левый верхний угол
            _bordersAssembler.AcceptBorderPixel(0, 0, pixels[maxX, 0], pixels[0, 0], pixels[0, 0], pixels[maxX, 0]);

            //Левая граница карты
            for (int y = 0; y < maxY; y++)
            {
                int y1 = y + 1;
                _bordersAssembler.AcceptBorderPixel(0, y1, pixels[maxX, y], pixels[0, y], pixels[0, y1], pixels[maxX, y1]);
            }

            //Левый нижний угол
            _bordersAssembler.AcceptBorderPixel(0, _size.y, pixels[maxX, maxY], pixels[0, maxY], pixels[0, maxY], pixels[maxX, maxY]);

            //Верхняя и нижняя граница карты
            for (int x = 0; x < maxX; x++)
            {
                int x1 = x + 1;
                _bordersAssembler.AcceptBorderPixel(x1, 0, pixels[x, 0], pixels[x1, 0], pixels[x1, 0], pixels[x, 0]);
                _bordersAssembler.AcceptBorderPixel(x1, _size.y, pixels[x, maxY], pixels[x1, maxY], pixels[x1, maxY], pixels[x, maxY]);
            }

            long innerCells = (long)maxX * maxY;
            const int parallelMinCells = 262144;

            if (innerCells >= parallelMinCells && Environment.ProcessorCount > 1)
            {
                var locals = new ConcurrentBag<BordersAssembler>();

                Parallel.For(
                    0,
                    maxX,
                    () => new BordersAssembler(),
                    (x, state, local) =>
                    {
                        int x1 = x + 1;
                        for (int y = 0; y < maxY; y++)
                        {
                            int y1 = y + 1;
                            local.AcceptBorderPixel(x1, y1, pixels[x, y], pixels[x1, y], pixels[x1, y1], pixels[x, y1]);
                        }
                        return local;
                    },
                    local => locals.Add(local)
                );

                foreach (var local in locals)
                    _bordersAssembler.MergeFrom(local);
            }
            else
            {
                for (int x = 0; x < maxX; x++)
                {
                    int x1 = x + 1;
                    for (int y = 0; y < maxY; y++)
                    {
                        int y1 = y + 1;
                        _bordersAssembler.AcceptBorderPixel(x1, y1, pixels[x, y], pixels[x1, y], pixels[x1, y1], pixels[x, y1]);
                    }
                }
            }

            _pixels = null;
        }

        private static void AssembleBorderTask()
        {
            short mapWidth = (short)MapManager.MapSize.x;
            var bordersData = _bordersAssembler.BordersData;
            if (bordersData == null || bordersData.Count == 0)
                return;

            int dataCount = GetBorderDataCount(bordersData);
            bool canParallel = dataCount >= ParallelMinBorderData && Environment.ProcessorCount > 1;

            if (canParallel)
            {
                var results = new ConcurrentBag<List<BorderBuildInfo>>();

                Parallel.ForEach(
                    bordersData,
                    () => new List<BorderBuildInfo>(8),
                    (entry, state, local) =>
                    {
                        int minColor = entry.Key;
                        var dataList = entry.Value;

                        for (int i = 0; i < dataList.Count; i++)
                        {
                            var data = dataList[i];
                            var pixelsLists = data.AssembleBorders(mapWidth);
                            dataList[i] = data;

                            if (pixelsLists == null || pixelsLists.Count == 0)
                                continue;

                            int maxColor = data.provinceMaxColor;
                            for (int p = 0; p < pixelsLists.Count; p++)
                                local.Add(new BorderBuildInfo(minColor, maxColor, pixelsLists[p]));
                        }

                        return local;
                    },
                    local =>
                    {
                        if (local.Count > 0)
                            results.Add(local);
                    }
                );

                var provinceCache = new Dictionary<int, Province>(bordersData.Count * 2);
                foreach (var list in results)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var info = list[i];
                        var minProvince = GetProvinceCached(provinceCache, info.MinColor);
                        var maxProvince = GetProvinceCached(provinceCache, info.MaxColor);
                        new ProvinceBorder(ProvinceBorderCount, info.Pixels, minProvince, maxProvince).AddToProvinces();
                        ProvinceBorderCount++;
                    }
                }
            }
            else
            {
                var provinceCache = new Dictionary<int, Province>(bordersData.Count * 2);
                foreach (var entry in bordersData)
                {
                    Province minProvince = GetProvinceCached(provinceCache, entry.Key);
                    var dataList = entry.Value;

                    for (int i = 0; i < dataList.Count; i++)
                    {
                        var data = dataList[i];
                        var pixelsLists = data.AssembleBorders(mapWidth);
                        dataList[i] = data;

                        if (pixelsLists == null || pixelsLists.Count == 0)
                            continue;

                        var maxProvince = GetProvinceCached(provinceCache, data.provinceMaxColor);
                        for (int p = 0; p < pixelsLists.Count; p++)
                        {
                            new ProvinceBorder(ProvinceBorderCount, pixelsLists[p], minProvince, maxProvince).AddToProvinces();
                            ProvinceBorderCount++;
                        }
                    }
                }
            }

            if (ProvinceBorderCount > ushort.MaxValue + 1)
                Logger.LogWarning(
                    EnumLocKey.WARNING_TOO_MUCH_PROVINCE_BORDERS,
                    new Dictionary<string, string> {
                        { "{currentCount}", $"{ProvinceBorderCount}"} ,
                        { "{maxCount}", $"{ushort.MaxValue + 1}"} ,
                    }
                );
        }

        private static int GetBorderDataCount(Dictionary<int, List<BorderData>> bordersData)
        {
            int count = 0;
            foreach (var entry in bordersData)
                count += entry.Value.Count;
            return count;
        }

        private static Province GetProvinceCached(Dictionary<int, Province> cache, int color)
        {
            if (!cache.TryGetValue(color, out var province))
            {
                province = ProvinceManager.Get(color);
                cache[color] = province;
            }
            return province;
        }


    }

}
