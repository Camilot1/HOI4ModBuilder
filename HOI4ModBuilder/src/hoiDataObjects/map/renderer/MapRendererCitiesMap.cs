using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererCitiesMap : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            MapManager.MapMainLayer.Texture = TextureManager.cities.texture;

            return MapRendererResult.ABORT;
        }

        public bool TextRenderRecalculate()
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .ClearAll()
                .End();

            return result;
        }
    }
}
