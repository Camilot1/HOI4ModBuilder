using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.auto
{
    class AutoTools
    {
        private static void PostAction(bool recalculateAllText, int count)
        {
            MapManager.HandleMapMainLayerChange(recalculateAllText, MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);

            MessageBox.Show(
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_MESSAGE_BOX_TEXT,
                    new Dictionary<string, string> { { "{count}", $"{count}" } }
                ),
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_MESSAGE_BOX_TITLE
                ),
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
            );
        }
        private static void PostExtendedAction(bool recalculateAllTextint, int count, int success, string unsuccessInfo)
        {
            MapManager.HandleMapMainLayerChange(recalculateAllTextint, MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);

            MessageBox.Show(
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_EXTENDED_MESSAGE_BOX_TEXT,
                    new Dictionary<string, string> {
                        { "{count}", $"{count}" },
                        { "{success}", $"{success}" },
                        { "{unsuccess}", $"{count - success}" },
                        { "{unsuccessInfo}", unsuccessInfo }
                    }
                ),
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_EXTENDED_MESSAGE_BOX_TITLE
                ),
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
            );
        }

        public static void FixProvincesCoastalType(bool displayResultMessage)
        {
            int counter = 0;
            ProvinceManager.ForEachProvince((p) =>
            {
                var newIsCoastal = p.CheckCoastalType();
                if (p.IsCoastal != newIsCoastal)
                {
                    counter++;
                    p.IsCoastal = newIsCoastal;
                }
            });

            if (displayResultMessage)
                PostAction(false, counter);
        }

        public static void RemoveSeaAndLakesContinents(bool displayResultMessage)
        {
            int counter = 0;
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.Type != EnumProvinceType.LAND && p.ContinentId != 0)
                {
                    counter++;
                    p.ContinentId = 0;
                }
            });

            if (displayResultMessage)
                PostAction(false, counter);
        }

        public static void RemoveSeaProvincesFromStates(bool displayResultMessage)
        {
            int counter = 0;
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.Type == EnumProvinceType.SEA && p.State != null)
                {
                    counter++;
                    p.State.RemoveProvince(p);
                }
            });

            if (displayResultMessage)
                PostAction(false, counter);
        }

        public static void ValidateAllStates(bool displayResultMessage)
        {
            int counter = 0;
            StateManager.ForEachState((s) =>
            {
                s.Validate(out bool hasChanged);
                if (hasChanged)
                    counter++;
            });

            if (displayResultMessage)
                PostAction(true, counter);
        }

        public static void ValidateAllRegions(bool displayResultMessage)
        {
            int counter = 0;
            StrategicRegionManager.ForEachRegion((r) =>
            {
                r.Validate(out bool hasChanged);
                if (hasChanged)
                    counter++;
            });

            if (displayResultMessage)
                PostAction(true, counter);
        }

        public static void RemoveGhostProvinces(bool displayResultMessage)
        {
            ProvinceManager.ProcessProvincesPixels(MapManager.ProvincesPixels, MapManager.MapSize.x, MapManager.MapSize.y);

            List<Province> provinces = new List<Province>();
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.pixelsCount == 0)
                    provinces.Add(p);
            });

            int success = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var p in provinces)
            {
                if (RemoveGhostProvince(p, sb))
                    success++;
            }

            if (sb.Length == 0)
                sb.Append('-');

            if (displayResultMessage)
                PostExtendedAction(true, provinces.Count, success, sb.ToString());
        }

        private static bool RemoveGhostProvince(Province p, StringBuilder sb)
        {
            // Если есть постройки в провинции, то нельзя удалить
            if (p.GetBuildingsCount() > 0)
            {
                sb.Append("\n\t")
                    .Append(p.Id)
                    .Append(" (State: ")
                    .Append(p.State?.Id?.GetValue())
                    .Append(")")
                    .Append(": Buildings: ");
                p.ForEachBuilding((building, count) => sb.Append(building.Name).Append('=').Append(count).Append(' '));
                return false;
            }

            // Если есть смежности в провинции, то нельзя удалить
            if (p.GetAdjacenciesCount() > 0)
            {
                sb.Append("\n\t")
                    .Append(p.Id)
                    .Append(" (State: ")
                    .Append(p.State?.Id?.GetValue())
                    .Append(")")
                    .Append(": Adjacencies: ")
                    .Append(p.GetAdjacenciesCount());
                return false;
            }

            //Ищем подходящую провинцию с наибольшим id и без викторипоинтов
            if (ProvinceManager.ProvincesCount > 1 && ProvinceManager.NextVacantProvinceId > 0)
            {
                ushort provinceIdToReplace = (ushort)(ProvinceManager.NextVacantProvinceId - 1);

                if (!ProvinceManager.TryGetProvince(provinceIdToReplace, out Province provinceToReplace))
                {
                    sb.Append("\n\t")
                        .Append(p.Id)
                        .Append(" (State: ")
                        .Append(p.State?.Id?.GetValue())
                        .Append(")")
                        .Append(": Could not found Province ")
                        .Append(provinceIdToReplace)
                        .Append(" for replacement ");
                    return false;
                }

                //Проверки на викторипоинты у провинции
                if (provinceToReplace.victoryPoints != 0)
                {
                    sb.Append("\n\t")
                        .Append(p.Id)
                        .Append(" (State: ")
                        .Append(p.State?.Id?.GetValue())
                        .Append(")")
                        .Append(": Other Province ")
                        .Append(provinceIdToReplace)
                        .Append(" has victory points: ")
                        .Append(provinceToReplace.victoryPoints);
                    return false;
                }

                //Заменяем связи со смежностями
                p.ForEachAdjacency((adj) => adj.ReplaceProvince(p, provinceToReplace));

                //Удаляем из словарей
                ProvinceManager.RemoveProvinceById(p.Id);
                ProvinceManager.RemoveProvinceByColor(p.Color);

                //Удаление из областей и регионов
                p.State?.RemoveProvince(p);
                p.Region?.RemoveProvince(p);

                //Заменяем id самой старшей провинции
                provinceToReplace.Id = p.Id;
                ProvinceManager.NextVacantProvinceId--;
            }
            else
            {
                //Удаляем из словарей
                ProvinceManager.RemoveProvinceById(p.Id);
                ProvinceManager.RemoveProvinceByColor(p.Color);

                //Удаление из областей и регионов
                p.State?.RemoveProvince(p);
                p.Region?.RemoveProvince(p);
            }

            // Если через провинцию идут дороги, то удаляем
            if (p.GetRailwaysCount() > 0)
            {
                List<Railway> toRemove = new List<Railway>();
                p.ForEachRailway((r) =>
                {
                    if (r.RemoveProvince(p))
                        SupplyManager.NeedToSaveRailways = true;

                    if (r.ProvincesCount < 2)
                        toRemove.Add(r);
                });

                foreach (var r in toRemove)
                    SupplyManager.RemoveRailway(r);
            }

            // Если в провинции есть узел снабжения, то удаляем
            if (p.SupplyNode != null)
                SupplyManager.RemoveSupplyNode(p.SupplyNode);

            return true;
        }
    }
}
