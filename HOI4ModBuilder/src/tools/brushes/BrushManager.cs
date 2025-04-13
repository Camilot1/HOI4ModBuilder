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

        public static Dictionary<string, Brush>.KeyCollection GetBrushesNames() => _brushes.Keys;
        public static bool TryGetBrush(string brushName, out Brush brush)
            => _brushes.TryGetValue(brushName, out brush);

        public static void Load()
        {
            _brushes = new Dictionary<string, Brush>();

            if (Directory.Exists(_dirPath))
                Directory.CreateDirectory(_dirPath);

            foreach (var brushFileName in Utils.GetFileNamesWithAllFormats(Directory.GetFiles(_dirPath), _fileFormats))
                LoadBrush(brushFileName);
        }

        private static void LoadBrush(string brushFileName)
        {
            var bitmap = new Bitmap(Path.Combine(_dirPath, brushFileName + ".bmp"));
            var brush = new Brush(bitmap);
            _brushes[brushFileName] = brush;
        }
    }
}
