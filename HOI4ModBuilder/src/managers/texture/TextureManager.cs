using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.managers.texture;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.managers.texture
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

        private static readonly HashSet<Texture2D> _mapPairTextures = new HashSet<Texture2D>();
        private static readonly HashSet<Texture2D> _textures = new HashSet<Texture2D>();

        public static void DisposeMapTextures()
        {
            foreach (var mapPairTexture in new HashSet<Texture2D>(_mapPairTextures))
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
        public static void RemoveTexture(Texture2D texture)
        {
            _textures.Remove(texture);
            _mapPairTextures.Remove(texture);
        }

        public static void AddMapPairTexture(Texture2D texture)
        {
            if (texture != null)
                _mapPairTextures.Add(texture);
        }

        public static void LoadTextures(BaseSettings settings)
        {
            DisposeMapTextures();
            LoadMapPairs(settings);
            LoadBorders();
        }

        private static void LoadMapPairs(BaseSettings settings)
        {
            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);

            var actions = new ConcurrentQueue<Action>();

            MainForm.ExecuteActionsParallel(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXTURE_MAPS, new (string, Action)[]
            {
                (PROVINCES_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, PROVINCES_FILE_NAME, _24bppRgb, out var producerProvincesPixes, out var bitmap);
                    MapManager.ProvincesPixels = Bitmap24bppToArgb(bitmap, 255);
                    actions.Enqueue(() => provinces = producerProvincesPixes());
                }),
                (TERRAIN_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, TERRAIN_FILE_NAME, _8bppIndexed, out var producer, out var _);
                    actions.Enqueue(() => terrain = producer());
                }),
                (TREES_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, TREES_FILE_NAME, _8bppIndexed, out var producer, out var _);
                    actions.Enqueue(() => trees = producer());
                }),
                (CITIES_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, CITIES_FILE_NAME, _8bppIndexed, out var producer, out var _);
                    actions.Enqueue(() => cities = producer());
                }),
                (HEIGHTMAP_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, HEIGHTMAP_FILE_NAME, _8bppGrayscale, out var producer, out var bitmap);
                     MapManager.HeightsPixels = Utils.BitmapToArray(bitmap, ImageLockMode.ReadOnly, _8bppGrayscale);
                    actions.Enqueue(() => height = producer());
                }),
                (WORLD_NORMAL_FILE_NAME, () => {
                    CreateMapPairProducer(fileInfoPairs, WORLD_NORMAL_FILE_NAME, _24bppRgb, out var producer, out var _);
                    actions.Enqueue(() => normal = producer());
                }),
                (RIVERS_FILE_NAME, () => {
                    CreateRiverMapProducer(fileInfoPairs, RIVERS_FILE_NAME, out var producer);
                    actions.Enqueue(() => rivers = producer());
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
            int pixelCount = data.Length / 3;
            int[] values = new int[pixelCount];
            int a = alpha << 24;
            for (int i = 0, j = 0; j < pixelCount; i += 3, j++)
            {
                values[j] = a | (data[i + 2] << 16) | (data[i + 1] << 8) | data[i];
            }
            return values;
        }

        private static int[] Bitmap24bppToArgb(Bitmap bitmap, byte alpha)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[] values = new int[width * height];
            int a = alpha << 24;

            var data = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, _24bppRgb.imagePixelFormat
            );

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int stride = data.Stride;
                int index = 0;

                for (int y = 0; y < height; y++)
                {
                    byte* row = ptr + y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = x * _24bppRgb.bytesPerPixel;
                        values[index++] = a | (row[offset + 2] << 16) | (row[offset + 1] << 8) | row[offset];
                    }
                }
            }

            bitmap.UnlockBits(data);
            return values;
        }

        private static byte[] Indexed8bppToBgr24Bytes(Bitmap inputBitmap)
        {
            int width = inputBitmap.Width;
            int height = inputBitmap.Height;
            byte[] output = new byte[width * height * _24bppRgb.bytesPerPixel];

            Color[] paletteEntries = inputBitmap.Palette.Entries;
            int[] paletteBgr = new int[paletteEntries.Length];
            for (int i = 0; i < paletteEntries.Length; i++)
            {
                var color = paletteEntries[i];
                paletteBgr[i] = color.B | (color.G << 8) | (color.R << 16);
            }

            var rect = new Rectangle(0, 0, width, height);
            var inputData = inputBitmap.LockBits(rect, ImageLockMode.ReadOnly, _8bppIndexed.imagePixelFormat);

            unsafe
            {
                byte* inputPtr = (byte*)inputData.Scan0;
                int inputStride = inputData.Stride;

                fixed (byte* outputPtr = output)
                {
                    byte* outputRow = outputPtr;
                    for (int y = 0; y < height; y++)
                    {
                        byte* inputRow = inputPtr + y * inputStride;
                        for (int x = 0; x < width; x++)
                        {
                            int color = paletteBgr[inputRow[x]];
                            outputRow[0] = (byte)color;
                            outputRow[1] = (byte)(color >> 8);
                            outputRow[2] = (byte)(color >> 16);
                            outputRow += _24bppRgb.bytesPerPixel;
                        }
                    }
                }
            }

            inputBitmap.UnlockBits(inputData);
            return output;
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
            using (var stream = new FileStream(
                fileInfo.filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                1024 * 1024,
                FileOptions.SequentialScan
            ))
            {
                using (var tempBitmap = new Bitmap(stream))
                    bitmap = tempBitmap.Clone(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), textureType.imagePixelFormat);
            }

            outBitmap = bitmap;
            if (textureType == _8bppIndexed)
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                var resultBytes = Indexed8bppToBgr24Bytes(bitmap);
                producer = () =>
                {
                    var texture = new Texture2D(_24bppRgb, false, null, width, height, resultBytes);
                    return new MapPair(fileInfo.needToSave, bitmap, texture);
                };
            }
            else
            {
                producer = () => new MapPair(fileInfo.needToSave, bitmap, new Texture2D(bitmap, textureType, false));
            }
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
            var values = Indexed8bppToBgr24Bytes(inputBitmap);
            Utils.ArrayToBitmap(values, outputBitmap, ImageLockMode.WriteOnly, width, height, _24bppRgb);
            return outputBitmap;
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
            var palette = inputBitmap.Palette;

            for (int i = 0; i < palette.Entries.Length; i++)
            {
                int paletteColor = palette.Entries[i].ToArgb();
                if (_riverColorsSet.Contains(paletteColor)) _indexesToColors[i] = paletteColor;
                else _indexesToColors[i] = 0;
            }

            outputBitmap = new Bitmap(width, height, _32bppArgb.imagePixelFormat);
            var rect = new Rectangle(0, 0, width, height);
            var inputData = inputBitmap.LockBits(rect, ImageLockMode.ReadOnly, _8bppGrayscale.imagePixelFormat);
            outputData = outputBitmap.LockBits(rect, ImageLockMode.WriteOnly, _32bppArgb.imagePixelFormat);

            unsafe
            {
                byte* inputPtr = (byte*)inputData.Scan0;
                byte* outputPtr = (byte*)outputData.Scan0;
                int inputStride = inputData.Stride;
                int outputStride = outputData.Stride;

                for (int y = 0; y < height; y++)
                {
                    byte* inputRow = inputPtr + y * inputStride;
                    int* outputRow = (int*)(outputPtr + y * outputStride);
                    for (int x = 0; x < width; x++)
                        outputRow[x] = _indexesToColors[inputRow[x]];
                }
            }

            inputBitmap.UnlockBits(inputData);
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
            int pixelCount = width * height;

            byte[] riversBytes = Utils.BitmapToArray(inputBitmap, ImageLockMode.ReadOnly, _32bppArgb);
            byte[] outputValues = new byte[pixelCount];
            int riversByteCount = riversBytes.Length;

            var modSettings = settings.GetModSettings();
            bool exportWithWaterPixels = modSettings.exportRiversMapWithWaterPixels;

            int colorRiver;
            int? prevColorRiver = null;

            int colorProvince;
            int? prevColorProvince = null;

            byte outputValue = 0;

            if (exportWithWaterPixels)
            {
                for (int i = 0, pixelIndex = 0; i < riversByteCount; i += 4, pixelIndex++)
                {
                    colorRiver = Utils.ArgbToInt(riversBytes[i + 3], riversBytes[i + 2], riversBytes[i + 1], riversBytes[i]);
                    colorProvince = MapManager.ProvincesPixels[pixelIndex];

                    if (colorRiver != prevColorRiver || colorProvince != prevColorProvince)
                    {
                        prevColorRiver = colorRiver;
                        prevColorProvince = colorProvince;

                        if (_riverColorsMap.TryGetValue(colorRiver, out var mappedRiver))
                            outputValue = mappedRiver;
                        else if (ProvinceManager.TryGet(colorProvince, out Province province) && province.Type != EnumProvinceType.LAND)
                            outputValue = 254;
                        else
                            outputValue = 255;
                    }
                    outputValues[pixelIndex] = outputValue;
                }
            }
            else
            {
                for (int i = 0, pixelIndex = 0; i < riversByteCount; i += 4, pixelIndex++)
                {
                    colorRiver = Utils.ArgbToInt(riversBytes[i + 3], riversBytes[i + 2], riversBytes[i + 1], riversBytes[i]);

                    if (colorRiver != prevColorRiver)
                    {
                        prevColorRiver = colorRiver;

                        if (_riverColorsMap.TryGetValue(colorRiver, out var mappedRiver))
                            outputValue = mappedRiver;
                        else
                            outputValue = 255;
                    }
                    outputValues[pixelIndex] = outputValue;
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
            if (!fileInfos.TryGetValue(fileName, out src.FileInfo fileInfo))
            {
                var errorBitmap = new Bitmap(0, 0);
                Logger.LogError(
                    EnumLocKey.ERROR_MAP_FILE_NOT_FOUND,
                    new Dictionary<string, string> { { "{fileName}", fileName } }
                );

                producer = () => new MapPair(false, errorBitmap, new Texture2D(errorBitmap, _32bppArgb, false));
                return;
            }

            using (var inputBitmap = new Bitmap(fileInfo.filePath))
            {
                LoadRiverMap(inputBitmap, out Bitmap outputBitmap, out BitmapData outputData);
                producer = () => new MapPair(fileInfo.needToSave, outputBitmap, new Texture2D(outputBitmap, outputData, _32bppArgb, false));
            }
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
                var inputData = inputBitmap.LockBits(rect, ImageLockMode.ReadOnly, _24bppRgb.imagePixelFormat);
                var outputData = outputBitmap.LockBits(rect, ImageLockMode.WriteOnly, _32bppArgb.imagePixelFormat);

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
                if (alpha == 255)
                {
                    var tempTexture = new Texture2D(inputBitmap, _32bppArgb, true);
                    inputBitmap.Dispose();
                    return tempTexture;
                }

                var rect = new Rectangle(0, 0, width, height);
                var data = inputBitmap.LockBits(rect, ImageLockMode.ReadWrite, _32bppArgb.imagePixelFormat);

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
                inputBitmap.Dispose();
                return null;
            }
        }

        public static Func<MapPair> GetBordersMapPairProducer(ICollection<ProvinceBorder> borders)
        {
            int width = MapManager.MapSize.x + 1;
            int height = MapManager.MapSize.y + 1;

            var outputBitmap = new Bitmap(width, height, _8bppAlpha.imagePixelFormat);
            var outData = outputBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, _8bppAlpha.imagePixelFormat
            );

            unsafe
            {
                byte* basePtr = (byte*)outData.Scan0;
                int stride = outData.Stride;

                foreach (var b in borders)
                {
                    foreach (var p in b.pixels)
                    {
                        basePtr[p.y * stride + p.x] = 127;
                    }
                }
            }

            return () => new MapPair(false, outputBitmap, new Texture2D(outputBitmap, outData, _8bppAlpha, false));
        }
    }
}
