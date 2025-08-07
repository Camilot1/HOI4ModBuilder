using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils.structs;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererVictoryPoints : IMapRenderer
    {
        private static readonly float scale = 0.04f;
        private static readonly Color color = Color.Yellow;
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            ProvinceManager.GetMinMaxVictoryPoints(out uint victoryPointsMin, out uint victoryPointsMax);
            var logScaleData = new LogScaleData(victoryPointsMin, victoryPointsMax);

            func = (p) =>
            {
                var type = p.Type;
                //Проверка на sea провинции
                if (type == EnumProvinceType.SEA)
                {
                    if (p.State == null)
                        return Utils.ArgbToInt(255, 0, 0, 255);
                    else
                        return Utils.ArgbToInt(255, 255, 0, 255);
                }

                byte value = (byte)logScaleData.CalculateInverted(p.victoryPoints, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
        {
            var controller = MapManager.FontRenderController;
            controller.TryStart(out var result)?
                .SetEventsHandler((int)EnumMapRenderEvents.VICTORY_POINTS, (flags, objs) =>
                {
                    controller.TryStart(controller.EventsFlags, out var eventResult)?
                    .ForEachProvince(objs, p => true, (fontRegion, p, pos) =>
                    {
                        if (p.victoryPoints == 0)
                            controller.PushAction(pos, r => r.RemoveTextMulti(p.Id));
                        else
                            controller.PushAction(pos, r => r.SetTextMulti(
                                p.Id, TextRenderManager.Instance.FontData64, scale,
                                p.victoryPoints + "", pos, QFontAlignment.Centre, color, true
                            ));
                    })
                    .EndAssembleParallel();
                })
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachProvince(
                    (p) => p.victoryPoints > 0,
                    (fontRegion, p, pos) => fontRegion.SetTextMulti(
                        p.Id, TextRenderManager.Instance.FontData64, scale,
                        p.victoryPoints + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
