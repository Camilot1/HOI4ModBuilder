using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
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
    public class MapRendererStateCategories : MapRendererGPUCompute
    {
        private static readonly float scale = 0.125f;
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
                if (p.State == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);

                var stateCategory = p.State.StateCategory.GetValue();
                if (stateCategory == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);

                return stateCategory.ColorInt;
            };

            return MapRendererResult.CONTINUE;
        }

        public override bool TextRenderRecalculate(string parameter, string parameterValue)
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetEventsHandlerStatesIdsReinit(scale, color, QFontAlignment.Centre)
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachState(
                    p => true,
                    (fontRegion, s, pos) => fontRegion.SetTextMulti(
                        s, TextRenderManager.Instance.FontData64, scale,
                        s.Id.GetValue() + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }

        protected override string GetShaderPath()
        {
            return Path.Combine(
                Application.StartupPath, "data", "shaders", "map", "renderer",
                "map_renderer_state_categories.comp.glsl"
            );
        }

        private static readonly List<BufferInfo> _bufferInfoList = new List<BufferInfo>
        {
            new BufferInfo(MapRendererBuffersManager.PixelsToProvinceIdsKey, MapRendererBuffersBuilder.BuildPixelsToProvinceIds),
            new BufferInfo(MapRendererBuffersManager.ProvinceDataByIdKey, MapRendererBuffersBuilder.BuildProvinceDataById),
            new BufferInfo(MapRendererBuffersManager.StateDataByIdKey, MapRendererBuffersBuilder.BuildStateDataById),
            new BufferInfo(MapRendererBuffersManager.StateCategoryDataByIdKey, MapRendererBuffersBuilder.BuildStateCategoryDataById),
        };

        protected override List<BufferInfo> GetBufferInfoList() => _bufferInfoList;
    }
}
