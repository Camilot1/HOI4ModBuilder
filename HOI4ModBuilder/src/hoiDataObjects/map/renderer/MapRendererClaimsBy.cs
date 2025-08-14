using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererClaimsBy : IMapRenderer
    {
        private static readonly float scale = 0.125f;
        private static readonly Color color = Color.Yellow;
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            CountryManager.TryGetCountry(parameter, out var targetClaimByCountry);
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
                else if (p.State == null || targetClaimByCountry == null)
                    return Utils.ArgbToInt(255, 0, 0, 0);
                else if (p.State.CurrentClaimsBy.Contains(targetClaimByCountry))
                    return targetClaimByCountry.color;
                else return Utils.ArgbToInt(255, 0, 0, 0);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
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
