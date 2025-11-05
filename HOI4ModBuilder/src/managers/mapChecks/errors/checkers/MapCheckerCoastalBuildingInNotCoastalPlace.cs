using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerCoastalBuildingInNotCoastalPlace : MapChecker
    {
        public static readonly EnumMapErrorCode TYPE_PROVINCE = EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_LAND_PROVINCE;
        public static readonly EnumMapErrorCode TYPE_STATE = EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_STATE;
        public MapCheckerCoastalBuildingInNotCoastalPlace()
            : base("MapCheckerCoastalBuildingInNotCoastalPlace", -1, list => Check(list))
        { }

        private static void Check(List<MapCheckData> list)
        {
            if (ErrorManager.Instance.CheckFilter((int)TYPE_PROVINCE))
            {
                foreach (var s in StateManager.GetStates())
                    foreach (var entry in s.provincesBuildings)
                        if (GetErrorBuildings(entry.Key).Count > 0)
                            list.Add(new MapCheckData(entry.Key.center, (int)TYPE_PROVINCE));
            }

            if (ErrorManager.Instance.CheckFilter((int)TYPE_STATE))
            {
                foreach (var s in StateManager.GetStates())
                    if (GetErrorBuildings(s).Count > 0)
                        list.Add(new MapCheckData(s.center, (int)TYPE_STATE));
            }
        }

        public static List<Building> GetErrorBuildings(Province p)
        {
            var list = new List<Building>();
            if (p == null || p.IsCoastal)
                return list;

            p.ForEachBuilding((building, count) =>
            {
                if (building.IsOnlyCoastal.GetValue() && count > 0)
                    list.Add(building);
            });

            return list;
        }

        public static void HandleProvince(Province province)
        {
            if (!ErrorManager.Instance.CheckFilter((int)TYPE_PROVINCE))
                return;

            if (province == null)
                return;

            var code = ErrorManager.Instance.GetErrorInfo(province.center);
            if (GetErrorBuildings(province).Count > 0)
                code |= (1uL << (int)TYPE_PROVINCE);
            else
                code &= ~(1uL << (int)TYPE_PROVINCE);

            ErrorManager.Instance.SetErrorInfo(province.center, code);
        }

        public static List<Building> GetErrorBuildings(State s)
        {
            var list = new List<Building>();
            if (s == null || s.IsCoastalStateCached)
                return list;

            foreach (var entry in s.stateBuildings)
                if (entry.Value > 0 && entry.Key.IsOnlyCoastal.GetValue())
                    list.Add(entry.Key);
            return list;
        }

        public static void HandleState(State state)
        {
            if (!ErrorManager.Instance.CheckFilter((int)TYPE_STATE))
                return;

            if (state == null)
                return;

            var code = ErrorManager.Instance.GetErrorInfo(state.center);
            if (GetErrorBuildings(state).Count > 0)
                code |= (1uL << (int)TYPE_STATE);
            else
                code &= ~(1uL << (int)TYPE_STATE);

            ErrorManager.Instance.SetErrorInfo(state.center, code);
        }
    }
}
