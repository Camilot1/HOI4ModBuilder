using HOI4ModBuilder.src.managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HOI4ModBuilder.src.openTK.text
{
    class FontManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "fonts" });
        public static Dictionary<string, Font> fonts = new Dictionary<string, Font>(0);

        public static void LoadFonts()
        {
            if (!Directory.Exists(FOLDER_PATH))
                Directory.CreateDirectory(FOLDER_PATH);

            foreach (string fileName in Utils.GetFileNamesWithAllFormats(Directory.GetFiles(FOLDER_PATH), new string[] { "json", "bmp" }))
            {
                try
                {
                    Bitmap bitmap = new Bitmap($"{FOLDER_PATH}{fileName}.bmp");
                    if (bitmap.PixelFormat != TextureManager._24bppRgb.imagePixelFormat)
                    {
                        throw new Exception($"При загрузке атласа шрифта {fileName}.bmp произошла ошибка! Разрешена загрузка только 24bppRgb .bmp атласов шрифтов!");
                    }

                    FontData fontData = JsonConvert.DeserializeObject<FontData>(File.ReadAllText($"{FOLDER_PATH}{fileName}.json"));
                    Texture2D texture = new Texture2D(bitmap, TextureManager._24bppRgb, true);

                    fonts[fileName] = new Font(fileName, fontData, texture);
                }
                catch (Exception ex)
                {
                    throw new Exception($"При загрузке шрифта {fileName} произошла ошибка!", ex);
                }
            }
        }

        public static void Dispose()
        {
            foreach (Font font in fonts.Values) font.Dispose();
        }
    }
}
