using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.provinces.border;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.borders;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    public class ProvinceBorderManager
    {
        private static Value2S size;
        private static int[,] pixels;
        public static int ProvinceBorderCount { get; private set; } = 0;

        private static BordersAssembler _bordersAssembler = new BordersAssembler();

        public static void Init(int[] values, short width, short height)
        {
            Stopwatch stopwatch = new Stopwatch();

            foreach (var p in ProvinceManager.GetProvinces())
                p.ClearBorders();
            ProvinceBorderCount = 0;

            _bordersAssembler.Reset();

            MainForm.ExecuteActions(new (EnumLocKey, Action)[]
            {
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => {
                    stopwatch.Start();
                    InitPixels(values, width, height);
                }),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => InitBordersPixelsTask()),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => {
                    AssembleBorderTask();

                    stopwatch.Stop();
                    pixels = null;
                    Logger.Log($"BordersInitAndAssemble = {stopwatch.ElapsedMilliseconds} ms");
                    Logger.Log($"ProvinceBorderCount = {ProvinceBorderCount}");
                }),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_STATES_BORDERS_ASSEMBLE, () => StateManager.InitStatesBorders()),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => StrategicRegionManager.InitRegionsBorders()),
            });
        }

        private static void InitPixels(int[] values, short width, short height)
        {
            size.x = width;
            size.y = height;
            pixels = new int[width, height];

            int pixelCount = width * height;
            int x, y;

            for (int i = 0; i < pixelCount; i++)
            {
                //Вычисляем координаты пикселя
                x = i % width;
                y = i / width;

                pixels[x, y] = values[i];
            }
        }

        private static void InitBordersPixelsTask()
        {
            int maxX = size.x - 1;
            int maxY = size.y - 1;

            //Левый верхний угол
            _bordersAssembler.AcceptBorderPixel(0, 0, pixels[maxX, 0], pixels[0, 0], pixels[0, 0], pixels[maxX, 0]);

            //Левая граница карты
            for (int y = 0; y < maxY; y++)
            {
                int y1 = y + 1;
                _bordersAssembler.AcceptBorderPixel(0, y1, pixels[maxX, y], pixels[0, y], pixels[0, y1], pixels[maxX, y1]);
            }

            //Левый нижний угол
            _bordersAssembler.AcceptBorderPixel(0, size.y, pixels[maxX, maxY], pixels[0, maxY], pixels[0, maxY], pixels[maxX, maxY]);

            //Верхняя и нижняя граница карты
            for (int x = 0; x < maxX; x++)
            {
                int x1 = x + 1;
                _bordersAssembler.AcceptBorderPixel(x1, 0, pixels[x, 0], pixels[x1, 0], pixels[x1, 0], pixels[x, 0]);
                _bordersAssembler.AcceptBorderPixel(x1, size.y, pixels[x, maxY], pixels[x1, maxY], pixels[x1, maxY], pixels[x, maxY]);
            }

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

        private static void AssembleBorderTask()
        {
            var mapSize = MapManager.MapSize;
            foreach (var entry in _bordersAssembler.BordersData)
            {
                Province minProvince = ProvinceManager.GetProvince(entry.Key);

                foreach (var data in entry.Value)
                {
                    var pixelsLists = data.AssembleBorders((short)mapSize.x);

                    var maxProvince = ProvinceManager.GetProvince(data.provinceMaxColor);

                    foreach (var pixelsList in pixelsLists)
                    {
                        new ProvinceBorder(ProvinceBorderCount, pixelsList, minProvince, maxProvince).AddToProvinces();
                        ProvinceBorderCount++;
                    }
                }
            }
        }


    }

}
