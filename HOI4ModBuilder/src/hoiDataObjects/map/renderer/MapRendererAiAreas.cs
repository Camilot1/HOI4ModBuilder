using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.openTK;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererAiAreas : IMapRenderer
    {
        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .ClearAll()
                .End();

            if (!result)
                return MapRendererResult.ABORT;

            if (!AiAreaManager.TryGetAiArea(parameter, out AiArea aiArea))
            {
                func = (p) => Utils.ArgbToInt(255, 0, 0, 0);
                return MapRendererResult.CONTINUE;
            }

            func = (p) =>
            {
                bool continentFlag = aiArea.HasContinents && aiArea.HasContinentId(p.ContinentId);
                bool regionFlag = aiArea.HasRegions && aiArea.HasRegion(p.Region);
                return Utils.ArgbToInt(255, 0, continentFlag ? (byte)127 : (byte)0, regionFlag ? (byte)127 : (byte)0);
            };

            return MapRendererResult.CONTINUE;
        }
    }
}
