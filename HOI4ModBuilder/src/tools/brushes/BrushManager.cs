using HOI4ModBuilder.src.utils.structs;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HOI4ModBuilder.src.tools.brushes
{
    public class BrushManager
    {
        private static Dictionary<string, Brush> _brushes = new Dictionary<string, Brush>();

        private static readonly string _dirPath = Path.Combine("data", "brushes");
        private static readonly string[] _fileFormats = new string[] { "bmp" };

        public static List<string> GetBrushesNames() => new List<string>() { "1" };

        public static void Load()
        {
            _brushes = new Dictionary<string, Brush>();

            if (Directory.Exists(_dirPath))
                Directory.CreateDirectory(_dirPath);

            foreach (var brushFilePath in Utils.GetFileNamesWithAllFormats(Directory.GetFiles(_dirPath), _fileFormats))
                LoadBrush(brushFilePath);
        }

        private static void LoadBrush(string brushFilePath)
        {
            var bitmap = new Bitmap(Path.Combine(_dirPath, brushFilePath + ".bmp"));

            var dif = new Point2F(-bitmap.Width / 2f, -bitmap.Height / 2f);
            var pixels = new List<Value2I>();

            bool[,] boolPixels = new bool[bitmap.Width + 2, bitmap.Height + 2];
            bool[,] usedPixel = new bool[bitmap.Width + 1, bitmap.Height + 1];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bool pixel = bitmap.GetPixel(x, y).A > 0;
                    boolPixels[x + 1, y + 1] = pixel;

                    if (pixel)
                        pixels.Add(new Value2I() { x = x, y = y });
                }
            }

            for (int x = 0; x < bitmap.Width + 1; x++)
            {
                for (int y = 0; y < bitmap.Height + 1; y++)
                {
                    if (usedPixel[x, y])
                        continue;

                    bool flag = boolPixels[x, y] != boolPixels[x + 1, y] ||
                        boolPixels[x, y + 1] != boolPixels[x + 1, y + 1] ||
                        boolPixels[x + 1, y] != boolPixels[x + 1, y + 1];


                }
            }
        }
    }
}
