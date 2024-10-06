using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerLandProvincesWithNoState : MapChecker
    {
        public MapCheckerLandProvincesWithNoState()
            : base((int)EnumMapErrorCode.PROVINCE_LAND_WITH_NO_STATE, (list) =>
            {
                foreach (var p in ProvinceManager.GetProvinces())
                {
                    if (p.Type == EnumProvinceType.LAND && p.State == null)
                        list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_LAND_WITH_NO_STATE));
                }
            })
        { }
    }
}
