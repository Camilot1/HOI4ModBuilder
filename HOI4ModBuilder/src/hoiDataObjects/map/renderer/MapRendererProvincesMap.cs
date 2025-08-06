using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererProvincesMap : IMapRenderer
    {
        private static readonly float scale = 0.03f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
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

            if (!result)
                return MapRendererResult.ABORT;

            func = null;

            return MapRendererResult.CONTINUE;
        }
    }
}
