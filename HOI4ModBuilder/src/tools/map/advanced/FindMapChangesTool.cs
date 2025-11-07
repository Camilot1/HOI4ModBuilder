using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.map.advanced
{
    public class FindMapChangesTool
    {
        public static void Execute()
        {
            if (MapManager.ProvincesPixels == null)
                return;

            string bmpFilePath, definitionFilePath;
            var fd = new OpenFileDialog();
            var dialogPath = FileManager.AssembleFolderPath(
                new[] { Application.StartupPath, "data", "mapChanges" }
            );

            Utils.PrepareFileDialog(
                fd,
                GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_PROVINCES_BMP_FILE),
                dialogPath,
                "BMP files (*.bmp)|*.bmp"
            );

            if (fd.ShowDialog() != DialogResult.OK)
                return;
            bmpFilePath = fd.FileName;

            fd = new OpenFileDialog();
            Utils.PrepareFileDialog(
                fd,
                GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_TITLE_CHOOSE_DEFINITION_CSV_FILE),
                dialogPath,
                "CSV files (*.csv)|*.csv"
            );

            if (fd.ShowDialog() != DialogResult.OK)
                return;
            definitionFilePath = fd.FileName;

            var provinceData = File.ReadAllLines(definitionFilePath);
            var provincesByColor = new Dictionary<int, ushort>(provinceData.Length);

            foreach (string provinceString in provinceData)
            {
                if (provinceString.Trim().Length == 0)
                    continue;

                string[] data = provinceString.Split(';');
                ushort provinceId = ushort.Parse(data[0]);
                byte r = byte.Parse(data[1]);
                byte g = byte.Parse(data[2]);
                byte b = byte.Parse(data[3]);
                provincesByColor[Utils.ArgbToInt(255, r, g, b)] = provinceId;
            }

            using (var provincesBmp = new Bitmap(bmpFilePath))
            {
                int width = provincesBmp.Width;
                int height = provincesBmp.Height;
                int pixelCount = width * height;

                if (provincesBmp.PixelFormat != PixelFormat.Format24bppRgb)
                    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_AUTOTOOL_FIND_MAP_CHANGES_PROVINCES_BMP_HAS_TO_BE_24BPP));
                else if (pixelCount != MapManager.ProvincesPixels.Length)
                    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_AUTOTOOL_FIND_MAP_CHANGES_PROVINCES_BMP_HAS_TO_BE_SAME_SIZE));

                var oldPixels = TextureManager.BrgToArgb(Utils.BitmapToArray(provincesBmp, ImageLockMode.ReadOnly, TextureManager._24bppRgb), 255);

                var resultPixels = new byte[pixelCount * 4];
                int i4;

                for (int i = 0; i < pixelCount; i++)
                {
                    i4 = i * 4;

                    try //TODO Доработать доп. цвета
                    {
                        if ( //Если провинции с новым и старым цветом всё ещё существуют
                            provincesByColor.TryGetValue(oldPixels[i], out var oldProvinceId) &&
                            ProvinceManager.TryGetProvince(oldProvinceId, out var oldProvince) &&
                            ProvinceManager.TryGetProvince(MapManager.ProvincesPixels[i], out var newProvince)
                        )
                        {
                            //Если у старой провинции изменился цвет
                            if (oldProvince.Color != oldPixels[i])
                            { //Ставим фиолетовый цвет
                                resultPixels[i4] = 255;
                                resultPixels[i4 + 1] = 0;
                                resultPixels[i4 + 2] = 255;
                                resultPixels[i4 + 3] = 255;
                            }
                            else if (newProvince.Color != oldPixels[i])
                            {
                                resultPixels[i4] = 0;
                                resultPixels[i4 + 1] = 255;
                                resultPixels[i4 + 2] = 0;
                                resultPixels[i4 + 3] = 255;
                            }
                        }
                        //Если старой или новой провинции с этим цветом нет
                        else
                        { //Ставим фиолетовый цвет
                            resultPixels[i4] = 255;
                            resultPixels[i4 + 1] = 0;
                            resultPixels[i4 + 2] = 255;
                            resultPixels[i4 + 3] = 255;
                        }
                    }
                    catch (Exception _)
                    {
                        resultPixels[i4] = 255;
                        resultPixels[i4 + 1] = 0;
                        resultPixels[i4 + 2] = 255;
                        resultPixels[i4 + 3] = 255;
                    }
                }

                var resultBitmap = new Bitmap(width, height, TextureManager._32bppArgb.imagePixelFormat);
                Utils.ArrayToBitmap(
                    resultPixels, resultBitmap, ImageLockMode.WriteOnly,
                    width, height, TextureManager._32bppArgb
                );
                var directoryPath = $"{Application.StartupPath}\\data\\mapChanges\\output\\";

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var filePath = directoryPath + DateTime.Now.ToString().Replace('.', '-').Replace(':', '-') + ".png";
                resultBitmap.Save(filePath, ImageFormat.Png);

                var text = GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_PNG_SAVED_TEXT,
                    new Dictionary<string, string> { { "{filePath}", filePath } }
                );
                var title = GuiLocManager.GetLoc(EnumLocKey.AUTOTOOLS_FIND_MAP_CHANGES_PNG_SAVED_TITLE);
                MessageBoxUtils.ShowInformation(text, title, MessageBoxButtons.OK);

                Utils.CleanUpMemory();
            }
        }
    }
}
