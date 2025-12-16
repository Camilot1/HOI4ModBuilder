using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced
{
    public class MergeProvincesTool
    {
        public MergeProvincesTool()
        {
            MainForm.SubscribeTabKeyEvent(
                EnumTabPage.MAP,
                Keys.M,
                (sender, e) =>
                {
                    if (e.Control && !(e.Alt || e.Shift))
                        MergeSelectedProvinces();
                }
            );
        }

        public static void MergeSelectedProvinces()
        {
            if (ProvinceManager.GroupSelectedProvinces.Count < 2)
                return;

            var list = new List<Province>(ProvinceManager.GroupSelectedProvinces);
            list.Sort();
            var minIdProvince = list[0];
            list.RemoveAt(0);

            var colorsToReplace = new Dictionary<int, int>(list.Count);
            foreach (var province in list)
                MergeProvincesInternal(colorsToReplace, minIdProvince, province);
            MapManager.ReplacePixels(colorsToReplace);
        }

        public static void MergeProvinces(Province main, Province second)
        {
            var colorsToReplace = new Dictionary<int, int>(1);
            MergeProvincesInternal(colorsToReplace, main, second);
            MapManager.ReplacePixels(colorsToReplace);
        }
        public static void MergeProvinces(Province main, ICollection<Province> list)
        {
            list.Remove(main);
            var colorsToReplace = new Dictionary<int, int>(list.Count);
            foreach (var province in list)
                MergeProvincesInternal(colorsToReplace, main, province);
            MapManager.ReplacePixels(colorsToReplace);
        }

        private static void MergeProvincesInternal(Dictionary<int, int> colorsToReplace, Province main, Province second)
        {
            //Проверки на ошибки
            if (main == null || second == null || main.Id == second.Id)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROVINCE_MERGE_NULL_PROVINCES_OR_SAME_PROVINCES_IDS));

            if (second.GetBuildingsCount() > 0)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_BUILDINGS,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            if (second.GetRailwaysCount() > 0)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_RAILWAYS,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            if (second.GetAdjacenciesCount() > 0)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_ADJACENCIES,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            if (second.SupplyNode != null)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_SUPPLY_HUB,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            //Ищем подходящую провинцию с наибольшим id и без викторипоинтов
            if (ProvinceManager.ProvincesCount < 2)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROVINCE_MERGE_NO_PROVINCE_FOR_VACANT_ID));
            if (ProvinceManager.NextVacantProvinceId == 0)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROVINCE_MERGE_NEXT_VACANT_PROVINCE_ID_IS_ZERO));

            ushort provinceIdToReplace = ProvinceManager.MaxProvinceId;

            if (!ProvinceManager.TryGet(provinceIdToReplace, out Province provinceToReplace))
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_PROVINCE_TO_REPLACE_NOT_FOUND,
                        new Dictionary<string, string> { { "{provinceIdToReplace}", $"{provinceIdToReplace}" } }
                    ));

            //Проверки на викторипоинты у провинций
            if (provinceToReplace.victoryPoints != 0 && second.victoryPoints != 0 && DialogResult.Yes != MessageBoxUtils.ShowWarningChooseAction(
                    GuiLocManager.GetLoc(
                        EnumLocKey.MERGE_PROVINCES_TOOL_SECOND_AND_LAST_PROVINCES_HAVE_VICTORY_POINTS,
                        new Dictionary<string, string> {
                            { "{secondProvinceId}", $"{second.Id}" },
                            { "{provinceIdToReplace}", $"{provinceIdToReplace}" },
                            { "{secondProvinceVictoryPoints}", $"{second.victoryPoints}" },
                            { "{provinceToReplaceVictoryPoints}", $"{provinceToReplace.victoryPoints}" }
                        }
                    ),
                    MessageBoxButtons.YesNo))
                return;
            else if (second.victoryPoints != 0 && DialogResult.Yes != MessageBoxUtils.ShowWarningChooseAction(
                     GuiLocManager.GetLoc(
                        EnumLocKey.MERGE_PROVINCES_TOOL_SECOND_PROVINCE_HAS_VICTORY_POINTS,
                        new Dictionary<string, string> {
                            { "{secondProvinceId}", $"{second.Id}" },
                            { "{provinceIdToReplace}", $"{provinceIdToReplace}" },
                            { "{secondProvinceVictoryPoints}", $"{second.victoryPoints}" }
                        }
                    ),
                    MessageBoxButtons.YesNo))
                return;
            else if (provinceToReplace.victoryPoints != 0 && DialogResult.Yes != MessageBoxUtils.ShowWarningChooseAction(
                    GuiLocManager.GetLoc(
                        EnumLocKey.MERGE_PROVINCES_TOOL_LAST_PROVINCE_HAS_VICTORY_POINTS,
                        new Dictionary<string, string> {
                            { "{secondProvinceId}", $"{second.Id}" },
                            { "{provinceIdToReplace}", $"{provinceIdToReplace}" },
                            { "{provinceToReplaceVictoryPoints}", $"{provinceToReplace.victoryPoints}" }
                        }
                    ),
                    MessageBoxButtons.YesNo))
                return;

            //Замена цветов на карте
            colorsToReplace.Add(second.Color, main.Color);

            //Замена центров на общий
            float centerX = main.center.x * main.pixelsCount + second.center.x * second.pixelsCount;
            float centerY = main.center.y * main.pixelsCount + second.center.y * second.pixelsCount;
            main.pixelsCount += second.pixelsCount;

            if (main.pixelsCount != 0)
            {
                main.center.x = centerX / main.pixelsCount;
                main.center.y = centerY / main.pixelsCount;
            }

            int centerIndex = (int)main.center.x + (int)main.center.y * MapManager.MapSize.x;
            if (centerIndex < MapManager.ProvincesPixels.Length && MapManager.ProvincesPixels[centerIndex] == main.Color)
            {
                main.dislayCenter = true;
            }
            else main.dislayCenter = false;

            main.State?.CalculateCenter();
            main.Region?.CalculateCenter();

            //Заменяем связи со смежностями
            second.ForEachAdjacency((adj) => adj.ReplaceProvince(second, provinceToReplace));

            //Удаляем из словарей
            ProvinceManager.RemoveProvinceById(second.Id);

            //Отправляем ивент о удалении и обновлении
            MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.PROVINCES, main);
            MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.PROVINCES, second);
            MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.PROVINCES, provinceToReplace);

            //Заменяем id самой старшей провинции
            provinceToReplace.Id = second.Id;
        }
    }
}
