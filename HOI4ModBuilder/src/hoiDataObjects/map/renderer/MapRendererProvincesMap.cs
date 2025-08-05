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
            TextRenderManager.Instance.ClearAllMulti();
            MapManager.TextScale = scale;

            ProvinceManager.ForEachProvince(p =>
            {
                if (p == null)
                    return;

                TextRenderManager.Instance.SetTextMulti(
                p.Id, TextRenderManager.Instance.FontData, p.Id + "",
                    p.center.ToVec3(MapManager.MapSize.y), scale, QFontAlignment.Centre, color, true
                );
            });

            TextRenderManager.Instance.RefreshBuffers();

            func = null;

            return MapRendererResult.CONTINUE;
        }
    }
}
