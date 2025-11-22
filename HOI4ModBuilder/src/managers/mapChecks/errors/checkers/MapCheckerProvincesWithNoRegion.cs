using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerProvincesWithNoRegion : MapChecker
    {
        public MapCheckerProvincesWithNoRegion()
            : base("MapCheckerProvincesWithNoRegion", (int)EnumMapErrorCode.PROVINCE_WITH_NO_REGION, (list) =>
            {
                foreach (var p in ProvinceManager.GetValues())
                {
                    if (p.Region == null)
                        list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_WITH_NO_REGION));
                }
            })
        { }
    }
}
