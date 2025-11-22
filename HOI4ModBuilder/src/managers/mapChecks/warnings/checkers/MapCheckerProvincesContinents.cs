using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvincesContinents : MapChecker
    {
        public MapCheckerProvincesContinents()
            : base("MapCheckerProvincesContinents", (int)EnumMapWarningCode.PROVINCE_CONTINENT_ID_NOT_EXISTS, (list) =>
            {
                foreach (var p in ProvinceManager.GetValues())
                {
                    if (p.ContinentId > ContinentManager.GetContinentsCount())
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_CONTINENT_ID_NOT_EXISTS));
                }
            })
        { }
    }
}
