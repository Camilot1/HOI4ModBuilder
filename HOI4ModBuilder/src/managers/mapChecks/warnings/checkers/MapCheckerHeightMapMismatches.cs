using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerHeightMapMismatches : MapChecker
    {
        public MapCheckerHeightMapMismatches()
            : base((int)EnumMapWarningCode.HEIGHTMAP_MISMATCH, (list) =>
            {
                var settings = SettingsManager.Settings;
                int width = MapManager.MapSize.x;
                int pixelCount = width * MapManager.MapSize.y;
                byte waterLevel = (byte)(settings.GetWaterHeight() * 10);

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

                        if (type == EnumProvinceType.LAND && height < waterLevel - 1 || type != EnumProvinceType.LAND && height >= waterLevel)
                            list.Add(new MapCheckData(x, y, (int)EnumMapWarningCode.HEIGHTMAP_MISMATCH));
                    }
                }
            })
        { }
    }
}
