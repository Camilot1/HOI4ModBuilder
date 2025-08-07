using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils.structs;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererProvincesSizes : IMapRenderer
    {
        private static readonly float scale = 0.03f;
        private static readonly Color color = Color.Yellow;
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            ProvinceManager.GetMinMaxMapProvinceSizes(out int minPixelsCount, out int maxPixelsCount);
            var logScaleData = new LogScaleData(minPixelsCount, maxPixelsCount);
            func = (p) =>
            {
                var value = (byte)logScaleData.CalculateInverted(p.pixelsCount, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachProvince(
                    (p) => true,
                    (fontRegion, p, pos) => fontRegion.SetTextMulti(
                        p.Id, TextRenderManager.Instance.FontData64, scale,
                        p.Id + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
