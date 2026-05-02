using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using HOI4ModBuilder.src.utils.structs;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererManpower : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            StateManager.GetMinMaxWeightedManpower(out double manpowerWeightedMin, out double manpowerWeightedMax);
            var logScaleData = new LogScaleData(manpowerWeightedMin, manpowerWeightedMax);

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
                else if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 127, 255, 255);
                else if (p.State == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                else if (p.State.CurrentManpower < 1)
                    return Utils.ArgbToInt(255, 255, 106, 0);

                var valueFactor = p.State.CurrentManpower / (double)p.State.pixelsCount;
                var value = (byte)logScaleData.Calculate(valueFactor, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.Manpower.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
