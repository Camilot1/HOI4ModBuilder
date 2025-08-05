using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererStateCategories : IMapRenderer
    {
        private static readonly float scale = 0.125f;
        private static readonly Color color = Color.Yellow;
        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            TextRenderManager.Instance.ClearAllMulti();
            MapManager.TextScale = scale;

            StateManager.ForEachState(s =>
            {
                TextRenderManager.Instance.SetTextMulti(
                s.Id.GetValue(), TextRenderManager.Instance.FontData, s.Id.GetValue() + "",
                    s.center.ToVec3(MapManager.MapSize.y), scale, QFontAlignment.Centre, color, true
                );
            });

            TextRenderManager.Instance.RefreshBuffers();

            func = (p) =>
            {
                if (p.State == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else if (p.State.StateCategory.GetValue() == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                else
                    return p.State.StateCategory.GetValue().color;
            };

            return MapRendererResult.CONTINUE;
        }
    }
}
