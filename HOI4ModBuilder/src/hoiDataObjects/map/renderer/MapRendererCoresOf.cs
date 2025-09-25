using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererCoresOf : IMapRenderer
    {
        private static readonly float scale = 0.125f;
        private static readonly Color color = Color.Yellow;
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter))
                    return MapRendererResult.ABORT;

            CountryManager.TryGetCountry(parameter, out var targetCoreOfCountry);
            func = (p) =>
            {
                var type = p.Type;
                //Проверка на sea провинции
                if (type == EnumProvinceType.SEA)
                {
                    if (p.State == null)
                        return Utils.ArgbToInt(255, 0, 0, 255);
                    else
                        return Utils.ArgbToInt(255, 0, 0, 0);
                }
                else if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 0, 255, 255);
                else if (p.State == null || targetCoreOfCountry == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else if (p.State.CurrentCoresOf.Contains(targetCoreOfCountry))
                    return targetCoreOfCountry.color;
                else return Utils.ArgbToInt(255, 0, 0, 0);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter)
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetEventsHandlerStatesIdsReinit(scale, color, QFontAlignment.Centre)
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachState(
                    (s) => true,
                    (fontRegion, s, pos) => fontRegion.SetTextMulti(
                        s, TextRenderManager.Instance.FontData64, scale,
                        s.Id.GetValue() + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
