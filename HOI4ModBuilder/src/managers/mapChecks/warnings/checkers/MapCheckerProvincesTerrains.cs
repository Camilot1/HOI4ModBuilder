using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvincesTerrains : MapChecker
    {
        public MapCheckerProvincesTerrains()
            : base("MapCheckerProvincesTerrains", (int)EnumMapWarningCode.PROVINCE_WITH_NO_TERRAIN, (list) =>
            {
                foreach (var p in ProvinceManager.GetValues())
                {
                    var terrain = p.Terrain;
                    if (terrain == null || terrain.name == "unknown")
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_WITH_NO_TERRAIN));
                }
            })
        { }
    }
}
