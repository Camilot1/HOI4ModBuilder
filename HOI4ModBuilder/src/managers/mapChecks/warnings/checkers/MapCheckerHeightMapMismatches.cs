using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerHeightMapMismatches : MapChecker
    {
        public static readonly EnumMapWarningCode TYPE = EnumMapWarningCode.HEIGHTMAP_MISMATCH;
        public MapCheckerHeightMapMismatches()
            : base((int)TYPE, (list) =>
            {
                var settings = SettingsManager.Settings;
                int width = MapManager.MapSize.x;
                int pixelCount = width * MapManager.MapSize.y;
                short waterLevel = (short)(settings.GetWaterHeight() * 10);
                short minLandOffset = (short)(settings.GetMinLandOffset() * 10);
                short maxWaterdOffset = (short)(settings.GetMaxWaterOffset() * 10);

                int color = 0;
                int[] provincesPixels = MapManager.ProvincesPixels;
                byte[] heightPixels = MapManager.HeightsPixels;
                Province p = null;

                for (int i = 0; i < pixelCount; i++)
                {
                    if (color != provincesPixels[i])
                        ProvinceManager.TryGetProvince(color = provincesPixels[i], out p);

                    byte height = heightPixels[i];
                    if (p != null)
                    {
                        var type = p.Type;
                        float x = i % width + 0.5f;
                        float y = i / width + 0.5f;

                        if (CheckWarning(type, height, waterLevel, minLandOffset, maxWaterdOffset))
                            list.Add(new MapCheckData(x, y, (int)EnumMapWarningCode.HEIGHTMAP_MISMATCH));
                    }
                }
            })
        { }

        public static void HandlePixel(int x, int y)
        {
            if (!WarningsManager.Instance.CheckFilter((int)TYPE))
                return;

            var settings = SettingsManager.Settings;
            int width = MapManager.MapSize.x;
            short waterLevel = (short)(settings.GetWaterHeight() * 10);
            short minLandOffset = (short)(settings.GetMinLandOffset() * 10);
            short maxWaterdOffset = (short)(settings.GetMaxWaterOffset() * 10);

            int[] provincesPixels = MapManager.ProvincesPixels;
            byte[] heightPixels = MapManager.HeightsPixels;

            int i = x + y * width;
            byte height = heightPixels[i];

            ProvinceManager.TryGetProvince(provincesPixels[i], out var province);

            if (province != null)
            {
                var type = province.Type;
                var pos = new Point2F(x + 0.5f, y + 0.5f);
                var code = WarningsManager.Instance.GetErrorInfo(pos);
                if (CheckWarning(type, height, waterLevel, minLandOffset, maxWaterdOffset))
                    code |= (1uL << (int)EnumMapWarningCode.HEIGHTMAP_MISMATCH);
                else
                    code &= ~(1uL << (int)EnumMapWarningCode.HEIGHTMAP_MISMATCH);

                WarningsManager.Instance.SetErrorInfo(pos, code);
            }
        }

        public static bool CheckWarning(EnumProvinceType type, short height, short waterLevel, short minLandOffset, short maxWaterOffset)
            => type == EnumProvinceType.LAND && (height < waterLevel + minLandOffset) ||
            type != EnumProvinceType.LAND && (height > waterLevel + maxWaterOffset);
    }
}
