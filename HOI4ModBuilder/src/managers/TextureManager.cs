﻿using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder
{

    public class TextureManager
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

        public static readonly int[] riverColorsInts = {
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
        public static readonly List<Color> RiverColors = new List<Color>(riverColorsInts.Length);

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
                    }
                }
                return false;
            }

            public void RGBFill(object rawPixels, HashSet<Value2US> positions, int fillColor, TextureType textureType)
            {
                if (positions.Count == 0)
                    return;

                int width = _bitmap.Width;
                int height = _bitmap.Height;

                byte[] values = Utils.BitmapToArray(_bitmap, ImageLockMode.ReadOnly, textureType);

                if (textureType == _24bppRgb)
                {
                    var pixels = (int[])rawPixels;
                    foreach (var pos in positions)
                    {
                        int i = (pos.x + pos.y * width);
                        int byteI = i * textureType.bytesPerPixel;
                        pixels[i] = fillColor;
                        values[byteI] = (byte)fillColor;
                        values[byteI + 1] = (byte)(fillColor >> 8);
                        values[byteI + 2] = (byte)(fillColor >> 16);
                    }
                }
                else if (textureType == _8bppGrayscale)
                {
                    var pixels = (byte[])rawPixels;
                    foreach (var pos in positions)
                    {
                        int i = (pos.x + pos.y * width);
                        pixels[i] = (byte)fillColor;
                        values[i] = (byte)fillColor;
                    }
                }
                else
                    throw new Exception("Unsupported texture type: " + textureType.ToString());


                Utils.ArrayToBitmap(values, _bitmap, ImageLockMode.WriteOnly, width, height, textureType);

                texture.Update(textureType, 0, 0, width, height, values);

                needToSave = true;
            }

            public HashSet<Value2US> NewGetRGBPositions(ushort x, ushort y, TextureType textureType)
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

                byte[] values = Utils.BitmapToArray(_bitmap, ImageLockMode.ReadOnly, textureType);

                Func<int, bool> colorChecker;
                if (textureType == _24bppRgb)
                    colorChecker = (i) => values[i] == color.B && values[i + 1] == color.G && values[i + 2] == color.R;
                else if (textureType == _8bppGrayscale)
                    colorChecker = (i) => values[i] == color.B;
                else
                    throw new Exception("Unsupported texture type: " + textureType.ToString());

                byte[] state = new byte[1];

                const byte stateDo = 0;
                const byte stateWait = 1;
                const byte stateContinue = 2;
                const byte stateCancel = 3;
                const int maxSteps = 500_000;

                while (nextPoses.Count > 0)
                {
                    while (!(state[0] == stateDo || state[0] == stateContinue))
                    {
                        Thread.Sleep(50);
                        if (state[0] == stateCancel)
                            throw new Exception(GuiLocManager.GetLoc(EnumLocKey.ACTION_WAS_CANCELED));
                    }

                    pos = nextPoses.Dequeue();

                    //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
                    if (poses.Contains(pos) || pos.x >= width || pos.y >= height)
                        continue;

                    int i = (pos.x + pos.y * width) * textureType.bytesPerPixel;

                    if (colorChecker(i))
                    {
                        poses.Add(pos);

                        if (state[0] == stateDo && poses.Count > maxSteps)
                        {
                            state[0] = stateWait; // Ожидание действия
                            Task.Run(() =>
                            {
                                var title = GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION);
                                var text = GuiLocManager.GetLoc(
                                    EnumLocKey.WARNINGS_ACTION_WILL_AFFECT_MORE_THAN_X_PIXELS,
                                    new Dictionary<string, string> { { "{count}", "" + maxSteps } }
                                );

                                if (MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                    state[0] = stateContinue; // Продолжение
                                else
                                    state[0] = stateCancel; // Отмена
                            }); ;
                        }

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

        public static void LoadTextures(BaseSettings settings)
        {
            DisposeMapTextures();
            LoadMapPairs(settings);
            LoadBorders();
        }

        private static void LoadMapPairs(BaseSettings settings)
        {
            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);

            List<Action> actions = new List<Action>();

            MainForm.ExecuteActionsParallel(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS, new (string, Action)[]
            {
                (PROVINCES_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, PROVINCES_FILE_NAME, _24bppRgb, out var producerProvincesPixes, out var bitmap);
                    MapManager.ProvincesPixels = BrgToArgb(Utils.BitmapToArray(bitmap, ImageLockMode.ReadOnly, _24bppRgb), 255);
                    CreateBordersMap(MapManager.ProvincesPixels, bitmap.Width, bitmap.Height, out var producerProvincesBorders);
                    actions.Add(() => provinces = producerProvincesPixes());
                    actions.Add(() => provincesBorders = producerProvincesBorders());
                }),
                (TERRAIN_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, TERRAIN_FILE_NAME, _8bppIndexed, out var producer, out var _);
                    actions.Add(() => terrain = producer());
                }),
                (TREES_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, TREES_FILE_NAME, _8bppIndexed, out var producer, out var _);
                    actions.Add(() => trees = producer());
                }),
                (CITIES_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, CITIES_FILE_NAME, _8bppIndexed, out var producer, out var _);
                    actions.Add(() => cities = producer());
                }),
                (HEIGHTMAP_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, HEIGHTMAP_FILE_NAME, _8bppGrayscale, out var producer, out var bitmap);
                     MapManager.HeightsPixels = Utils.BitmapToArray(bitmap, ImageLockMode.ReadOnly, _8bppGrayscale);
                    actions.Add(() => height = producer());
                }),
                (WORLD_NORMAL_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, WORLD_NORMAL_FILE_NAME, _24bppRgb, out var producer, out var _);
                    actions.Add(() => normal = producer());
                }),
                (RIVERS_FILE_NAME, () => {
                    CreateRiverMapProducer(fileInfoPairs, RIVERS_FILE_NAME, out var producer);
                    actions.Add(() => rivers = producer());
                })
            });


            MainForm.ExecuteActions(new (EnumLocKey, Action)[] {
                (EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS, () =>
                {
                    foreach (var action in actions)
                        action();

                    var bitmap = new Bitmap(1, 1, _8bppGrayscale.imagePixelFormat);
                    var texture = new Texture2D(bitmap, _8bppGrayscale, false);
                    none = new MapPair(false, bitmap, texture);
                    texture.Update(_8bppGrayscale, 0, 0, 1, 1, new byte[] { 255 });
                })
            });
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

        private static void CreateMapPairProducer(Dictionary<string, src.FileInfo> fileInfos, string fileName, TextureType textureType, out Func<MapPair> producer, out Bitmap outBitmap)
        {
            if (!fileInfos.TryGetValue(fileName, out src.FileInfo fileInfo))
            {
                var errorBitmap = new Bitmap(0, 0);
                Logger.LogError(
                    EnumLocKey.ERROR_MAP_FILE_NOT_FOUND,
                    new Dictionary<string, string> { { "{fileName}", fileName } }
                );

                producer = () => new MapPair(false, errorBitmap, new Texture2D(errorBitmap, textureType, false));
                outBitmap = errorBitmap;
                return;
            }

            Bitmap bitmap;
            using (var stream = new FileStream(fileInfo.filePath, FileMode.Open, FileAccess.Read))
            {
                var tempBitmap = new Bitmap(stream);
                bitmap = tempBitmap.Clone(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), textureType.imagePixelFormat);
                tempBitmap.Dispose();
            }

            outBitmap = bitmap;
            if (textureType == _8bppIndexed)
            {
                var resultBitmap = Transfer8bbpIndexedTo24bpp(bitmap);
                producer = () => new MapPair(fileInfo.needToSave, bitmap, new Texture2D(resultBitmap, _24bppRgb, false));
            }
            else
                producer = () => new MapPair(fileInfo.needToSave, bitmap, new Texture2D(bitmap, textureType, false));
        }

        public static void LoadBorders()
        {
            MainForm.ExecuteActions(new (EnumLocKey, Action)[]
            {
                (EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_BORDERS_TEXTURE_MAP, () => ProvinceManager.ProcessProvincesPixels(MapManager.ProvincesPixels, provinces.GetBitmap().Width, provinces.GetBitmap().Height))
            });
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

        private static void CreateBordersMap(int[] values, int width, int height, out Func<MapPair> producer)
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

            producer = () => new MapPair(false, outputBitmap, new Texture2D(outputBitmap, outData, _8bppAlpha, false));
        }

        public static void InitRiverPallete()
        {
            _riverColorsSet.Clear();
            _riverColorsMap.Clear();
            RiverColors.Clear();

            for (byte i = 0; i < riverColorsInts.Length; i++)
            {
                _riverColorsSet.Add(riverColorsInts[i]);
                _riverColorsMap[riverColorsInts[i]] = i;
                RiverColors.Add(Color.FromArgb(riverColorsInts[i]));
            }
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

        public static void SaveAllMaps(BaseSettings settings)
            => MainForm.ExecuteActionsParallel(EnumLocKey.MAP_TAB_PROGRESSBAR_SAVING_TEXTURE_MAPS, GetSaveAllActions(settings));

        public static (string, Action)[] GetSaveAllActions(BaseSettings settings)
        {
            return new (string, Action)[]
            {
                ("ProvincesMap", () => SaveProvincesMap()),
                ("RiversMap", () => SaveRiversMap(settings)),
                ("TerrainMap", () => SaveTerrainMap()),
                ("TreesMap", () => SaveTreesMap()),
                ("CitiesMap", () => SaveCitiesMap()),
                ("Height and Normal Maps", () => SaveHeightAndNormalMaps(settings))
            };
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

        public static void SaveHeightAndNormalMaps(BaseSettings settings)
        {
            if (height.needToSave)
            {
                SaveHeightMap(height.GetBitmap());

                var modSettings = settings.GetModSettings();

                if (modSettings.generateNormalMap)
                {
                    SaveNormalMap(NormalMapGenerator.GenerateNormalMap(
                        height.GetBitmap(),
                        modSettings.normalMapStrength,
                        modSettings.normalMapBlur
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

        public static void SaveRiversMap(BaseSettings settings)
        {
            if (rivers.needToSave)
                SaveRiversMap(settings, rivers.GetBitmap());
        }

        public static void SaveRiversMap(BaseSettings settings, Bitmap inputBitmap)
        {
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            int pixelCount = width * height * _32bppArgb.bytesPerPixel;

            byte[] riversBytes = Utils.BitmapToArray(inputBitmap, ImageLockMode.ReadOnly, _32bppArgb);
            byte[] outputValues = new byte[pixelCount];

            int colorRiver;
            int? prevColorRiver = null;

            int colorProvince;
            int? prevColorProvince = null;

            byte outputValue = 0;

            if (settings.GetModSettings().exportRiversMapWithWaterPixels)
            {
                for (int i = 0; i < pixelCount; i += 4)
                {
                    colorRiver = Utils.ArgbToInt(riversBytes[i + 3], riversBytes[i + 2], riversBytes[i + 1], riversBytes[i]);
                    colorProvince = MapManager.ProvincesPixels[i / 4];

                    if (colorRiver != prevColorRiver || colorProvince != prevColorProvince)
                    {
                        prevColorRiver = colorRiver;
                        prevColorProvince = colorProvince;

                        if (_riverColorsMap.ContainsKey(colorRiver))
                            outputValue = _riverColorsMap[colorRiver];
                        else if (ProvinceManager.TryGetProvince(colorProvince, out Province province) && province.Type != EnumProvinceType.LAND)
                            outputValue = 254;
                        else
                            outputValue = 255;
                    }
                    outputValues[i / 4] = outputValue;
                }
            }
            else
            {
                for (int i = 0; i < pixelCount; i += 4)
                {
                    colorRiver = Utils.ArgbToInt(riversBytes[i + 3], riversBytes[i + 2], riversBytes[i + 1], riversBytes[i]);

                    if (colorRiver != prevColorRiver)
                    {
                        prevColorRiver = colorRiver;

                        if (_riverColorsMap.ContainsKey(colorRiver))
                            outputValue = _riverColorsMap[colorRiver];
                        else
                            outputValue = 255;
                    }
                    outputValues[i / 4] = outputValue;
                }
            }

            var outputBitmap = new Bitmap(width, height, _8bppIndexed.imagePixelFormat);

            var palette = outputBitmap.Palette;
            for (byte i = 0; i < riverColorsInts.Length; i++) palette.Entries[i] = Color.FromArgb(riverColorsInts[i]);

            Color colorStruct = Color.FromArgb(255, 0, 0, 0);
            for (byte i = (byte)riverColorsInts.Length; i < 254; i++) palette.Entries[i] = colorStruct;

            palette.Entries[254] = Color.FromArgb(255, 122, 122, 122);
            palette.Entries[255] = Color.FromArgb(255, 255, 255, 255);
            outputBitmap.Palette = palette;

            Utils.ArrayToBitmap(outputValues, outputBitmap, ImageLockMode.WriteOnly, width, height, _8bppIndexed);
            outputBitmap.Save(SettingsManager.Settings.modDirectory + FOLDER_PATH + RIVERS_FILE_NAME, ImageFormat.Bmp);
        }

        private static void CreateRiverMapProducer(Dictionary<string, src.FileInfo> fileInfos, string fileName, out Func<MapPair> producer)
        {
            var fileInfo = fileInfos[fileName];
            LoadRiverMap(new Bitmap(fileInfo.filePath), out Bitmap outputBitmap, out BitmapData outputData);
            producer = () => new MapPair(fileInfo.needToSave, outputBitmap, new Texture2D(outputBitmap, outputData, _32bppArgb, false));
        }

        public static void LoadSegmentedTextures(
            string filePath, BaseSettings settings, List<Texture2D> textures, out float imageWidth, out float imageHeight
        )
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
