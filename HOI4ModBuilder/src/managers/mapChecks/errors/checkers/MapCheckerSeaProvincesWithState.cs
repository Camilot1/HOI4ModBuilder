
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerSeaProvincesWithState : MapChecker
    {
        public MapCheckerSeaProvincesWithState()
            : base("MapCheckerSeaProvincesWithState", (int)EnumMapErrorCode.PROVINCE_SEA_WITH_STATE, (list) =>
            {
                foreach (var p in ProvinceManager.GetValues())
                {
                    if (p.Type == EnumProvinceType.SEA && p.State != null)
                        list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_SEA_WITH_STATE));
                }
            })
        { }
    }
}
