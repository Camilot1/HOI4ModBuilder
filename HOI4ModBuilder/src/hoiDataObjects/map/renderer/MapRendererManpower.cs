using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils.structs;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererManpower : IMapRenderer
    {
        private static readonly float scale = 0.125f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            TextRenderManager.Instance.ClearAllMulti();
            MapManager.TextScale = scale;

            StateManager.ForEachState(s =>
            {
                TextRenderManager.Instance.SetTextMulti(
                s.Id.GetValue(), TextRenderManager.Instance.FontData, s.Manpower.GetValue() + "",
                    s.center.ToVec3(MapManager.MapSize.y), scale, QFontAlignment.Centre, color, true
                );
            });

            TextRenderManager.Instance.RefreshBuffers();

            StateManager.GetMinMaxWeightedManpower(out double manpowerWeightedMin, out double manpowerWeightedMax);
            var logScaleData = new LogScaleData(manpowerWeightedMin, manpowerWeightedMax);

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
                else if (type == EnumProvinceType.LAKE)
                    return Utils.ArgbToInt(255, 127, 255, 255);
                else if (p.State == null)
                    return Utils.ArgbToInt(255, 255, 0, 0);
                else if (p.State.CurrentManpower < 1)
                    return Utils.ArgbToInt(255, 255, 106, 0);

                var valueFactor = p.State.CurrentManpower / (double)p.State.pixelsCount;
                var value = (byte)logScaleData.CalculateInverted(valueFactor, 255d);
                return Utils.ArgbToInt(255, value, value, value);
            };

            return MapRendererResult.CONTINUE;
        }
    }
}
