using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerStateWithMultiRegions : MapChecker
    {
        public MapCheckerStateWithMultiRegions()
            : base("MapCheckerStateWithMultiRegions", (int)EnumMapErrorCode.STATE_MULTI_REGIONS, (list) =>
            {
                foreach (var s in StateManager.GetStates())
                {
                    int regionId = -1;
                    foreach (var p in s.Provinces)
                    {
                        if (p.Region == null) continue;
                        if (regionId == -1)
                        {
                            regionId = p.Region.Id;
                            continue;
                        }
                        if (p.Region.Id != regionId)
                        {
                            list.Add(new MapCheckData(s.center, (int)EnumMapErrorCode.STATE_MULTI_REGIONS));
                            break;
                        }
                    }
                }
            })
        { }
    }
}
