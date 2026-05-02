using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererStategicLocations : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            func = (p) =>
            {
                var type = p.Type;
                if (type == EnumProvinceType.SEA)
                {
                    if (p.State == null)
                        return Utils.ArgbToInt(255, 0, 0, 255);
                    else
                        return Utils.ArgbToInt(255, 255, 0, 255);
                }
                if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 127, 255, 255);
                if (p.State == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                if (p.State.stateStrategicLocations.Count == 0 &&
                    p.State.provinceStrategicLocations.Count == 0)
                {
                    return Utils.ArgbToInt(255, 0, 0, 0);
                }

                if (CheckContains(p, p.State.stateStrategicLocations))
                    return Utils.ArgbToInt(255, 255, 0, 255);
                if (CheckContains(p, p.State.provinceStrategicLocations))
                    return Utils.ArgbToInt(255, 0, 255, 0);
                if (CheckContains2(p.State.stateStrategicLocations))
                    return Utils.ArgbToInt(255, 128, 0, 128);
                return Utils.ArgbToInt(255, 0, 0, 0);
            };

            bool CheckContains(Province p, Dictionary<Province, List<StrategicLocation>> dict)
            {
                if (!dict.TryGetValue(p, out var list))
                    return false;
                if (parameterValue == "")
                    return true;
                foreach (var obj in list)
                    if (obj.Name == parameterValue)
                        return true;
                return false;
            }

            bool CheckContains2(Dictionary<Province, List<StrategicLocation>> dict)
            {
                if (dict.Count == 0)
                    return false;

                if (parameterValue == "")
                    return true;

                foreach (var list in dict.Values)
                    foreach (var obj in list)
                        if (obj.Name == parameterValue)
                            return true;

                return false;
            }

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.StrategicLocations.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
