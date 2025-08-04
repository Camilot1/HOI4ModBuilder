using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HOI4ModBuilder
{

    class TextureManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map" });
        private static readonly string PROVINCES_FILE_NAME = "provinces.bmp";
        private static readonly string HEIGHTMAP_FILE_NAME = "heightmap.bmp";
        private static readonly string WORLD_NORMAL_FILE_NAME = "world_normal.bmp";
        private static readonly string RIVERS_FILE_NAME = "rivers.bmp";
        private static readonly string TERRAIN_FILE_NAME = "terrain.bmp";
        private static readonly string TREES_FILE_NAME = "trees.bmp";
        private static readonly string CITIES_FILE_NAME = "cities.bmp";

        public static readonly TextureType _8bppAlpha = TextureTypes.Get(EnumTextureType._8bppAlpha);
        public static readonly TextureType _8bppGrayscale = TextureTypes.Get(EnumTextureType._8bppGrayScale);
        public static readonly TextureType _8bppIndexed = TextureTypes.Get(EnumTextureType._8bppIndexed);
        public static readonly TextureType _24bppRgb = TextureTypes.Get(EnumTextureType._24bppRgb);
        public static readonly TextureType _32bppArgb = TextureTypes.Get(EnumTextureType._32bppArgb);

        public static MapPair provinces, terrain, trees, cities, height, normal, none;
        public static MapPair provincesBorders, statesBorders, regionsBorders;
        public static MapPair rivers;

        public static readonly int[] riverColors = {
                Utils.ArgbToInt(255, 0, 255, 0),
                Utils.ArgbToInt(255, 255, 0, 0),
                Utils.ArgbToInt(255, 255, 252, 0),
                Utils.ArgbToInt(255, 0, 225, 255),
                Utils.ArgbToInt(255, 0, 200, 255),
                Utils.ArgbToInt(255, 0, 150, 255),
                Utils.ArgbToInt(255, 0, 100, 255),
                Utils.ArgbToInt(255, 0, 0, 255),
                Utils.ArgbToInt(255, 0, 0, 225),
                Utils.ArgbToInt(255, 0, 0, 200),
                Utils.ArgbToInt(255, 0, 0, 150),
                Utils.ArgbToInt(255, 0, 0, 100)
            };

        private static HashSet<int> _riverColorsSet = new HashSet<int>();
        private static Dictionary<int, byte> _riverColorsMap = new Dictionary<int, byte>();
        private static readonly int[] _indexesToColors = new int[256];

        private static HashSet<Texture2D> _mapPairTextures = new HashSet<Texture2D>();
        private static HashSet<Texture2D> _textures = new HashSet<Texture2D>();

        public struct MapPair
        {
            public bool needToSave;
            public Texture2D texture;
            private Bitmap _bitmap;
            public Bitmap GetBitmap() => _bitmap;


            public MapPair(bool needToSave, Bitmap bitmap, Texture2D texture)
            {
                this.needToSave = needToSave;
                this._bitmap = bitmap;
                this.texture = texture;
                _mapPairTextures.Add(texture);
            }

            public void SetColor(int x, int y, int color)
            {
                if (GetColor(x, y) == color)
                    return;
                _bitmap.SetPixel(x, y, Color.FromArgb(color));
                needToSave = true;
            }
            public void SetColor(Point2D point, int color)
                => SetColor((int)point.x, (int)point.y, color);

            public void WriteByte(int x, int y, byte value)
            {
                var bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
                var scan0 = bitmapData.Scan0;

                byte prevValue = Marshal.ReadByte(scan0, y * bitmapData.Stride + x);
                if (prevValue == value)
                {
                    _bitmap.UnlockBits(bitmapData);
                    return;
                }
                Marshal.WriteByte(scan0, y * bitmapData.Stride + x, value);
                _bitmap.UnlockBits(bitmapData);
                needToSave = true;
            }
            public void WriteByte(Point2D point, byte value) => WriteByte((int)point.x, (int)point.y, value);

            public byte GetByte(Point2D point) => GetByte((int)point.x, (int)point.y);

            public byte GetByte(int x, int y)
            {
                var bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
                var scan0 = bitmapData.Scan0;

                byte value = Marshal.ReadByte(scan0, y * bitmapData.Stride + x);
                _bitmap.UnlockBits(bitmapData);
                return value;
            }

            public int GetColor(Point2D point) => _bitmap.GetPixel((int)point.x, (int)point.y).ToArgb();
            public int GetColor(int x, int y) => _bitmap.GetPixel(x, y).ToArgb();

            public bool GetIndex(int color, out byte index)
            {
                //TODO оптимизировать (сделать через словарь-палитру)
                Color[] entries = _bitmap.Palette.Entries;
                Color c = Color.FromArgb(color);
                index = 0;
                for (byte i = 0; i < entries.Length; i++)
                {
                    if (entries[i] == c)
                    {
                        index = i;
                        return true;
                    };
                }
                return false;
            }

            public void RGBFill(int[] pixels, HashSet<Value2US> positions, int fillColor)
            {
                if (positions.Count == 0) return;

                if (_bitmap.PixelFormat != _24bppRgb.imagePixelFormat)
                {
                    throw new Exception($"Can't use RGBFill with Bitmap {_bitmap.PixelFormat}");
                }

                int width = _bitmap.Width;
                int height = _bitmap.Height;

                byte[] values = Utils.BitmapToArray(_bitmap, ImageLockMode.ReadOnly, _24bppRgb);

                foreach (var pos in positions)
                {
                    int i = (pos.x + pos.y * width);
                    int byteI = i * 3;
                    pixels[i] = fillColor;
                    values[byteI] = (byte)fillColor;
                    values[byteI + 1] = (byte)(fillColor >> 8);
                    values[byteI + 2] = (byte)(fillColor >> 16);
                }

                Utils.ArrayToBitmap(values, _bitmap, ImageLockMode.WriteOnly, width, height, _24bppRgb);

                texture.Update(_24bppRgb, 0, 0, width, height, values);

                needToSave = true;
            }

            public HashSet<Value2US> NewGetRGBPositions(ushort x, ushort y)
            {
                var poses = new HashSet<Value2US>();
                var nextPoses = new Queue<Value2US>();
                int width = _bitmap.Width;
                int height = _bitmap.Height;

                //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
                if (x >= width || y >= height) return poses;

                var color = _bitmap.GetPixel(x, y);
                var pos = new Value2US(x, y);
                nextPoses.Enqueue(pos);

                byte[] values = Utils.BitmapToArray(_bitmap, ImageLockMode.ReadOnly, _24bppRgb);

                while (nextPoses.Count > 0)
                {
                    pos = nextPoses.Dequeue();

                    //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
                    if (poses.Contains(pos) || pos.x >= width || pos.y >= height) continue;

                    int i = (pos.x + pos.y * width) * 3;

                    if (values[i] == color.B && values[i + 1] == color.G && values[i + 2] == color.R)
                    {
                        poses.Add(pos);

                        nextPoses.Enqueue(new Value2US((ushort)(pos.x - 1), pos.y));
                        nextPoses.Enqueue(new Value2US(pos.x, (ushort)(pos.y + 1)));
                        nextPoses.Enqueue(new Value2US((ushort)(pos.x + 1), pos.y));
                        nextPoses.Enqueue(new Value2US(pos.x, (ushort)(pos.y - 1)));
                    }
                }
                return poses;
            }

        }

        public static void DisposeMapTextures()
        {
            foreach (var mapPairTexture in _mapPairTextures)
                mapPairTexture.Dispose();
            _mapPairTextures.Clear();
        }

        public static void DisposeAllTextures()
        {
            _mapPairTextures.Clear();

            foreach (var texture in new HashSet<Texture2D>(_textures))
                texture.Dispose();
        }

        public static void AddTexture(Texture2D texture) => _textures.Add(texture);
        public static void RemoveTexture(Texture2D texture) => _textures.Remove(texture);
        public static void AddSDFTextBundle(SDFTextBundle sdfTextBundle) => _sdfTextBundles.Add(sdfTextBundle);
        public static void RemoveSDFTextBundle(SDFTextBundle sdfTextBundle) => _sdfTextBundles.Remove(sdfTextBundle);

        public static void LoadTextures(Settings settings)
        {
            DisposeMapTextures();
            LoadMapPairs(settings);
            LoadBorders();
            LoadAdditionalLayers(settings);
        }

        private static void LoadMapPairs(Settings settings)
        {
            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);
            LocalizedAction[] actions =
            {
                new LocalizedAction(
                     EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS,
                    () => {
                        provinces = LoadMapPair(fileInfoPairs, PROVINCES_FILE_NAME, _24bppRgb);
                        MapManager.ProvincesPixels = BrgToArgb(Utils.BitmapToArray(provinces.GetBitmap(), ImageLockMode.ReadOnly, _24bppRgb), 255);
                        terrain = LoadMapPair(fileInfoPairs, TERRAIN_FILE_NAME, _8bppIndexed);
                        trees = LoadMapPair(fileInfoPairs, TREES_FILE_NAME, _8bppIndexed);
                        cities = LoadMapPair(fileInfoPairs, CITIES_FILE_NAME, _8bppIndexed);
                        height = LoadMapPair(fileInfoPairs, HEIGHTMAP_FILE_NAME, _8bppGrayscale);
                        MapManager.HeightsPixels = Utils.BitmapToArray(height.GetBitmap(), ImageLockMode.ReadOnly, _8bppGrayscale);
                        normal = LoadMapPair(fileInfoPairs, WORLD_NORMAL_FILE_NAME, _24bppRgb);

                        var bitmap = new Bitmap(1, 1, _8bppGrayscale.imagePixelFormat);
                        var texture = new Texture2D(bitmap, _8bppGrayscale, false);
                        none = new MapPair(false, bitmap, texture);
                        texture.Update(_8bppGrayscale, 0, 0, 1, 1, new byte[] { 255 });
                    })
            };

            MainForm.ExecuteActions(actions);
        }

        public static int[] BrgToArgb(byte[] data, byte alpha)
        {
            int[] values = new int[data.Length / 3];
            int a = alpha << 24;
            for (int i = 0; i < data.Length; i += 3)
            {
                values[i / 3] = a | (data[i + 2] << 16) | (data[i + 1] << 8) | data[i];
            }
            return values;
        }

        private static MapPair LoadMapPair(Dictionary<string, src.FileInfo> fileInfos, string fileName, TextureType textureType)
        {
            if (!fileInfos.TryGetValue(fileName, out src.FileInfo fileInfo))
            {
                var errorBitmap = new Bitmap(0, 0);
                Logger.LogError(
                    EnumLocKey.ERROR_MAP_FILE_NOT_FOUND,
                    new Dictionary<string, string> { { "{fileName}", fileName } }
                );
                return new MapPair(false, errorBitmap, new Texture2D(errorBitmap, textureType, false));
            }

            Bitmap bitmap;
            using (var stream = new FileStream(fileInfo.filePath, FileMode.Open, FileAccess.Read))
            {
                var tempBitmap = new Bitmap(stream);
                bitmap = tempBitmap.Clone(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), textureType.imagePixelFormat);
                tempBitmap.Dispose();
            }

            Texture2D texture;
            if (textureType == _8bppIndexed)
                texture = new Texture2D(Transfer8bbpIndexedTo24bpp(bitmap), _24bppRgb, false);
            else
                texture = new Texture2D(bitmap, textureType, false);
            return new MapPair(fileInfo.needToSave, bitmap, texture);
        }

        public static void LoadBorders()
        {
            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_BORDERS_TEXTURE_MAP, () => provincesBorders = CreateBordersMap(MapManager.ProvincesPixels, provinces.GetBitmap().Width, provinces.GetBitmap().Height)),
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_BORDERS_TEXTURE_MAP, () => ProvinceManager.ProcessProvincesPixels(MapManager.ProvincesPixels, provinces.GetBitmap().Width, provinces.GetBitmap().Height))
            };

            MainForm.ExecuteActions(actions);
        }

        private static void LoadAdditionalLayers(Settings settings)
        {
            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);
            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_ADDITIONAL_MAP_LAYERS, () => rivers = CreateRiverMap(fileInfoPairs["rivers.bmp"]))
            };

            MainForm.ExecuteActions(actions);
        }

        public static Bitmap Transfer8bbpIndexedTo24bpp(Bitmap inputBitmap)
        {
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            var outputBitmap = new Bitmap(width, height, _24bppRgb.imagePixelFormat);
            var rect = new Rectangle(0, 0, width, height);
            var inputData = inputBitmap.LockBits(rect, ImageLockMode.ReadWrite, _8bppIndexed.imagePixelFormat);
            var outputData = outputBitmap.LockBits(rect, ImageLockMode.ReadWrite, _24bppRgb.imagePixelFormat);

            int pixelCount = inputData.Stride * height;
            Color[] colors = inputBitmap.Palette.Entries;
            Color color;

            unsafe
            {
                byte* inputPtr = (byte*)inputData.Scan0;
                byte* outputPtr = (byte*)outputData.Scan0;
                for (int y = 0; y < height; y++)
                {
                    byte* inputPtrRow = inputPtr + y * inputData.Stride;
                    byte* outputPtrRow = outputPtr + y * outputData.Stride;

                    for (int x = 0; x < width; x++)
                    {
                        int outputIndex = x * _24bppRgb.bytesPerPixel;

                        color = colors[inputPtrRow[x]];

                        outputPtrRow[outputIndex] = color.B;
                        outputPtrRow[outputIndex + 1] = color.G;
                        outputPtrRow[outputIndex + 2] = color.R;
                    }
                }
            }

            inputBitmap.UnlockBits(inputData);
            outputBitmap.UnlockBits(outputData);
            return outputBitmap;
        }

        private static MapPair CreateBordersMap(int[] values, int width, int height)
        {
            int pixelCount = width * height;

            byte[] tempValues = new byte[pixelCount];

            int checkBytesCount = pixelCount - width - 1;

            int color;
            for (int i = 0; i < checkBytesCount; i++)
            {
                color = values[i];

                if (!(color == values[i + 1] && //Сравниваем с пикселем справа
                    color == values[i + width] && //Сравниваем с пикселем снизу
                    color == values[i + width + 1])) //Сравниваем с пикселем справа снизу
                {
                    tempValues[i] = 127; //Если пиксель рядом другого цвета, то делаем непрозрачным пиксель на текстуре границ
                }
            }

            var outputBitmap = new Bitmap(width, height, _8bppAlpha.imagePixelFormat);
            var outData = outputBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, _8bppAlpha.imagePixelFormat
            );

            Marshal.Copy(tempValues, 0, outData.Scan0, tempValues.Length);

            return new MapPair(false, outputBitmap, new Texture2D(outputBitmap, outData, _8bppAlpha, false));
        }

        public static void InitRiverPallete()
        {
            _riverColorsSet.Clear();
            _riverColorsMap.Clear();

            MainForm.Instance.InvokeAction(() => MainForm.Instance.FlowLayoutPanel_Color.Controls.Clear());

            for (byte i = 0; i < riverColors.Length; i++)
            {
                _riverColorsSet.Add(riverColors[i]);
                _riverColorsMap[riverColors[i]] = i;
                var panel = new Panel
                {
                    BackColor = Color.FromArgb(riverColors[i]),
                    BorderStyle = BorderStyle.FixedSingle,
                    Size = new Size(21, 21),
                    Margin = new Padding(1, 0, 1, 0),
                    Padding = new Padding(1, 0, 1, 0)
                };
                panel.MouseDown += new MouseEventHandler(PalleteColorMouseDown);
                MainForm.Instance.InvokeAction(() => MainForm.Instance.FlowLayoutPanel_Color.Controls.Add(panel));
            }

        }

        private static void PalleteColorMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MainForm.SetBrushColor(0, ((Panel)sender).BackColor);
            else if (e.Button == MouseButtons.Right)
                MainForm.SetBrushColor(1, ((Panel)sender).BackColor);
        }

        private static void LoadRiverMap(Bitmap inputBitmap, out Bitmap outputBitmap, out BitmapData outputData)
        {
            InitRiverPallete();
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            int pixelCount = width * height;
            var palette = inputBitmap.Palette;

            for (int i = 0; i < palette.Entries.Length; i++)
            {
                int paletteColor = palette.Entries[i].ToArgb();
                if (_riverColorsSet.Contains(paletteColor)) _indexesToColors[i] = paletteColor;
                else _indexesToColors[i] = 0;
            }

            byte[] values = Utils.BitmapToArray(inputBitmap, ImageLockMode.ReadOnly, _8bppGrayscale);
            byte[] newValues = new byte[pixelCount * _32bppArgb.bytesPerPixel];

            int tempIndex;
            for (int i = 0; i < pixelCount; i++)
            {
                Utils.IntToArgb(_indexesToColors[values[i]], out byte a, out byte r, out byte g, out byte b);
                tempIndex = i * 4;
                newValues[tempIndex] = b;
                newValues[tempIndex + 1] = g;
                newValues[tempIndex + 2] = r;
                newValues[tempIndex + 3] = a;
            }

            outputBitmap = new Bitmap(width, height);
            outputData = outputBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, _32bppArgb.imagePixelFormat
            );

            Marshal.Copy(newValues, 0, outputData.Scan0, newValues.Length);
        }

        public static void SaveAllMaps(Settings settings)
        {
            LocalizedAction[] actions =
            {
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_TEXTURE_MAPS, () => {
                    SaveProvincesMap();
                    SaveRiversMap(settings);
                    SaveTerrainMap();
                    SaveTreesMap();
                    SaveCitiesMap();
                    SaveHeightMap(settings);
                })
            };

            MainForm.ExecuteActions(actions);
        }

        public static void SaveProvincesMap()
        {
            if (provinces.needToSave)
                SaveProvincesMap(provinces.GetBitmap());
        }

        public static void SaveTerrainMap()
        {
            if (terrain.needToSave)
                terrain.GetBitmap().Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + TERRAIN_FILE_NAME, ImageFormat.Bmp);
        }

        public static void SaveTreesMap()
        {
            if (trees.needToSave)
                trees.GetBitmap().Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + TREES_FILE_NAME, ImageFormat.Bmp);
        }

        public static void SaveCitiesMap()
        {
            if (cities.needToSave)
                cities.GetBitmap().Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + CITIES_FILE_NAME, ImageFormat.Bmp);
        }

        public static void SaveHeightMap(Settings settings)
        {
            if (height.needToSave)
            {
                SaveHeightMap(height.GetBitmap());

                if (settings.GetGenerateNormalMapFlag())
                {
                    SaveNormalMap(NormalMapGenerator.GenerateNormalMap(
                        height.GetBitmap(),
                        SettingsManager.Settings.GetNormalMapStrength(),
                        SettingsManager.Settings.GetNormalMapBlur()
                    ));
                }
            }
        }

        public static void SaveProvincesMap(Bitmap inputBitmap)
        {
            byte[] values = Utils.BitmapToArray(inputBitmap, ImageLockMode.ReadOnly, _24bppRgb);
            var outputBitmap = new Bitmap(inputBitmap.Width, inputBitmap.Height, PixelFormat.Format24bppRgb);
            Utils.ArrayToBitmap(values, outputBitmap, ImageLockMode.WriteOnly, inputBitmap.Width, inputBitmap.Height, _24bppRgb);
            outputBitmap.Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + PROVINCES_FILE_NAME, ImageFormat.Bmp);
        }

        public static void SaveHeightMap(Bitmap inputBitmap)
        {
            byte[] values = Utils.BitmapToArray(inputBitmap, ImageLockMode.ReadOnly, _8bppGrayscale);
            var outputBitmap = new Bitmap(inputBitmap.Width, inputBitmap.Height, PixelFormat.Format8bppIndexed);

            var palette = outputBitmap.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                byte value = (byte)i;
                palette.Entries[i] = Color.FromArgb(value, value, value);
            }
            outputBitmap.Palette = palette;

            Utils.ArrayToBitmap(values, outputBitmap, ImageLockMode.WriteOnly, inputBitmap.Width, inputBitmap.Height, _8bppGrayscale);
            outputBitmap.Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + HEIGHTMAP_FILE_NAME, ImageFormat.Bmp);
        }
        public static void SaveNormalMap(Bitmap inputBitmap)
        {
            inputBitmap.Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + WORLD_NORMAL_FILE_NAME, ImageFormat.Bmp);
        }

        public static void SaveRiversMap(Settings settings)
        {
            if (rivers.needToSave)
                SaveRiversMap(settings, rivers.GetBitmap());
        }

        public static void SaveRiversMap(Settings settings, Bitmap inputBitmap)
        {
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            int pixelCount = width * height * _32bppArgb.bytesPerPixel;

            byte[] values = Utils.BitmapToArray(inputBitmap, ImageLockMode.ReadOnly, _32bppArgb);
            byte[] outputValues = new byte[pixelCount];

            int color;

            if (settings.GetExportRiversMapWithWaterPixelsFlag())
            {
                for (int i = 0; i < pixelCount; i += _32bppArgb.bytesPerPixel)
                {
                    color = Utils.ArgbToInt(values[i + 3], values[i + 2], values[i + 1], values[i]);
                    int provinceColor = MapManager.ProvincesPixels[i / 4];

                    if (_riverColorsMap.ContainsKey(color))
                        outputValues[i / _32bppArgb.bytesPerPixel] = _riverColorsMap[color];
                    else if (ProvinceManager.TryGetProvince(provinceColor, out Province province) && province.Type != EnumProvinceType.LAND)
                        outputValues[i / _32bppArgb.bytesPerPixel] = 254;
                    else
                        outputValues[i / _32bppArgb.bytesPerPixel] = 255;
                }
            }
            else
            {
                for (int i = 0; i < pixelCount; i += _32bppArgb.bytesPerPixel)
                {
                    color = Utils.ArgbToInt(values[i + 3], values[i + 2], values[i + 1], values[i]);

                    if (_riverColorsMap.ContainsKey(color))
                        outputValues[i / _32bppArgb.bytesPerPixel] = _riverColorsMap[color];
                    else
                        outputValues[i / _32bppArgb.bytesPerPixel] = 255;
                }
            }

            var outputBitmap = new Bitmap(width, height, _8bppIndexed.imagePixelFormat);

            var palette = outputBitmap.Palette;
            for (byte i = 0; i < riverColors.Length; i++) palette.Entries[i] = Color.FromArgb(riverColors[i]);

            Color colorStruct = Color.FromArgb(255, 0, 0, 0);
            for (byte i = (byte)riverColors.Length; i < 254; i++) palette.Entries[i] = colorStruct;

            palette.Entries[254] = Color.FromArgb(255, 122, 122, 122);
            palette.Entries[255] = Color.FromArgb(255, 255, 255, 255);
            outputBitmap.Palette = palette;

            Utils.ArrayToBitmap(outputValues, outputBitmap, ImageLockMode.WriteOnly, width, height, _8bppIndexed);
            outputBitmap.Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + RIVERS_FILE_NAME, ImageFormat.Bmp);
        }

        private static MapPair CreateRiverMap(src.FileInfo fileInfo)
        {
            LoadRiverMap(new Bitmap(fileInfo.filePath), out Bitmap outputBitmap, out BitmapData outputData);
            return new MapPair(fileInfo.needToSave, outputBitmap, new Texture2D(outputBitmap, outputData, _32bppArgb, false));
        }

        public static void LoadSegmentedTextures(string filePath, Settings settings, List<Texture2D> textures, out float imageWidth, out float imageHeight)
        {
            imageWidth = 0;
            imageHeight = 0;
            if (!File.Exists(filePath)) return;

            Utils.GetFileNameAndFormat(filePath, out string fileName, out string fileFormat);


            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                Value2I imageSize;
                //Получаем размер исходного изображения
                using (var image = Image.FromStream(stream, false, false))
                {
                    imageSize.x = image.Width;
                    imageSize.y = image.Height;

                    if (image.PixelFormat != PixelFormat.Format32bppArgb)
                        throw new FormatException(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_TEXTURE_LOADING_INCORRECT_TEXTURE_PIXEL_FORMAT,
                            new Dictionary<string, string>
                            {
                                { "{currentPixelFormat}", image.PixelFormat.ToString() },
                                { "{correctPixelFormat}", PixelFormat.Format32bppArgb.ToString() }
                            }
                        ));
                }

                imageWidth = imageSize.x;
                imageHeight = imageSize.y;

                //Вычисляем параметры разделения на суб-текстуры
                Value2I regionSize;
                regionSize.x = imageSize.x;
                regionSize.y = imageSize.y;

                Value2I regionCount;
                regionCount.x = 1;
                regionCount.y = 1;

                Value2I additionalLastPixels;
                additionalLastPixels.x = 0;
                additionalLastPixels.y = 0;

                byte alpha = settings.textureOpacity;
                int maxTextureSize = settings.maxAdditionalTextureSize;

                if (imageSize.x > maxTextureSize)
                {
                    regionCount.x = (int)Math.Ceiling(imageSize.x / (double)maxTextureSize);
                    regionSize.x = imageSize.x / regionCount.x;
                    additionalLastPixels.x = imageSize.x - regionSize.x * regionCount.x;
                }

                if (imageSize.y > maxTextureSize)
                {
                    regionCount.y = (int)Math.Ceiling(imageSize.y / (double)maxTextureSize);
                    regionSize.y = imageSize.y / regionCount.y;
                    additionalLastPixels.y = imageSize.y - regionSize.y * regionCount.y;
                }

                int partsCount = regionCount.x * regionCount.y;
                MainForm.DisplayProgress(
                    EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ADDITIONAL_TEXTURE_LOADING,
                    new Dictionary<string, string>
                    {
                        {"{fileName}", fileName},
                        {"{progress}", $"(0/{partsCount})"}
                    }, 0
                );

                //Загружаем суб-текстуры
                for (int regionXIndex = 0; regionXIndex < regionCount.x; regionXIndex++)
                {
                    int startX = regionXIndex * regionSize.x;
                    int width = regionSize.x + ((regionXIndex == regionCount.x - 1) ? additionalLastPixels.x : 0);

                    for (int regionYIndex = 0; regionYIndex < regionCount.y; regionYIndex++)
                    {
                        int startY = regionYIndex * regionSize.y;
                        int height = regionSize.y + ((regionYIndex == regionCount.y - 1) ? additionalLastPixels.y : 0);

                        byte[] data = new byte[width * height * 4];

                        for (int y = startY; y < height + startY; y++)
                        {
                            int startIndex = (y * imageSize.x + startX) * 4;
                            stream.Seek(startIndex + 150, SeekOrigin.Begin);
                            stream.Read(data, (y - startY) * width * 4, width * 4);
                        }

                        if (alpha != 255)
                        {
                            for (int i = 3; i < data.Length; i += 4)
                            {
                                data[i] = (byte)(data[i] * (alpha / 255f));
                            }
                        }

                        Value2F sizeFactor;
                        sizeFactor.x = imageWidth / (float)regionSize.x;
                        sizeFactor.y = imageHeight / (float)regionSize.y;

                        Value2F moveFactor;
                        moveFactor.x = -regionXIndex;
                        moveFactor.y = -regionYIndex;

                        float[] coordinates = new float[]
                        {
                            moveFactor.x, moveFactor.y,
                            sizeFactor.x + moveFactor.x, moveFactor.y,
                            sizeFactor.x + moveFactor.x, sizeFactor.y + moveFactor.y,
                            moveFactor.x, sizeFactor.y + moveFactor.y,
                        };

                        int partIndex = regionYIndex + regionXIndex * regionCount.y + 1;

                        MainForm.DisplayProgress(
                            EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ADDITIONAL_TEXTURE_LOADING,
                            new Dictionary<string, string>
                            {
                                {"{fileName}", fileName},
                                {"{progress}", $"({partIndex}/{partsCount})"}
                            },
                            partIndex / (float)partsCount
                        );

                        textures.Add(new Texture2D(_32bppArgb, false, coordinates, width, height, data));
                    }
                }
            }
            MainForm.DisplayProgress(
                EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ADDITIONAL_TEXTURE_LOADED,
                new Dictionary<string, string> { { "{fileName}", fileName } },
                0
            );
        }

        public static Texture2D LoadTexture(string filePath, byte alpha)
        {
            if (!File.Exists(filePath))
                return null;

            var inputBitmap = new Bitmap(filePath);
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            int pixelCount;
            byte bytesPerPixel;

            if (inputBitmap.PixelFormat == _24bppRgb.imagePixelFormat)
            {
                Texture2D texture;
                if (alpha == 255)
                {
                    texture = new Texture2D(inputBitmap, _24bppRgb, true);
                    inputBitmap.Dispose();
                    return texture;
                }

                var outputBitmap = new Bitmap(width, height, _32bppArgb.imagePixelFormat);
                var rect = new Rectangle(0, 0, width, height);
                var inputData = inputBitmap.LockBits(rect, ImageLockMode.ReadWrite, _24bppRgb.imagePixelFormat);
                var outputData = outputBitmap.LockBits(rect, ImageLockMode.ReadWrite, _32bppArgb.imagePixelFormat);

                pixelCount = inputData.Stride * height;

                unsafe
                {
                    byte* inputPtr = (byte*)inputData.Scan0;
                    byte* outputPtr = (byte*)outputData.Scan0;
                    for (int y = 0; y < height; y++)
                    {
                        byte* inputPtrRow = inputPtr + y * inputData.Stride;
                        byte* outputPtrRow = outputPtr + y * outputData.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int inputIndex = x * _24bppRgb.bytesPerPixel;
                            int outputIndex = x * _32bppArgb.bytesPerPixel;

                            outputPtrRow[outputIndex] = inputPtrRow[inputIndex];
                            outputPtrRow[outputIndex + 1] = inputPtrRow[inputIndex + 1];
                            outputPtrRow[outputIndex + 2] = inputPtrRow[inputIndex + 2];
                            outputPtrRow[outputIndex + 3] = alpha;
                        }
                    }
                }

                inputBitmap.UnlockBits(inputData);
                outputBitmap.UnlockBits(outputData);

                texture = new Texture2D(outputBitmap, _32bppArgb, true);
                inputBitmap.Dispose();
                outputBitmap.Dispose();

                return texture;
            }
            else if (inputBitmap.PixelFormat == _32bppArgb.imagePixelFormat)
            {
                var rect = new Rectangle(0, 0, width, height);
                var data = inputBitmap.LockBits(rect, ImageLockMode.ReadWrite, _32bppArgb.imagePixelFormat);
                bytesPerPixel = _32bppArgb.bytesPerPixel;

                pixelCount = data.Stride * height;

                float alphaFactor = alpha / 255f;

                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;
                    for (int y = 0; y < height; y++)
                    {
                        byte* ptrRow = ptr + y * data.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int inputIndex = x * _32bppArgb.bytesPerPixel;

                            ptrRow[inputIndex + 3] = (byte)(ptrRow[inputIndex + 3] * alphaFactor);
                        }
                    }
                }

                inputBitmap.UnlockBits(data);

                var texture = new Texture2D(inputBitmap, _32bppArgb, true);
                inputBitmap.Dispose();
                return texture;
            }
            else
            {
                Logger.LogSingleErrorMessage(
                    EnumLocKey.EXCEPTION_WHILE_TEXTURE_LOADING_INCORRECT_TEXTURE_PIXEL_FORMAT_WITH_FILEPATH,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", filePath },
                        { "{currentPixelFormat}", inputBitmap.PixelFormat.ToString() },
                        { "{correctPixelFormats}", $"{_24bppRgb.imagePixelFormat}, {_32bppArgb.imagePixelFormat}" },
                    }
                );
                return null;
            }
        }

        public static void InitRegionsBordersMap(HashSet<ProvinceBorder> borders) => regionsBorders = InitBordersMapPair(borders);
        public static void InitStateBordersMap(HashSet<ProvinceBorder> borders) => statesBorders = InitBordersMapPair(borders);

        private static MapPair InitBordersMapPair(HashSet<ProvinceBorder> borders)
        {
            var mainBitmap = provincesBorders.GetBitmap();
            if (mainBitmap == null) return new MapPair();

            int width = mainBitmap.Width;
            int height = mainBitmap.Height;
            int pixelCount = width * height;

            byte[] newValues = new byte[pixelCount];

            short mapWidth = (short)MapManager.MapSize.x;

            foreach (var b in borders)
            {
                foreach (var p in b.pixels)
                {
                    if (p.x == 0 || p.x == mapWidth || p.y == 0)
                        continue;
                    newValues[(p.y - 1) * width + p.x - 1] = 127;
                }
            }

            var outputBitmap = new Bitmap(width, height, _8bppAlpha.imagePixelFormat);
            var outData = outputBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, _8bppAlpha.imagePixelFormat
            );

            Marshal.Copy(newValues, 0, outData.Scan0, newValues.Length);

            return new MapPair(false, outputBitmap, new Texture2D(outputBitmap, outData, _8bppAlpha, false));
        }
    }
}
