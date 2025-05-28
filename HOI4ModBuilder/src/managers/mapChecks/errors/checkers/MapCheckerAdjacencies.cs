using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.utils.structs;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerAdjacencies : MapChecker
    {
        public MapCheckerAdjacencies()
            : base(-1, (list) => CheckAdjacencies(list))
        { }

        private static void CheckAdjacencies(List<MapCheckData> list)
        {
            foreach (var adj in AdjacenciesManager.GetAdjacencies())
            {
                float x = 0, y = 0;

                if (adj.GetLine(out Point2F s, out Point2F e))
                {
                    x = (s.x + e.x) / 2f;
                    y = (s.y + e.y) / 2f;
                }

                if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.ADJACENCY_START_OR_END_PROVINCE_IS_NULL) &&
                    adj.StartProvince == null || adj.EndProvince == null)
                {
                    list.Add(new MapCheckData(x, y, (int)EnumMapErrorCode.ADJACENCY_START_OR_END_PROVINCE_IS_NULL));
                    return;
                }

                if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.ADJACENCY_CONNECTS_DIFFERENT_TYPES_PROVINCES) &&
                    adj.StartProvince.Type != adj.EndProvince.Type)
                    list.Add(new MapCheckData(x, y, (int)EnumMapErrorCode.ADJACENCY_CONNECTS_DIFFERENT_TYPES_PROVINCES));

                if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.ADJACENCY_ONE_OF_THE_PROVINCES_IS_A_LAKE) &&
                    adj.StartProvince.Type == EnumProvinceType.LAKE)
                    list.Add(new MapCheckData(x, y, (int)EnumMapErrorCode.ADJACENCY_ONE_OF_THE_PROVINCES_IS_A_LAKE));

                if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.ADJACENCY_THAT_CONNECTS_SEA_PROVINCES_MUST_HAVE_TYPE_NONE) &&
                    adj.StartProvince.Type == EnumProvinceType.SEA && adj.EnumType != EnumAdjaciencyType.NONE)
                    list.Add(new MapCheckData(x, y, (int)EnumMapErrorCode.ADJACENCY_THAT_CONNECTS_SEA_PROVINCES_MUST_HAVE_TYPE_NONE));

                if (adj.GetEnumType() == EnumAdjaciencyType.IMPASSABLE)
                    continue;

                if (ErrorManager.Instance.CheckFilter((int)EnumMapErrorCode.ADJACENCY_SEA_CROSS_HAS_NO_RULE_NOR_SEA_PROVINCE) &&
                    adj.AdjacencyRule == null && adj.ThroughProvince == null)
                    list.Add(new MapCheckData(x, y, (int)EnumMapErrorCode.ADJACENCY_SEA_CROSS_HAS_NO_RULE_NOR_SEA_PROVINCE));

            }
        }
    }

}
