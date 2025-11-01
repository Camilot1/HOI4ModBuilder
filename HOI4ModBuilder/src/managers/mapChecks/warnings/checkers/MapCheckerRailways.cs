using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.managers.warnings;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerRailways : MapChecker
    {
        public MapCheckerRailways()
            : base("MapCheckerRailways", -1, (list) =>
            {
                var connections = new HashSet<int>();

                foreach (var railway in SupplyManager.Railways)
                {
                    for (int i = 1; i < railway.ProvincesCount; i++)
                    {
                        var p0 = railway.GetProvince(i - 1);
                        var p1 = railway.GetProvince(i);

                        if (
                            WarningsManager.Instance.CheckFilter((int)EnumMapWarningCode.RAILWAY_OVERLAP_CONNECTION) &&
                            (!(p0.HasBorderWith(p1) || p0.HasSeaConnectionWith(p1)))
                        )
                        {
                            list.Add(new MapCheckData(
                                p0.center.x + (p1.center.x - p0.center.x) / 2f,
                                p0.center.y + (p1.center.y - p0.center.y) / 2f,
                                (int)EnumMapWarningCode.RAILWAY_PROVINCES_CONNECTION
                            ));
                        }

                        ushort id0 = p0.Id;
                        ushort id1 = p1.Id;

                        int connection = id0 < id1 ? ((id0 << 16) | id1) : (id0 | (id1 << 16));

                        if (!connections.Contains(connection)) connections.Add(connection);
                        else if (WarningsManager.Instance.CheckFilter((int)EnumMapWarningCode.RAILWAY_OVERLAP_CONNECTION))
                            list.Add(new MapCheckData(
                                p0.center.x + (p1.center.x - p0.center.x) / 2f,
                                p0.center.y + (p1.center.y - p0.center.y) / 2f,
                                (int)EnumMapWarningCode.RAILWAY_OVERLAP_CONNECTION
                            ));
                    }
                }
            })
        { }
    }
}
