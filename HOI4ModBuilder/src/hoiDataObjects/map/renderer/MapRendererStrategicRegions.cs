using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererStrategicRegions : IMapRenderer
    {
        private static readonly float scale = 0.15f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            func = (p) =>
            {
                if (p.Region == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else return p.Region.color;
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetEventsHandlerRegionsIdsReinit(scale, color, QFontAlignment.Centre)
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachRegion(
                    (p) => true,
                    (fontRegion, r, pos) => fontRegion.SetTextMulti(
                        r, TextRenderManager.Instance.FontData64, scale,
                        r.Id + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
