
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolRemoveGhostProvinces : AbstractAutoTool
    {
        public static void Execute(bool displayResultMessage)
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
                    .Append("Province: ").Append(p.Id)
                    .Append(" (State: " + p.State?.Id.GetValue() ?? "None").Append(")")
                    .Append(": Buildings: ");
                p.ForEachBuilding((building, count) => sb.Append(building.Name).Append('=').Append(count).Append(' '));
                return false;
            }

            // Если есть смежности в провинции, то нельзя удалить
            if (p.GetAdjacenciesCount() > 0)
            {
                sb.Append("\n\t")
                    .Append("Province: ").Append(p.Id)
                    .Append(" (State: " + p.State?.Id.GetValue() ?? "None").Append(")")
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
                        .Append("Province: ").Append(p.Id)
                        .Append(" (State: " + p.State?.Id.GetValue() ?? "None").Append(")")
                        .Append(": Could not found Province ")
                        .Append(provinceIdToReplace)
                        .Append(" for replacement ");
                    return false;
                }

                //Проверки на викторипоинты у провинции
                if (provinceToReplace.victoryPoints != 0)
                {
                    sb.Append("\n\t")
                        .Append("Province: ").Append(p.Id)
                        .Append(" (State: " + p.State?.Id.GetValue() ?? "None").Append(")")
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
