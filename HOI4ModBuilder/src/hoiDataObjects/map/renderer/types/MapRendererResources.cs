using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using HOI4ModBuilder.src.utils.structs;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererResources : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            var resource = ResourceManager.Get(parameter);
            if (resource == null)
                return MapRendererResult.ABORT;

            StateManager.GetMinMaxResourceCount(resource, out uint min, out uint max);
            var logScaleData = new LogScaleData(min, max);

            func = (p) =>
            {
                if (resource == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);

                uint count = 0;
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
                else if (p.State.Resources.Count == 0 || !p.State.Resources.TryGetValue(resource, out count) || count == 0)
                    return Utils.ArgbToInt(255, 0, 0, 0);

                var value = (byte)logScaleData.Calculate(count, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.Resources.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
