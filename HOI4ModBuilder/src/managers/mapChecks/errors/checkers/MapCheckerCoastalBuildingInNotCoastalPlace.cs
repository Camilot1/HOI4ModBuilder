using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
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
                        if (CheckError(entry.Key))
                            list.Add(new MapCheckData(entry.Key.center, (int)TYPE_PROVINCE));
            }

            if (ErrorManager.Instance.CheckFilter((int)TYPE_STATE))
            {
                foreach (var s in StateManager.GetStates())
                    if (CheckError(s))
                        list.Add(new MapCheckData(s.center, (int)TYPE_STATE));
            }
        }

        private static bool CheckError(Province p)
        {
            if (p == null || p.IsCoastal)
                return false;

            bool flag = false;
            p.ForEachBuilding((building, count) =>
            {
                if (flag || building.IsOnlyCoastal.GetValue() && count > 0)
                    flag = true;
            });

            return flag;
        }

        public static void HandleProvince(Province province)
        {
            if (!ErrorManager.Instance.CheckFilter((int)TYPE_PROVINCE))
                return;

            if (province == null)
                return;

            var code = ErrorManager.Instance.GetErrorInfo(province.center);
            if (CheckError(province))
                code |= (1uL << (int)TYPE_PROVINCE);
            else
                code &= ~(1uL << (int)TYPE_PROVINCE);

            ErrorManager.Instance.SetErrorInfo(province.center, code);
        }

        private static bool CheckError(State s)
        {
            if (s == null || s.isCoastalState())
                return false;

            foreach (var entry in s.stateBuildings)
                if (entry.Value > 0 && entry.Key.IsOnlyCoastal.GetValue())
                    return true;
            return false;
        }

        public static void HandleState(State state)
        {
            if (!ErrorManager.Instance.CheckFilter((int)TYPE_STATE))
                return;

            if (state == null)
                return;

            var code = ErrorManager.Instance.GetErrorInfo(state.center);
            if (CheckError(state))
                code |= (1uL << (int)TYPE_STATE);
            else
                code &= ~(1uL << (int)TYPE_STATE);

            ErrorManager.Instance.SetErrorInfo(state.center, code);
        }
    }
}
