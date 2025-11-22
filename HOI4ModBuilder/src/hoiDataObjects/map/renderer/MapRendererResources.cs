using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils.structs;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererResources : IMapRenderer
    {
        private static readonly float scale = 0.125f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter))
                    return MapRendererResult.ABORT;

            var resource = ResourceManager.Get(parameter);
            if (resource == null)
                return MapRendererResult.ABORT;

            StateManager.GetMinMaxResourceCount(resource, out uint min, out uint max);
            var logScaleData = new LogScaleData(min, max);

            func = (p) =>
            {
                if (resource == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);

                uint count = 0;

                var type = p.Type;
                //Проверка на sea провинции
                if (type == EnumProvinceType.SEA)
                {
                    if (p.State == null)
                        return Utils.ArgbToInt(255, 0, 0, 255);
                    else
                        return Utils.ArgbToInt(255, 255, 0, 255);
                }
                else if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 127, 255, 255);
                else if (p.State == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                else if (p.State.Resources.Count == 0 || !p.State.Resources.TryGetValue(resource, out count) || count == 0)
                    return Utils.ArgbToInt(255, 0, 0, 0);

                var value = (byte)logScaleData.Calculate(count, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter)
        {
            var controller = MapManager.FontRenderController;
            controller.TryStart(out var result)?
                .SetEventsHandler(
                    (int)EnumMapRenderEvents.RESOURCES | (int)EnumMapRenderEvents.STATES,
                    (flags, objs) =>
                    {
                        var resource = ResourceManager.Get(parameter);
                        controller.TryStart(controller.EventsFlags, out var eventResult)?
                        .ForEachState(objs, p => true, (fontRegion, s, pos) =>
                        {
                            var resourceCount = s.GetResourceCount(resource);

                            if (resourceCount == 0)
                                controller.PushAction(pos, r => r.RemoveTextMulti(s));
                            else
                                controller.PushAction(pos, r => r.SetTextMulti(
                                    s, TextRenderManager.Instance.FontData64, scale,
                                    resourceCount + "", pos, QFontAlignment.Centre, color, true
                                ));
                        })
                        .EndAssembleParallelWithWait();
                    })
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachState(
                    (s) => s.GetResourceCount(parameter) > 0,
                    (fontRegion, s, pos) => fontRegion.SetTextMulti(
                        s, TextRenderManager.Instance.FontData64, scale,
                        s.GetResourceCount(parameter) + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
