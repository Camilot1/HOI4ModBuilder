using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererCountries : IMapRenderer
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
                        return Utils.ArgbToInt(255, 0, 0, 0);
                }
                else if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 0, 255, 255);
                else if (p.State == null || (p.State.owner == null && p.State.controller == null))
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else if (p.State.controller != null)
                    return p.State.controller.color;
                else
                    return p.State.owner.color;
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.None.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
