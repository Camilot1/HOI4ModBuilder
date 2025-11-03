using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers.warnings;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerDividedRegions : MapChecker
    {
        public MapCheckerDividedRegions()
            : base("MapCheckerDividedRegions", (int)EnumMapWarningCode.DIVIDED_REGION, (list) =>
            {
                foreach (var r in StrategicRegionManager.GetRegions())
                {
                    if (r.Provinces.Count == 0)
                        continue;

                    var usedProvinces = new HashSet<Province>();
                    var nextProvinces = new Queue<Province>();

                    var startProvince = r.Provinces[0];
                    nextProvinces.Enqueue(startProvince);
                    usedProvinces.Add(startProvince);

                    while (nextProvinces.Count > 0)
                    {
                        var p = nextProvinces.Dequeue();

                        p.ForEachAdjacentProvince((thisP, otherP) =>
                        {
                            if (otherP.Region != r || usedProvinces.Contains(otherP))
                                return;

                            nextProvinces.Enqueue(otherP);
                            usedProvinces.Add(otherP);
                        });
                    }


                    foreach (var p in r.Provinces)
                    {
                        if (!usedProvinces.Contains(p))
                        {
                            list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.DIVIDED_STATE));
                            break;
                        }
                    }
                }
            })
        { }
    }
}

