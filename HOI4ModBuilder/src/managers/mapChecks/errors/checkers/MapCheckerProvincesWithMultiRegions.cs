using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.managers.errors;
using System;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerProvincesWithMultiRegions : MapChecker
    {
        public MapCheckerProvincesWithMultiRegions()
            : base("MapCheckerProvincesWithMultiRegions", (int)EnumMapErrorCode.PROVINCE_MULTI_REGIONS, (list) =>
            {
                Action<StrategicRegion, Province> action = (r, p) =>
                {
                    if (p.Region != null && p.Region.Id != r.Id)
                        list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_MULTI_REGIONS));
                };

                foreach (var r in StrategicRegionManager.GetValues())
                    r.ForEachProvince(action);
            })
        { }
    }
}
