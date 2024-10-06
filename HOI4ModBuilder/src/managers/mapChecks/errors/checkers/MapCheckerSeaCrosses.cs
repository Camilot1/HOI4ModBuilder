using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.managers.errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerSeaCrosses : MapChecker
    {
        public MapCheckerSeaCrosses()
            : base((int)EnumMapErrorCode.SEA_CROSS_HAS_NO_RULE_NOR_SEA_PROVINCE, (list) =>
            {
                foreach (var adj in AdjacenciesManager.GetAdjacencies())
                {
                    if (adj.GetEnumType() == EnumAdjaciencyType.IMPASSABLE) continue;

                    if (adj.AdjacencyRule == null && adj.ThroughProvince == null)
                    {
                        float x = 0, y = 0;

                        if (adj.GetLine(out Point2F s, out Point2F e))
                        {
                            x = (s.x + e.x) / 2f;
                            y = (s.y + e.y) / 2f;
                        }

                        list.Add(new MapCheckData(x, y, (int)EnumMapErrorCode.SEA_CROSS_HAS_NO_RULE_NOR_SEA_PROVINCE));
                    }
                }
            })
        { }
    }

}
