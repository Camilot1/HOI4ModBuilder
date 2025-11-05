using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.buildings;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.map.positions
{
    public class BuildingsPositionsData
    {
        private Dictionary<string, List<BuildingPosition>> _namedPositions = new Dictionary<string, List<BuildingPosition>>(0);

        public void Push(BuildingPosition position)
        {
            var name = position.buildingInfo.name;
            if (!_namedPositions.TryGetValue(name, out List<BuildingPosition> positions))
            {
                positions = new List<BuildingPosition>();
                _namedPositions[name] = positions;
            }

            positions.Add(position);
        }

        public void Check(BuildingInfo info, ushort stateId, ushort provinceId)
        {
            if (info.count == 0)
                return;

            Point2F pos;
            Dictionary<Point2F, List<BuildingErrorInfo>> errors;

            StateManager.TryGetState(stateId, out var state);
            ProvinceManager.TryGetProvince(provinceId, out var province);

            if (info.isProvincial)
            {
                errors = MapPositionsManager.provincesErrors;
                pos = province.center;
            }
            else
            {
                if (info.isOnlyCoastal && state != null && !state.IsCoastalStateCached)
                    return;

                pos = state.center;
                errors = MapPositionsManager.statesErrors;
            }

            if (!_namedPositions.TryGetValue(info.name, out var list))
            {
                PushError(errors, pos, new BuildingErrorInfo
                {
                    buildingInfo = info,
                    error = EnumPositionError.NOT_PRESENT,
                    data = 0
                });
                return;
            }

            ushort presentCount = 0;

            foreach (var posInfo in list)
            {
                if (posInfo.buildingInfo == info)
                    presentCount++;
            }

            if (presentCount != info.count)
            {
                PushError(errors, pos, new BuildingErrorInfo
                {
                    buildingInfo = info,
                    error = presentCount < info.count ?
                        EnumPositionError.NOT_ENOUGH :
                        EnumPositionError.TOO_MUCH,
                    data = presentCount
                });
            }

        }

        public static void PushError(Dictionary<Point2F, List<BuildingErrorInfo>> erros, Point2F pos, BuildingErrorInfo errorInfo)
        {
            if (!erros.TryGetValue(pos, out var list))
            {
                list = new List<BuildingErrorInfo>();
                erros[pos] = list;
            }

            list.Add(errorInfo);
        }
    }

    public struct BuildingErrorInfo
    {
        public BuildingInfo buildingInfo;
        public EnumPositionError error;
        public object data;

        public override string ToString()
        {
            return buildingInfo?.ToString() + " " + error.ToString() + " " + data;
        }
    }

    public enum EnumPositionError
    {
        NONE,
        NOT_PRESENT,
        NOT_ENOUGH,
        TOO_MUCH,
    }
}
