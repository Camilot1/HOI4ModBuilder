using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.texture;
using HOI4ModBuilder.src.openTK;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererCitiesMap : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            MapManager.MapMainLayer.Texture = TextureManager.cities.texture;

            return MapRendererResult.ABORT;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .ClearAll()
                .End();

            return result;
        }
    }
}
