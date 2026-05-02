using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using HOI4ModBuilder.src.utils.structs;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererVictoryPoints : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            ProvinceManager.GetMinMaxVictoryPoints(out uint victoryPointsMin, out uint victoryPointsMax);
            var logScaleData = new LogScaleData(victoryPointsMin, victoryPointsMax);

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

                byte value = (byte)logScaleData.Calculate(p.victoryPoints, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.VictoryPoints.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
