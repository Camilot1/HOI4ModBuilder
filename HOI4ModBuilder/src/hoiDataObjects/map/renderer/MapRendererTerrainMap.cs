using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererTerrainMap : IMapRenderer
    {
        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            TextRenderManager.Instance.ClearAll();

            MapManager.MapMainLayer.Texture = TextureManager.terrain.texture;

            return MapRendererResult.ABORT;
        }
    }
}
