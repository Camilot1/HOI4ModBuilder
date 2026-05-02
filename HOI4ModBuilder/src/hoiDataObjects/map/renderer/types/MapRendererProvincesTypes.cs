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
    public class MapRendererProvincesTypes : MapRendererGPUCompute
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

            func = p =>
            {
                var type = p.Type;
                bool isCoastal = p.IsCoastal;
                if (type == EnumProvinceType.LAND)
                    return isCoastal ? Utils.ArgbToInt(255, 127, 127, 0) : Utils.ArgbToInt(255, 0, 127, 0);
                if (type == EnumProvinceType.SEA)
                    return isCoastal ? Utils.ArgbToInt(255, 127, 0, 127) : Utils.ArgbToInt(255, 0, 0, 127);
                if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 127, 255, 255);
                return Utils.ArgbToInt(255, 0, 0, 0);
            };

            return MapRendererResult.CONTINUE;
        }

        public override bool TextRenderRecalculate(string parameter, string parameterValue)
            => MapTextLayerDefinitions.ProvinceIds.Rebuild(
                MapManager.FontRenderController,
                new TextLayerContext(parameter, parameterValue)
            );

        protected override string GetShaderPath()
        {
            return Path.Combine(
                Application.StartupPath, "data", "shaders", "map", "renderer",
                "map_renderer_provinces_types.comp.glsl"
            );
        }

        private static readonly List<BufferInfo> _bufferInfoList = new List<BufferInfo>
        {
            new BufferInfo(MapRendererBuffersManager.PixelsToProvinceIdsKey, MapRendererBuffersBuilder.BuildPixelsToProvinceIds),
            new BufferInfo(MapRendererBuffersManager.ProvinceDataByIdKey, MapRendererBuffersBuilder.BuildProvinceDataById),
        };

        protected override List<BufferInfo> GetBufferInfoList() => _bufferInfoList;
    }
}
