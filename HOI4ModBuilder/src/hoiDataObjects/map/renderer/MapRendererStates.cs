using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererStates : IMapRenderer
    {
        private static readonly float scale = 0.125f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            func = (p) =>
            {
                if (p.State == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else return p.State.Color;
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachState(
                    (p) => true,
                    (fontRegion, s, pos) => fontRegion.SetTextMulti(
                        s.Id.GetValue(), TextRenderManager.Instance.FontData64, scale,
                        s.Id.GetValue() + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
