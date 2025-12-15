using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerDividedProvinces : MapChecker
    {
        public MapCheckerDividedProvinces()
            : base("MapCheckerDividedProvinces", (int)EnumMapWarningCode.PROVINCE_DIVIDED, (list) =>
            {
                int width = MapManager.MapSize.x;
                int height = MapManager.MapSize.y;
                int x, y;

                bool[] usedProvinces = new bool[ProvinceManager.MAX_PROVINCES_COUNT];
                bool[] usedPixels = new bool[width * height];
                int[] values = MapManager.ProvincesPixels;
                int pixelCount = values.Length;

                int pixelColor;
                int? prevPixelColor = null;

                for (int i = 0; i < pixelCount; i++)
                {
                    if (usedPixels[i])
                        continue;

                    UseProvinceRegionPixels(ref usedPixels, i);

                    pixelColor = values[i];

                    if (pixelColor == prevPixelColor)
                        continue;
                    prevPixelColor = pixelColor;

                    if (!ProvinceManager.TryGet(pixelColor, out Province p))
                        continue;

                    ushort id = p.Id;
                    if (usedProvinces[id])
                    {
                        x = i % width;
                        y = i / width;
                        list.Add(new MapCheckData(x + 0.5f, y + 0.5f, (int)EnumMapWarningCode.PROVINCE_DIVIDED));
                    }
                    else usedProvinces[id] = true;
                }
            })
        { }


        private static void UseProvinceRegionPixels(ref bool[] usedPixels, int i)
        {
            var nextPoses = new Queue<int>();
            int width = MapManager.MapSize.x;
            int height = MapManager.MapSize.y;
            int x, y;

            int color = MapManager.ProvincesPixels[i];
            nextPoses.Enqueue(i);

            int[] values = MapManager.ProvincesPixels;

            while (nextPoses.Count > 0)
            {
                i = nextPoses.Dequeue();
                x = i % width;
                y = i / width;

                //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
                if (x < 0 || y < 0 || x >= width || y >= height || usedPixels[i]) continue;

                i = x + y * width;

                if (values[i] == color)
                {
                    usedPixels[i] = true;

                    nextPoses.Enqueue(i - 1);
                    nextPoses.Enqueue(i + width);
                    nextPoses.Enqueue(i + 1);
                    nextPoses.Enqueue(i - width);
                }
            }
        }
    }
}
