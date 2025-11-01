using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerSupplyHubHasNoConnections : MapChecker
    {
        public MapCheckerSupplyHubHasNoConnections()
            : base("MapCheckerSupplyHubHasNoConnections", (int)EnumMapWarningCode.SUPPLY_HUB_NO_CONNECTION, (list) =>
            {
                foreach (var node in SupplyManager.SupplyNodes)
                {
                    var p = node.GetProvince();
                    if (p.GetRailwaysCount() != 0)
                        continue;

                    list.Add(new MapCheckData(p.center.x, p.center.y, (int)EnumMapWarningCode.SUPPLY_HUB_NO_CONNECTION));
                }
            })
        { }
    }
}
