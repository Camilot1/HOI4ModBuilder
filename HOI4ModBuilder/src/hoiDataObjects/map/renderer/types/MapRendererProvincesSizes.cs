using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using HOI4ModBuilder.src.utils.structs;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererProvincesSizes : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            ProvinceManager.GetMinMaxMapProvinceSizes(out int minPixelsCount, out int maxPixelsCount);
            var logScaleData = new LogScaleData(minPixelsCount, maxPixelsCount);
            func = (p) =>
            {
                var value = (byte)logScaleData.Calculate(p.pixelsCount, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.ProvinceIds.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );
    }
}
