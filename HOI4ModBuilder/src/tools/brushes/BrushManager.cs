using HOI4ModBuilder.src.managers.settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HOI4ModBuilder.src.tools.brushes
{
    public class BrushManager
    {
        private static Dictionary<string, Dictionary<string, Brush>> _brushesByLanguages = new Dictionary<string, Dictionary<string, Brush>>();

        private static readonly string _dirPath = Path.Combine("data", "brushes");
        private static readonly string _dirPathCustom = Path.Combine("data", "custom", "brushes");
        private static readonly string[] _fileFormats = new string[] { "bmp" };

        public static Dictionary<string, Brush>.KeyCollection GetBrushesNames(BaseSettings settings)
        {
            if (_brushesByLanguages.TryGetValue(settings.language, out var brushes))
                return brushes.Keys;

            return null;
        }

        public static bool TryGetBrush(BaseSettings settings, string brushName, out Brush brush)
        {
            if (_brushesByLanguages.TryGetValue(settings.language, out var brushes) &&
                brushes.TryGetValue(brushName, out brush))
                return true;

            brush = null;
            return false;
        }

        public static void Load()
        {
            _brushesByLanguages = new Dictionary<string, Dictionary<string, Brush>>();

            foreach (var language in SettingsManager.SUPPORTED_LANGUAGES)
                _brushesByLanguages[language] = new Dictionary<string, Brush>();

            if (!Directory.Exists(_dirPath))
                Directory.CreateDirectory(_dirPath);

            if (!Directory.Exists(_dirPathCustom))
                Directory.CreateDirectory(_dirPathCustom);

            GenerateCircleBrush();

            foreach (var brushDirectoryPath in Directory.GetDirectories(_dirPath))
                LoadBrush(brushDirectoryPath);

            foreach (var brushDirectoryPath in Directory.GetDirectories(_dirPathCustom))
                LoadBrush(brushDirectoryPath);
        }

        private static void GenerateCircleBrush()
        {
            var defaultBrushName = "circle";
            var brushDirectoryPath = Path.Combine(_dirPath, defaultBrushName);

            if (Directory.Exists(brushDirectoryPath))
                Directory.Delete(brushDirectoryPath, true);
            Directory.CreateDirectory(brushDirectoryPath);

            var brushInfo = new BrushInfo(defaultBrushName);
            brushInfo.Localization["ru"] = "Круг";
            brushInfo.Localization["en"] = "Circle";

            for (int i = 1; i <= 100; i++)
            {
                GenerateCircle(i);
            }

            var infoPath = Path.Combine(brushDirectoryPath, "_info.json");
            File.WriteAllText(infoPath, JsonConvert.SerializeObject(brushInfo, Formatting.Indented));

            void GenerateCircle(float diameter)
            {
                int size = (int)Math.Ceiling(diameter);
                var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);

                var rect = new Rectangle(0, 0, size, size);
                var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

                int stride = data.Stride;
                IntPtr scan0 = data.Scan0;

                float cx = diameter / 2f;
                float cy = diameter / 2f;
                float radius = diameter / 2f;
                float radiusSq = radius * radius;

                unsafe
                {
                    byte* ptrBase = (byte*)scan0.ToPointer();
                    const int bytesPerPixel = 4;

                    for (int y = 0; y < size; y++)
                    {
                        float dy = (y + 0.5f) - cy;
                        float dySq = dy * dy;
                        byte* ptrRow = ptrBase + y * stride;

                        for (int x = 0; x < size; x++)
                        {
                            float dx = (x + 0.5f) - cx;
                            if (dx * dx + dySq <= radiusSq)
                            {
                                byte* pixel = ptrRow + x * bytesPerPixel;
                                pixel[0] = 0;     // B
                                pixel[1] = 0;     // G
                                pixel[2] = 0;     // R
                                pixel[3] = 255;   // A
                            }
                        }
                    }
                }

                bmp.UnlockBits(data);

                bmp.Save(Path.Combine(brushDirectoryPath, diameter + ".bmp"));
                bmp.Dispose();
            }
        }

        private static void LoadBrush(string brushDirectoryPath)
        {
            var infoPath = Path.Combine(brushDirectoryPath, "_info.json");

            Utils.GetFileNameAndFormat(brushDirectoryPath, out var brushDefaultName, out var _);

            if (!File.Exists(infoPath))
                File.WriteAllText(infoPath, JsonConvert.SerializeObject(new BrushInfo(brushDefaultName), Formatting.Indented));

            var brushInfo = JsonConvert.DeserializeObject<BrushInfo>(File.ReadAllText(infoPath));
            bool needToResaveFileInfo = false;
            foreach (var language in SettingsManager.SUPPORTED_LANGUAGES)
            {
                if (!brushInfo.Localization.ContainsKey(language))
                {
                    brushInfo.Localization[language] = brushDefaultName;
                    needToResaveFileInfo = true;
                }
            }

            if (needToResaveFileInfo)
                File.WriteAllText(infoPath, JsonConvert.SerializeObject(brushInfo, Formatting.Indented));

            var brush = new Brush(brushInfo);

            foreach (var brushVariant in Utils.GetFileNamesWithAllFormats(Directory.GetFiles(brushDirectoryPath), _fileFormats))
            {
                var bitmap = new Bitmap(Path.Combine(brushDirectoryPath, brushVariant + ".bmp"));

                if (long.TryParse(brushVariant, out var longValue))
                    brush.LoadVariant(longValue, bitmap);
                else if (double.TryParse(brushVariant, out var doubleValue))
                    brush.LoadVariant(doubleValue, bitmap);
                else
                    brush.LoadVariant(brushVariant, bitmap);

            }

            brush.SortedVariantKeys.Sort();

            foreach (var entry in brush.BrushInfo.Localization)
            {
                if (_brushesByLanguages.TryGetValue(entry.Key, out var brushes))
                    brushes[entry.Value] = brush;
            }
        }
    }
}
