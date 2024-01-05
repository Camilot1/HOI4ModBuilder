using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text
{
    class FontManager
    {
        public static string dirPath = @"fonts\";
        public static Dictionary<string, Font> fonts = new Dictionary<string, Font>(0);

        public static void LoadFonts()
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            foreach (string fileName in Utils.GetFileNamesWithAllFormats(Directory.GetFiles(dirPath), new string[] { "json", "bmp" }))
            {
                try
                {
                    Bitmap bitmap = new Bitmap($"{dirPath}{fileName}.bmp");
                    if (bitmap.PixelFormat != TextureManager._24bppRgb.imagePixelFormat)
                    {
                        throw new Exception($"При загрузке атласа шрифта {fileName}.bmp произошла ошибка! Разрешена загрузка только 24bppRgb .bmp атласов шрифтов!");
                    }

                    FontData fontData = JsonConvert.DeserializeObject<FontData>(File.ReadAllText($"{dirPath}{fileName}.json"));
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
