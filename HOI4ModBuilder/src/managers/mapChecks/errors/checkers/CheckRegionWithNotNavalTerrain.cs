using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class CheckRegionWithNotNavalTerrain : MapChecker
    {
        public CheckRegionWithNotNavalTerrain()
            : base("CheckRegionWithNotNavalTerrain", (int)EnumMapErrorCode.REGION_USES_NOT_NAVAL_TERRAIN, (list) =>
            {
                foreach (var r in StrategicRegionManager.GetRegions())
                {
                    if (r.Terrain == null) continue;
                    if (!r.Terrain.isNavalTerrain)
                        list.Add(new MapCheckData(r.center, (int)EnumMapErrorCode.REGION_USES_NOT_NAVAL_TERRAIN));
                }
            })
        { }
    }
}
