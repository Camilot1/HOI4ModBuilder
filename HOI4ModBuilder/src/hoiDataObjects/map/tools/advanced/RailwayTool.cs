using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using System;
using static HOI4ModBuilder.src.hoiDataObjects.map.railways.Railway;

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

        public static void SilentAddRailway(Railway railway) => AddRailwayAction(railway);

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
            if (railway == null || province == null) return;
            railway.TryAddProvince(province);

        }
        private static void RemoveProvinceFromRailwayAction(Railway railway, Province province)
        {
            if (railway == null || province == null) return;
            railway.TryRemoveProvince(province);

        }

        public static void RemoveRailway(Railway railway)
        {
            if (railway == null) return;

            MapManager.ActionHistory.Add(
                () => AddRailwayAction(railway),
                () => RemoveRailwayAction(railway)
            );
        }


        public static void SilentRemoveRailway(Railway railway) => RemoveRailwayAction(railway);

        private static void RemoveRailwayAction(Railway railway)
        {
            if (railway == null || !railway.RemoveFromProvinces()) return;
            if (SupplyManager.SelectedRailway == railway) SupplyManager.SelectedRailway = null;
            else if (SupplyManager.RMBRailway == railway) SupplyManager.RMBRailway = null;

            SupplyManager.Railways.Remove(railway);
            SupplyManager.NeedToSaveRailways = true;
        }

        public static void JoinRailways(Railway firstRailway, Railway secondRailway)
        {
            Province joinProvince = null;

            if (firstRailway == null || secondRailway == null &&
                !firstRailway.CanJoin(secondRailway, out _, out joinProvince, out _)) return;

            var railwayContainer = new Railway[] { secondRailway };

            MapManager.ActionHistory.Add(
                () => SplitRailwayAction(firstRailway, joinProvince, railwayContainer),
                () => JoinRailwaysAction(firstRailway, railwayContainer)
            );
        }
        public static void SplitRailwayAtProvince(Railway firstRailway, Province province)
        {
            if (firstRailway == null || province == null &&
                !firstRailway.CanSplitAtProvince(province, out _)) return;

            var railwayContainer = new Railway[] { null };

            MapManager.ActionHistory.Add(
                () => JoinRailwaysAction(firstRailway, railwayContainer),
                () => SplitRailwayAction(firstRailway, province, railwayContainer)
            );
        }

        private static void JoinRailwaysAction(Railway firstRailway, Railway[] railwayContainer)
            => firstRailway?.TryJoinWithRailway(railwayContainer);

        private static void SplitRailwayAction(Railway railway, Province province, Railway[] railwayContainer)
            => railway?.TrySplitAtProvince(province, railwayContainer);
    }
}
