using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererHeightMap : IMapRenderer
    {
        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .ClearAll()
                .End();

            if (!result)
                return MapRendererResult.ABORT;

            MapManager.MapMainLayer.Texture = TextureManager.height.texture;

            return MapRendererResult.ABORT;
        }
    }
}
