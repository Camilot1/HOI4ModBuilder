using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvincesXCrosses : MapChecker
    {
        public MapCheckerProvincesXCrosses()
            : base("MapCheckerProvincesXCrosses", (int)EnumMapWarningCode.PROVINCE_X_CROSS, (list) =>
            {
                int[] pixels = MapManager.ProvincesPixels;
                for (int x = 0; x < MapManager.MapSize.x - 1; x++)
                {
                    for (int y = 0; y < MapManager.MapSize.y - 1; y++)
                    {
                        int lu = (x + y * MapManager.MapSize.x);
                        int ru = lu + 1;
                        int ld = lu + MapManager.MapSize.x;
                        int rd = ld + 1;

                        if (
                            (pixels[lu] != pixels[ru]) && //lu != ru
                            (pixels[lu] != pixels[ld]) && //lu != ld
                            (pixels[ld] != pixels[rd]) && //ld != rd
                            (pixels[rd] != pixels[ru])    //rd != ru
                        )
                        {
                            list.Add(new MapCheckData(x + 1f, y + 1f, (int)EnumMapWarningCode.PROVINCE_X_CROSS));
                        }
                    }
                }
            })
        { }
    }
}
