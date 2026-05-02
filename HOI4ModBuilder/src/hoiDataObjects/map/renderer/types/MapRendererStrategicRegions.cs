using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.openTK.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererStrategicRegions : MapRendererGPUCompute
    {
        public override MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

            if (!IsComputeDisabled() && TryRenderWithCompute())
            {
                func = null;
                customFunc = null;
                return MapRendererResult.GPU_COMPUTED;
            }

            func = p => p.Region == null
                ? Utils.ArgbToInt(255, 0, 0, 0)
                : p.Region.Color;

            return MapRendererResult.CONTINUE;
        }

        public override bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.RegionIds.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );

        protected override string GetShaderPath()
        {
            return Path.Combine(
                Application.StartupPath, "data", "shaders", "map", "renderer",
                "map_renderer_strategic_regions.comp.glsl"
            );
        }

        private static readonly List<BufferInfo> _bufferInfoList = new List<BufferInfo>
        {
            new BufferInfo(MapRendererBuffersManager.PixelsToProvinceIdsKey, MapRendererBuffersBuilder.BuildPixelsToProvinceIds),
            new BufferInfo(MapRendererBuffersManager.ProvinceDataByIdKey, MapRendererBuffersBuilder.BuildProvinceDataById),
            new BufferInfo(MapRendererBuffersManager.StrategicRegionDataByIdKey, MapRendererBuffersBuilder.BuildStrategicRegionDataById),
        };

        protected override List<BufferInfo> GetBufferInfoList() => _bufferInfoList;
    }
}
