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
        private static Value2S _size;
        private static int[,] _pixels;
        public static int ProvinceBorderCount { get; private set; } = 0;

        private static BordersAssembler _bordersAssembler = new BordersAssembler();

        public static void Init(int[] values, short width, short height)
        {
            foreach (var p in ProvinceManager.GetValues())
                p.ClearBorders();
            ProvinceBorderCount = 0;

            _bordersAssembler.Reset();

            MainForm.ExecuteActions(new (EnumLocKey, Action)[]
            {
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => InitPixels(values, width, height)),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => InitBordersPixelsTask()),
                (EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => AssembleBorderTask()),
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

            //Левый верхний угол
            _bordersAssembler.AcceptBorderPixel(0, 0, _pixels[maxX, 0], _pixels[0, 0], _pixels[0, 0], _pixels[maxX, 0]);

            //Левая граница карты
            for (int y = 0; y < maxY; y++)
            {
                int y1 = y + 1;
                _bordersAssembler.AcceptBorderPixel(0, y1, _pixels[maxX, y], _pixels[0, y], _pixels[0, y1], _pixels[maxX, y1]);
            }

            //Левый нижний угол
            _bordersAssembler.AcceptBorderPixel(0, _size.y, _pixels[maxX, maxY], _pixels[0, maxY], _pixels[0, maxY], _pixels[maxX, maxY]);

            //Верхняя и нижняя граница карты
            for (int x = 0; x < maxX; x++)
            {
                int x1 = x + 1;
                _bordersAssembler.AcceptBorderPixel(x1, 0, _pixels[x, 0], _pixels[x1, 0], _pixels[x1, 0], _pixels[x, 0]);
                _bordersAssembler.AcceptBorderPixel(x1, _size.y, _pixels[x, maxY], _pixels[x1, maxY], _pixels[x1, maxY], _pixels[x, maxY]);
            }

            for (int x = 0; x < maxX; x++)
            {
                int x1 = x + 1;
                for (int y = 0; y < maxY; y++)
                {
                    int y1 = y + 1;

                    _bordersAssembler.AcceptBorderPixel(x1, y1, _pixels[x, y], _pixels[x1, y], _pixels[x1, y1], _pixels[x, y1]);
                }
            }

            _pixels = null;
        }

        private static void AssembleBorderTask()
        {
            var mapSize = MapManager.MapSize;
            foreach (var entry in _bordersAssembler.BordersData)
            {
                Province minProvince = ProvinceManager.Get(entry.Key);

                foreach (var data in entry.Value)
                {
                    var pixelsLists = data.AssembleBorders((short)mapSize.x);

                    var maxProvince = ProvinceManager.Get(data.provinceMaxColor);

                    foreach (var pixelsList in pixelsLists)
                    {
                        new ProvinceBorder(ProvinceBorderCount, pixelsList, minProvince, maxProvince).AddToProvinces();
                        ProvinceBorderCount++;
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


    }

}
