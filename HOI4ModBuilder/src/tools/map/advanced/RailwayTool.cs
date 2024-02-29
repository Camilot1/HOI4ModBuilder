﻿using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced
{
    class RailwayTool
    {
        public RailwayTool()
        {
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Delete, (sender, e) => RemoveRailway(SupplyManager.SelectedRailway));

            MainForm.SubscribeTabKeyEvent(
                MainForm.Instance.TabPage_Map,
                Keys.R,
                (sender, e) =>
                {
                    if (e.Control && !(e.Alt || e.Shift))
                    {
                        var railway = CreateRailway(
                            MainForm.Instance.SelectedRailwayLevel,
                            ProvinceManager.SelectedProvince,
                            ProvinceManager.RMBProvince
                        );
                        if (railway != null) SupplyManager.SelectedRailway = railway;
                    }
                }
            );

            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.D1, (sender, e) => ChangeRailwayAction(e, 1));
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.D2, (sender, e) => ChangeRailwayAction(e, 2));
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.D3, (sender, e) => ChangeRailwayAction(e, 3));
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.D4, (sender, e) => ChangeRailwayAction(e, 4));
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.D5, (sender, e) => ChangeRailwayAction(e, 5));

            void ChangeRailwayAction(KeyEventArgs e, byte value)
            {
                if (e.Control && !(e.Alt || e.Shift))
                    MainForm.Instance.SelectedRailwayLevel = value;
            }
        }

        public static Railway CreateRailway(byte level, Province start, Province end)
        {
            if (start == null || end == null || start.Id == end.Id ||
                start.Type != EnumProvinceType.LAND || end.Type != EnumProvinceType.LAND ||
                !(start.HasBorderWith(end) || start.HasSeaConnectionWith(end)) ||
                start.HasDirectRailwayConnectionWith(end))
            {
                return null;
            }

            var railway = new Railway(level, start, end);
            AddRailway(railway);
            return railway;
        }

        public static void AddRailway(Railway railway)
        {
            if (railway == null) return;

            MapManager.ActionHistory.Add(
                () => AddRailwayAction(railway),
                () => RemoveRailwayAction(railway)
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
                () => AddProvinceToRailwayAction(railway, province),
                () => RemoveProvinceFromRailwayAction(railway, province)
            );
        }

        public static void RemoveProvinceFromRailway(Railway railway, Province province)
        {
            if (railway == null || !railway.CanRemoveProvince(province)) return;

            MapManager.ActionHistory.Add(
                () => RemoveProvinceFromRailwayAction(railway, province),
                () => AddProvinceToRailwayAction(railway, province)
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
                () => RemoveRailwayAction(railway),
                () => AddRailwayAction(railway)
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
                () => JoinRailwaysAction(firstRailway, railwayContainer),
                () => SplitRailwayAction(firstRailway, joinProvince, railwayContainer)
            );
        }
        public static void SplitRailwayAtProvince(Railway firstRailway, Province province)
        {
            if (firstRailway == null || province == null &&
                !firstRailway.CanSplitAtProvince(province, out _)) return;

            var railwayContainer = new Railway[] { null };

            MapManager.ActionHistory.Add(
                () => SplitRailwayAction(firstRailway, province, railwayContainer),
                () => JoinRailwaysAction(firstRailway, railwayContainer)
            );
        }

        private static void JoinRailwaysAction(Railway firstRailway, Railway[] railwayContainer)
            => firstRailway?.TryJoinWithRailway(railwayContainer);

        private static void SplitRailwayAction(Railway railway, Province province, Railway[] railwayContainer)
            => railway?.TrySplitAtProvince(province, railwayContainer);

        public static void ChangeRailwayLevel(Railway railway, byte newLevel)
        {
            if (railway == null || railway.Level == newLevel) return;
            byte prevLayer = railway.Level;

            MapManager.ActionHistory.Add(
                () => railway.Level = newLevel,
                () => railway.Level = prevLayer
            );
        }
    }
}