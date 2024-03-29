﻿using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced
{
    class MergeProvincesTool
    {
        public static void MergeProvinces(Province main, Province second)
        {
            //Проверки на ошибки
            if (main == null || second == null || main.Id == second.Id)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROVINCE_MERGE_NULL_PROVINCES_OR_SAME_PROVINCES_IDS));

            if (second.buildings.Count > 0)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_BUILDINGS,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            if (second.railways.Count > 0)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_RAILWAYS,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            if (second.adjacencies.Count > 0)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_ADJACENCIES,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            if (second.supplyNode != null)
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_SECOND_PROVINCE_HAS_SUPPLY_HUB,
                        new Dictionary<string, string> { { "{id}", $"{second.Id}" } }
                    ));

            //Ищем подходящую провинцию с наибольшим id и без викторипоинтов
            if (ProvinceManager.ProvincesCount < 2)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROVINCE_MERGE_NO_PROVINCE_FOR_VACANT_ID));
            if (ProvinceManager.NextVacantProvinceId == 0)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROVINCE_MERGE_NEXT_VACANT_PROVINCE_ID_IS_ZERO));

            ushort provinceIdToReplace = (ushort)(ProvinceManager.NextVacantProvinceId - 1);

            if (!ProvinceManager.TryGetProvince(provinceIdToReplace, out Province provinceToReplace))
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_MERGE_PROVINCE_TO_REPLACE_NOT_FOUND,
                        new Dictionary<string, string> { { "{provinceIdToReplace}", $"{provinceIdToReplace}" } }
                    ));

            //Проверки на викторипоинты у провинций
            if (provinceToReplace.victoryPoints != 0 && second.victoryPoints != 0 && MessageBox.Show(
                    GuiLocManager.GetLoc(
                        EnumLocKey.MERGE_PROVINCES_TOOL_SECOND_AND_LAST_PROVINCES_HAVE_VICTORY_POINTS,
                        new Dictionary<string, string> {
                            { "{secondProvinceId}", $"{second.Id}" },
                            { "{provinceIdToReplace}", $"{provinceIdToReplace}" },
                            { "{secondProvinceVictoryPoints}", $"{second.victoryPoints}" },
                            { "{provinceToReplaceVictoryPoints}", $"{provinceToReplace.victoryPoints}" }
                        }
                    ),
                    GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) != DialogResult.Yes)
                return;
            else if (second.victoryPoints != 0 && MessageBox.Show(
                     GuiLocManager.GetLoc(
                        EnumLocKey.MERGE_PROVINCES_TOOL_SECOND_PROVINCE_HAS_VICTORY_POINTS,
                        new Dictionary<string, string> {
                            { "{secondProvinceId}", $"{second.Id}" },
                            { "{provinceIdToReplace}", $"{provinceIdToReplace}" },
                            { "{secondProvinceVictoryPoints}", $"{second.victoryPoints}" }
                        }
                    ),
                    GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) != DialogResult.Yes)
                return;
            else if (provinceToReplace.victoryPoints != 0 && MessageBox.Show(
                    GuiLocManager.GetLoc(
                        EnumLocKey.MERGE_PROVINCES_TOOL_LAST_PROVINCE_HAS_VICTORY_POINTS,
                        new Dictionary<string, string> {
                            { "{secondProvinceId}", $"{second.Id}" },
                            { "{provinceIdToReplace}", $"{provinceIdToReplace}" },
                            { "{provinceToReplaceVictoryPoints}", $"{provinceToReplace.victoryPoints}" }
                        }
                    ),
                    GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) != DialogResult.Yes)
                return;

            //Замена цветов на карте
            int firstColor = main.Color;
            byte[] firstColorBrg = Utils.ArgbToBrg(new int[] { firstColor });

            int secondColor = second.Color;
            var provincesBitmap = TextureManager.provinces.GetBitmap();

            byte[] bytes = Utils.BitmapToArray(provincesBitmap, ImageLockMode.ReadOnly, TextureManager._24bppRgb);
            bool needToSave = false;

            for (int i = 0; i < MapManager.ProvincesPixels.Length; i++)
            {
                if (MapManager.ProvincesPixels[i] == secondColor)
                {
                    int byteIndex = i * 3;
                    bytes[byteIndex] = firstColorBrg[0];
                    bytes[byteIndex + 1] = firstColorBrg[1];
                    bytes[byteIndex + 2] = firstColorBrg[2];

                    MapManager.ProvincesPixels[i] = firstColor;
                    needToSave = true;
                }
            }

            if (needToSave)
            {
                Utils.ArrayToBitmap(bytes, provincesBitmap, ImageLockMode.WriteOnly, provincesBitmap.Width, provincesBitmap.Height, TextureManager._24bppRgb);
                TextureManager.provinces.texture.Update(TextureManager._24bppRgb, 0, 0, MapManager.MapSize.x, MapManager.MapSize.y, bytes);
                TextureManager.provinces.needToSave = true;
            }

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

            //Удаление из областей и регионов
            second.State?.RemoveProvince(second);
            second.Region?.RemoveProvince(second);

            //Заменяем связи со смежностями
            foreach (Adjacency adj in second.adjacencies)
            {
                adj.ReplaceProvince(second, provinceToReplace);
            }

            //Удаляем из словарей
            ProvinceManager.RemoveProvinceById(second.Id);
            ProvinceManager.RemoveProvinceByColor(second.Color);

            //Заменяем id самой старшей провинции
            provinceToReplace.UpdateId(second.Id);
            ProvinceManager.NextVacantProvinceId--;
        }
    }
}
