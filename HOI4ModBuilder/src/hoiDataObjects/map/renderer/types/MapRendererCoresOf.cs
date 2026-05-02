using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererCoresOf : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            CountryManager.TryGet(parameter, out var targetCoreOfCountry);
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
                else if (p.State == null || targetCoreOfCountry == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);

                bool isOwner = p.State.owner == targetCoreOfCountry;
                bool hasCoresOf = p.State.CurrentCoresOf.Contains(targetCoreOfCountry);
                if (isOwner && hasCoresOf)
                    return Utils.ArgbToInt(255, 0, 255, 0);
                else if (isOwner)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                else if (hasCoresOf)
                    return Utils.ArgbToInt(255, 255, 255, 0);
                else
                    return Utils.ArgbToInt(255, 0, 0, 0);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.StateIds.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
