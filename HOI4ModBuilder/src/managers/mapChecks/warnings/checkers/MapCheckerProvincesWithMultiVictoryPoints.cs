using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.warnings;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvincesWithMultiVictoryPoints : MapChecker
    {
        public MapCheckerProvincesWithMultiVictoryPoints()
            : base("MapCheckerProvincesWithMultiVictoryPoints", (int)EnumMapWarningCode.PROVINCE_MULTI_VICTORY_POINTS, (list) =>
            {
                var usedIds = new HashSet<ushort>(512);
                foreach (var s in StateManager.GetValues())
                {
                    foreach (var p in s.victoryPoints.Keys)
                    {
                        if (usedIds.Contains(p.Id))
                            list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_MULTI_VICTORY_POINTS));
                        else
                            usedIds.Add(p.Id);
                    }
                }
            })
        { }
    }
}
