using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced
{
    class RailwayTool
    {

        public static void AddRailway(Railway railway)
        {
            if (railway == null) return;

            MapManager.ActionHistory.Add(
                () => RemoveRailwayAction(railway),
                () => AddRailwayAction(railway)
            );
        }

        private static void AddRailwayAction(Railway railway)
        {
            if (railway == null || !railway.AddToProvinces()) return;
            SupplyManager.Railways.Add(railway);
            SupplyManager.NeedToSaveRailways = true;
        }

        public static void AddProvinceToRailway(Railway railway, Province province)
        {
            if (railway == null || !railway.CanAddProvince(province)) return;

            MapManager.ActionHistory.Add(
                () => RemoveProvinceFromRailwayAction(railway, province),
                () => AddProvinceToRailwayAction(railway, province)
            );
        }

        public static void RemoveProvinceFromRailway(Railway railway, Province province)
        {
            if (railway == null || !railway.CanRemoveProvince(province)) return;

            MapManager.ActionHistory.Add(
                () => AddProvinceToRailwayAction(railway, province),
                () => RemoveProvinceFromRailwayAction(railway, province)
            );
        }

        private static void AddProvinceToRailwayAction(Railway railway, Province province)
        {
            if (railway == null || province == null || !railway.TryAddProvince(province)) return;

        }
        private static void RemoveProvinceFromRailwayAction(Railway railway, Province province)
        {
            if (railway == null || province == null || !railway.TryRemoveProvince(province)) return;

        }

        public static void RemoveRailway(Railway railway)
        {
            if (railway == null) return;

            MapManager.ActionHistory.Add(
                () => AddRailwayAction(railway),
                () => RemoveRailwayAction(railway)
            );
        }

        private static void RemoveRailwayAction(Railway railway)
        {
            if (railway == null || !railway.RemoveFromProvinces()) return;
            if (SupplyManager.SelectedRailway == railway) SupplyManager.SelectedRailway = null;
            SupplyManager.Railways.Remove(railway);
            SupplyManager.NeedToSaveRailways = true;
        }
    }
}
