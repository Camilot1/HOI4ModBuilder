using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.provinces.border;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
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
        private static int _provinceBorderCount = 0;
        private static Dictionary<int, List<ProvinceBorderData>> _bordersData = new Dictionary<int, List<ProvinceBorderData>>();

        public static void Init(int[] values, short width, short height)
        {
            Stopwatch stopwatch = new Stopwatch();
            _provinceBorderCount = 0;
            _bordersData = new Dictionary<int, List<ProvinceBorderData>>();

            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => {
                    stopwatch.Start();
                    InitPixels(values, width, height);
                }),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => InitBordersPixelsTask()),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => {
                    AssembleBorderTask();

                    stopwatch.Stop();
                    pixels = null;
                    Logger.Log($"BordersInitAndAssemble = {stopwatch.ElapsedMilliseconds} ms");
                    Logger.Log($"ProvinceBorderCount = {_provinceBorderCount}");
                }),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_STATES_BORDERS_ASSEMBLE, () => StateManager.InitStatesBorders()),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => StrategicRegionManager.InitRegionsBorders()),
            };

            MainForm.ExecuteActions(actions);
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
            AcceptBorderPixel(0, 0, pixels[maxX, 0], pixels[0, 0], pixels[0, 0], pixels[maxX, 0]);

            //Левая граница карты
            for (int y = 0; y < maxY; y++)
            {
                int y1 = y + 1;
                AcceptBorderPixel(0, y1, pixels[maxX, y], pixels[0, y], pixels[0, y1], pixels[maxX, y1]);
            }

            //Левый нижний угол
            AcceptBorderPixel(0, size.y, pixels[maxX, maxY], pixels[0, maxY], pixels[0, maxY], pixels[maxX, maxY]);

            //Верхняя и нижняя граница карты
            for (int x = 0; x < maxX; x++)
            {
                int x1 = x + 1;
                AcceptBorderPixel(x1, 0, pixels[x, 0], pixels[x1, 0], pixels[x1, 0], pixels[x, 0]);
                AcceptBorderPixel(x1, size.y, pixels[x, maxY], pixels[x1, maxY], pixels[x1, maxY], pixels[x, maxY]);
            }

            for (int x = 0; x < maxX; x++)
            {
                int x1 = x + 1;
                for (int y = 0; y < maxY; y++)
                {
                    int y1 = y + 1;

                    AcceptBorderPixel(x1, y1, pixels[x, y], pixels[x1, y], pixels[x1, y1], pixels[x, y1]);
                }
            }
        }

        private static void AssembleBorderTask()
        {
            foreach (var entry in _bordersData)
            {
                Province minProvince = ProvinceManager.GetProvince(entry.Key);

                foreach (var data in entry.Value)
                {
                    var pixelsLists = data.AssembleBorders();

                    var maxProvince = ProvinceManager.GetProvince(data.provinceMaxColor);

                    //Logger.Log($"{minProvince.Id} ({minProvince.center}) - {maxProvince.Id} ({maxProvince.center}) = {pixelsLists.Count} poses");

                    foreach (var pixelsList in pixelsLists)
                    {
                        new ProvinceBorder(_provinceBorderCount, pixelsList, minProvince, maxProvince).AddToProvinces();
                        _provinceBorderCount++;
                    }
                }
            }
        }

        private static readonly Action<short, short, int, int, int, int, byte>[] actionsTable = new Action<short, short, int, int, int, int, byte>[]
        {
            (x, y, lu, ru, rd, ld, f) => { //0b0000
                //PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0001
                //PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0010
                //PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0011
                //PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0100
                //PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0101
                //PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0110
                //PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b0111
                //PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1000
                PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1001
                PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1010
                PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1011
                PushData(x, y, ld, lu, f);
                //PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1100
                PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1101
                PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                //PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1110
                PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                //PushData(x, y, rd, ld, f);
            },
            (x, y, lu, ru, rd, ld, f) => { //0b1111
                PushData(x, y, ld, lu, f);
                PushData(x, y, lu, ru, f);
                PushData(x, y, ru, rd, f);
                PushData(x, y, rd, ld, f);
            },
        };

        private static void AcceptBorderPixel(int x, int y, int lu, int ru, int rd, int ld)
        {
            //Если не граница провинций, то выходим
            if (lu == ru && ru == rd && rd == ld)
                return;

            List<ProvinceBorderData> dataList = null;

            byte flags = 0;
            int notEqualsCounter = 0;

            if (lu != ld)
            {
                flags |= 0b1000;
                notEqualsCounter++;
            }
            if (lu != ru)
            {
                flags |= 0b0100;
                notEqualsCounter++;
            }
            if (ru != rd)
            {
                flags |= 0b0010;
                notEqualsCounter++;
            }
            if (rd != ld)
            {
                flags |= 0b0001;
                notEqualsCounter++;
            }

            actionsTable[flags]((short)x, (short)y, lu, ru, rd, ld, flags);
        }

        private static void PushData(short x, short y, int colorA, int colorB, byte flags)
        {
            int tempMinColor, tempMaxColor;

            if (colorA < colorB)
            {
                tempMinColor = colorA;
                tempMaxColor = colorB;
            }
            else
            {
                tempMinColor = colorB;
                tempMaxColor = colorA;
            }

            if (!_bordersData.TryGetValue(tempMinColor, out var dataList))
            {
                dataList = new List<ProvinceBorderData>(4);
                _bordersData[tempMinColor] = dataList;
            }

            var pos = new ValueDirectionalPos()
            {
                pos = new Value2S { x = x, y = y },
                flags = flags
            };

            bool hasFound = false;
            foreach (var data in dataList)
            {
                if (data.provinceMinColor == tempMinColor && data.provinceMaxColor == tempMaxColor)
                {
                    data.points.Add(pos);
                    hasFound = true;
                    break;
                }
            }

            if (!hasFound)
                dataList.Add(new ProvinceBorderData(tempMinColor, tempMaxColor).Add(pos));
        }
    }

}
