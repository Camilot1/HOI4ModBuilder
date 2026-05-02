using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererProvincesTypes : MapRendererGPUCompute
    {
        private static readonly float scale = 0.03f;
        private static readonly Color color = Color.Yellow;

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
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetEventsHandlerProvincesIdsReinit(scale, color, QFontAlignment.Centre)
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachProvince(
                    p => true,
                    (fontRegion, p, pos) => fontRegion.SetTextMulti(
                        p, TextRenderManager.Instance.FontData64, scale,
                        p.Id + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }

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
