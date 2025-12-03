using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects.replaceTags;
using HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations;
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
    public class MapRendererStategicLocations : IMapRenderer
    {
        private static readonly float scale = 0.075f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue)
        {
            if (recalculateAllText)
                if (!TextRenderRecalculate(parameter, parameterValue))
                    return MapRendererResult.ABORT;

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
                if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 127, 255, 255);
                if (p.State == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                if (p.State.stateStrategicLocations.Count == 0 &&
                    p.State.provinceStrategicLocations.Count == 0)
                {
                    return Utils.ArgbToInt(255, 0, 0, 0);
                }

                if (CheckContains(p, p.State.stateStrategicLocations))
                    return Utils.ArgbToInt(255, 255, 0, 255);
                if (CheckContains(p, p.State.provinceStrategicLocations))
                    return Utils.ArgbToInt(255, 0, 255, 0);
                if (CheckContains2(p.State.stateStrategicLocations))
                    return Utils.ArgbToInt(255, 128, 0, 128);
                return Utils.ArgbToInt(255, 0, 0, 0);
            };

            bool CheckContains(Province p, Dictionary<Province, List<StrategicLocation>> dict)
            {
                if (!dict.TryGetValue(p, out var list))
                    return false;
                if (parameterValue == "")
                    return true;
                foreach (var obj in list)
                    if (obj.Name == parameterValue)
                        return true;
                return false;
            }

            bool CheckContains2(Dictionary<Province, List<StrategicLocation>> dict)
            {
                if (dict.Count == 0)
                    return false;

                if (parameterValue == "")
                    return true;

                foreach (var list in dict.Values)
                    foreach (var obj in list)
                        if (obj.Name == parameterValue)
                            return true;

                return false;
            }

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate(string parameter, string parameterValue)
        {
            var controller = MapManager.FontRenderController;
            controller.TryStart(out var result)?
                .SetEventsHandler(
                    (int)EnumMapRenderEvents.STRATEGIC_LOCATIONS | (int)EnumMapRenderEvents.PROVINCES | (int)EnumMapRenderEvents.STATES,
                    (flags, objs) =>
                    {
                        controller.TryStart(controller.EventsFlags, out var eventResult)?
                        .ForEachProvince(objs, p => true, (fontRegion, p, pos) =>
                        {
                            var text = AssembleText(p, parameterValue);

                            if (text == null)
                                controller.PushAction(pos, r => r.RemoveTextMulti(p));
                            else
                                controller.PushAction(pos, r => r.SetTextMulti(
                                    p, TextRenderManager.Instance.FontData64, scale,
                                    text, pos, QFontAlignment.Centre, color, true
                                ));
                        })
                        .EndAssembleParallelWithWait();
                    })
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachProvince(
                    (p) => p.State != null &&
                        (p.State.provinceStrategicLocations.ContainsKey(p) ||
                        p.State.stateStrategicLocations.ContainsKey(p)),
                    (fontRegion, p, pos) =>
                    {
                        var text = AssembleText(p, parameterValue);
                        if (text == null)
                            return;

                        fontRegion.SetTextMulti(
                            p, TextRenderManager.Instance.FontData64, scale,
                            text, pos, QFontAlignment.Centre, color, true
                        );
                    })
                .EndAssembleParallel();

            return result;
        }

        private string AssembleText(Province p, string parameterValue)
        {
            if (p.State == null)
                return null;

            p.State.provinceStrategicLocations.TryGetValue(p, out var listP);
            p.State.stateStrategicLocations.TryGetValue(p, out var listS);

            if (listP == null && listS == null)
                return null;

            string firstName = null;
            int count = 0;

            if (parameterValue == "")
            {
                if (listP != null && listP.Count > 0)
                {
                    firstName = listP[0].Name;
                    count += listP.Count;
                }
                if (listS != null && listS.Count > 0)
                {
                    firstName = listS[0].Name;
                    count += listS.Count;
                }
            }
            else
            {
                if (listP != null)
                {
                    foreach (var obj in listP)
                    {
                        if (obj.Name == parameterValue)
                        {
                            firstName = parameterValue;
                            count += 1;
                            break;
                        }
                    }
                }
                if (listS != null)
                {
                    foreach (var obj in listS)
                    {
                        if (obj.Name == parameterValue)
                        {
                            firstName = parameterValue;
                            count += 1;
                            break;
                        }
                    }
                }
            }

            if (count == 1)
                return firstName;
            else if (count > 1)
                return firstName + " [+" + (count - 1) + "]";
            return null;
        }
    }
}
