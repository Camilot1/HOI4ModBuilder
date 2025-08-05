using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererStrategicRegions : IMapRenderer
    {
        private static readonly float scale = 0.15f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            TextRenderManager.Instance.ClearAllMulti();
            MapManager.TextScale = scale;

            StrategicRegionManager.ForEachRegion(r =>
            {
                TextRenderManager.Instance.SetTextMulti(
                r.Id, TextRenderManager.Instance.FontData, r.Id + "",
                    r.center.ToVec3(MapManager.MapSize.y), scale, QFontAlignment.Centre, color, true
                );
            });

            TextRenderManager.Instance.RefreshBuffers();

            func = (p) =>
            {
                if (p.Region == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else return p.Region.color;
            };

            return MapRendererResult.CONTINUE;
        }
    }
}
