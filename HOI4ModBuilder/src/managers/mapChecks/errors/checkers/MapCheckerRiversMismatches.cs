using HOI4ModBuilder.src.managers.errors;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerRiversMismatches : MapChecker
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map" });

        public MapCheckerRiversMismatches()
            : base(-1, (list) => CheckRiversMismatches(list))
        { }

        private static void CheckRiversMismatches(List<MapCheckData> list)
        {
            var settings = SettingsManager.Settings;
            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);
            if (!fileInfoPairs.TryGetValue("rivers.bmp", out FileInfo fileInfo)) return;

            var riversBitmap = new Bitmap(fileInfo.filePath);
            byte[] riversPixels = Utils.BitmapToArray(riversBitmap, ImageLockMode.ReadOnly, TextureManager._8bppIndexed);
            int width = riversBitmap.Width;
            int height = riversBitmap.Height;

            riversBitmap.Dispose();

            byte[,] riversMap = new byte[width, height];

            var correctPixels = new HashSet<int>();
            var starts = new Queue<int>();

            byte pixel;
            //Выделяем позиции истоков рек и корректных речных пикселей и подготавливаем карту пикселей
            int x, y;
            int correctColorsCount = TextureManager.riverColorsInts.Length;
            for (int i = 0; i < riversPixels.Length; i++)
            {
                pixel = riversPixels[i];
                x = i % width;
                y = i / width;
                riversMap[x, y] = pixel;

                if (pixel == 0)
                {
                    starts.Enqueue(i);
                    correctPixels.Add(i);
                }
                else if (pixel < correctColorsCount) correctPixels.Add(i);
            }

            if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.RIVER_CROSSING))
            {
                for (int tx = 0; tx < (width - 1); tx++)
                {
                    for (int ty = 0; ty < (height - 1); ty++)
                    {
                        if (
                            riversMap[tx, ty] < correctColorsCount && riversMap[tx + 1, ty + 1] < correctColorsCount && riversMap[tx + 1, ty] >= correctColorsCount && riversMap[tx, ty + 1] >= correctColorsCount ||
                            riversMap[tx, ty] >= correctColorsCount && riversMap[tx + 1, ty + 1] >= correctColorsCount && riversMap[tx + 1, ty] < correctColorsCount && riversMap[tx, ty + 1] < correctColorsCount
                        )
                        {
                            list.Add(new MapCheckData(tx + 1f, ty + 1f, (int)EnumMapErrorCode.RIVER_CROSSING));
                        }
                    }
                }
            }

            var flows = new Queue<int>(); //Входящие и выходящие соединения (индексы = 1, 2)
            //Проверяем цепочки рек на корректность

            //Начинаем с зелёных пикселей
            foreach (int index in starts)
                TraceRiver(list, riversMap, correctPixels, flows, correctColorsCount, width, height, index);

            //Идём по всем доп участкам рек
            while (flows.Count > 0)
                TraceRiver(list, riversMap, correctPixels, flows, correctColorsCount, width, height, flows.Dequeue());

            //Добавляем ошибки на все неиспользованные пиксели
            if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.RIVER_NO_START_POINT))
            {
                foreach (int index in correctPixels)
                    list.Add(new MapCheckData(index % width + 0.5f, index / width + 0.5f, (int)EnumMapErrorCode.RIVER_NO_START_POINT));
            }
        }

        private static void TraceRiver(List<MapCheckData> list, in byte[,] riversMap, in HashSet<int> correctPixels, in Queue<int> flows, int correctColorsCount, int width, int height, int startIndex)
        {
            int x = startIndex % width;
            int y = startIndex / width;

            if (!correctPixels.Contains(startIndex) && ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.RIVER_START_POINT_ERROR))
            {
                list.Add(new MapCheckData(x + 0.5f, y + 0.5f, (int)EnumMapErrorCode.RIVER_START_POINT_ERROR));
                return;
            }
            else correctPixels.Remove(startIndex);

            byte bordersCount = 1;
            while (bordersCount != 0)
            {
                TraceNextRiverPixel(
                    list, riversMap, correctPixels, flows, correctColorsCount, x, y, width, height,
                    out x, out y, out startIndex, out bordersCount, out bool hasFlowInOrOut
                );

                if (hasFlowInOrOut && bordersCount == 0 && ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.RIVER_FLOW_IN_OR_OUT_ERROR))
                    list.Add(new MapCheckData(x + 0.5f, y + 0.5f, (int)EnumMapErrorCode.RIVER_FLOW_IN_OR_OUT_ERROR));

            }

            //Если у истока реки нет продолжения или их несколько
            if (bordersCount > 1 && ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.RIVER_START_POINT_ERROR))
            {
                list.Add(new MapCheckData(x + 0.5f, y + 0.5f, (int)EnumMapErrorCode.RIVER_START_POINT_ERROR));
                return;
            }
        }

        private static void TraceNextRiverPixel(List<MapCheckData> list, in byte[,] riversMap, in HashSet<int> correctPixels, in Queue<int> flows, int correctColorsCount, int x, int y, int width, int height, out int nextX, out int nextY, out int nextIndex, out byte bordersCount, out bool hasFlowInOrOut)
        {
            bordersCount = 0;
            hasFlowInOrOut = false;
            int newX;
            nextX = x;
            nextY = y;
            nextIndex = 0;

            //Убираем текущий пиксель из списка неиспользованных пикселей
            correctPixels.Remove(y * width + x);

            if (GetCorrectRiverPosition(width, height, x, y - 1, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains((y - 1) * width + newX))
            {
                if (riversMap[newX, y - 1] == 1 || riversMap[newX, y - 1] == 2)
                {
                    flows.Enqueue((y - 1) * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y - 1;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
            if (GetCorrectRiverPosition(width, height, x + 1, y, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains(y * width + newX))
            {

                if (riversMap[newX, y] == 1 || riversMap[newX, y] == 2)
                {
                    flows.Enqueue(y * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
            if (bordersCount < 2 && GetCorrectRiverPosition(width, height, x, y + 1, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains((y + 1) * width + newX))
            {
                if (riversMap[newX, y + 1] == 1 || riversMap[newX, y + 1] == 2)
                {
                    flows.Enqueue((y + 1) * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y + 1;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
            if (bordersCount < 2 && GetCorrectRiverPosition(width, height, x - 1, y, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains((y) * width + newX))
            {

                if (riversMap[newX, y] == 1 || riversMap[newX, y] == 2)
                {
                    flows.Enqueue(y * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
        }

        private static bool GetCorrectRiverPosition(int width, int height, int x, int y, out int newX)
        {
            if (x < 0) newX = width + x;
            else if (x >= width) newX = x - width;
            else newX = x;

            if (y < 0 || y >= height) return false;
            else return true;
        }
    }


}
