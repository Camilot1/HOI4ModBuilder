using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererRegionsTerrains : IMapRenderer
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
                if (p.Region == null || p.Region.Terrain == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else
                    return p.Region.Terrain.color;
            };

            return MapRendererResult.CONTINUE;
        }
    }
}
