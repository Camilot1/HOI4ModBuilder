using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils.structs;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererVictoryPoints : IMapRenderer
    {
        private static readonly float scale = 0.04f;
        private static readonly Color color = Color.Yellow;
        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            TextRenderManager.Instance.ClearAllMulti();
            MapManager.TextScale = scale;

            ProvinceManager.ForEachProvince(p =>
            {
                if (p == null)
                    return;

                if (p.victoryPoints != 0)
                    TextRenderManager.Instance.SetTextMulti(
                    p.Id, TextRenderManager.Instance.FontData, p.victoryPoints + "",
                        p.center.ToVec3(MapManager.MapSize.y), scale, QFontAlignment.Centre, color, true
                    );
            });

            TextRenderManager.Instance.RefreshBuffers();

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
    }
}
