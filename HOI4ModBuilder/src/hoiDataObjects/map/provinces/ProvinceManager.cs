using HOI4ModBuilder.hoiDataObjects.map;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using static HOI4ModBuilder.TextureManager;
using static HOI4ModBuilder.utils.Structs;
using OpenTK.Graphics.OpenGL;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using System.Windows.Forms;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.hoiDataObjects;

namespace HOI4ModBuilder.managers
{

    class ProvinceManager
    {
        public static bool NeedToSave { get; set; }
        private static bool _hasProcessedDefinitionFile;
        public static ushort NextVacantProvinceId { get; set; }
        public static Province SelectedProvince { get; set; }
        public static Province RMBProvince { get; set; }
        private static Dictionary<ushort, Province> _provincesById = new Dictionary<ushort, Province>();
        private static Dictionary<int, Province> _provincesByColor = new Dictionary<int, Province>();

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            NextVacantProvinceId = 1;
            LoadProvinces(settings);
        }

        public static void AddProvince(Province province, out bool canAddById, out bool canAddByColor)
        {
            ushort id = province.Id;
            int color = province.Color;
            canAddById = _provincesById.ContainsKey(id);
            canAddByColor = _provincesByColor.ContainsKey(color);

            if (canAddById && canAddByColor)
            {
                _provincesById[id] = province;
                _provincesByColor[color] = province;
                NeedToSave = true;
            }
        }

        public static Color3B GetNewLandColor()
        {
            Color3B color = Color3B.GetRandowColor();

            int max = color.red;
            if (color.green > max) max = color.green;
            if (color.blue > max) max = color.blue;
            int sum = color.red + color.green + color.blue;

            while (sum < 300 || _provincesByColor.ContainsKey(color.GetHashCode()))
            {
                color = Color3B.GetRandowColor();
                max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;
                sum = color.red + color.green + color.blue;
            }

            return color;
        }

        public static Color3B GetNewSeaColor()
        {
            Color3B color = Color3B.GetRandowColor();

            int max = color.red;
            if (color.green > max) max = color.green;
            if (color.blue > max) max = color.blue;
            int sum = color.red + color.green + color.blue;

            while (sum > 299 || max > 127 || _provincesByColor.ContainsKey(color.GetHashCode()))
            {
                color = Color3B.GetRandowColor();
                max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;
                sum = color.red + color.green + color.blue;
            }

            return color;
        }

        public static Color3B GetNewLakeColor()
        {
            Color3B color = Color3B.GetRandowColor();

            int max = color.red;
            if (color.green > max) max = color.green;
            if (color.blue > max) max = color.blue;
            int sum = color.red + color.green + color.blue;

            while (sum > 299 || max > 127 || _provincesByColor.ContainsKey(color.GetHashCode()))
            {
                color = Color3B.GetRandowColor();
                max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;
                sum = color.red + color.green + color.blue;
            }

            return color;
        }

        public static bool ContainsProvinceIdKey(ushort id) => _provincesById.ContainsKey(id);
        public static Dictionary<ushort, Province>.KeyCollection GetProvincesIds() => _provincesById.Keys;
        public static bool TryGetProvince(ushort id, out Province province) => _provincesById.TryGetValue(id, out province);
        public static bool TryGetProvince(int color, out Province province) => _provincesByColor.TryGetValue(color, out province);
        public static bool TryGetProvince(Point2D point, out Province province) => TryGetProvince(MapManager.GetColor(point), out province);
        public static Dictionary<ushort, Province>.ValueCollection GetProvinces() => _provincesById.Values;
        public static int ProvincesCount => _provincesById.Count;

        public static void AddProvince(ushort id, Province province)
        {
            _provincesById[id] = province;
            NeedToSave = true;
        }

        public static void RemoveProvinceById(ushort id)
        {
            _provincesById.Remove(id);
            NeedToSave = true;
        }
        public static void AddProvince(int color, Province province)
        {
            _provincesByColor[color] = province;
            NeedToSave = true;
        }

        public static bool CreateNewProvince(int color)
        {
            if (_provincesByColor.ContainsKey(color)) return false;

            var ids = new List<ushort>(GetProvincesIds());
            ids.Sort();

            ushort prevId = 0;
            ushort newId = 0;
            Province p;
            foreach (ushort id in ids)
            {
                if (id - prevId > 1)
                {
                    newId = (ushort)(prevId + 1);
                    p = new Province(newId, color);

                    _provincesById[newId] = p;
                    _provincesByColor[color] = p;
                    NeedToSave = true;
                    return true;
                }
                else prevId = id;
            }

            newId = (ushort)(prevId + 1);
            p = new Province(newId, color);

            _provincesById[newId] = p;
            _provincesByColor[color] = p;
            NextVacantProvinceId = (ushort)(newId + 1);
            NeedToSave = true;
            return true;
        }

        public static void RemoveProvinceByColor(int color)
        {
            _provincesByColor.Remove(color);
            NeedToSave = true;
        }

        /**
         * Метод загрузки данных о провинциях
         */
        public static void LoadProvinces(Settings settings)
        {
            _provincesById = new Dictionary<ushort, Province>();
            _provincesByColor = new Dictionary<int, Province>();

            var fileInfos = FileManager.ReadMultiFileInfos(settings, @"map\");

            if (!fileInfos.TryGetValue("definition.csv", out src.FileInfo fileInfo))
            {
                throw new FileNotFoundException("definition.csv");
            }

            NeedToSave = fileInfo.needToSave;
            ProcessDefinitionFile(fileInfo.filePath);
        }

        public static void SaveProvinces(Settings settings)
        {
            if (!NeedToSave) return;

            string filePath = settings.modDirectory + @"map\definition.csv";
            ushort[] ids = _provincesById.Keys.OrderBy(x => x).ToArray();
            var sb = new StringBuilder();
            if (ids.Length > 0 && ids[0] != 0)
            {
                sb.Append("0;0;0;0;land;false;unknown;0").Append(Constants.NEW_LINE);
            }
            foreach (ushort id in ids)
            {
                var province = _provincesById[id];
                province.Save(sb);
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        public static void Draw(bool showCenters)
        {
            if (showCenters)
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
                GL.Translate(0f, 1f, 0f);
                GL.Begin(PrimitiveType.Points);

                foreach (var province in _provincesByColor.Values)
                {
                    if (province.dislayCenter) GL.Vertex2(province.center.x, province.center.y);
                }

                GL.End();
                GL.Translate(0f, -1f, 0f);
            }

            if (SelectedProvince != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedProvince.borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2US vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }

                GL.Color3(0f, 0f, 1f);
                GL.Begin(PrimitiveType.Points);
                foreach (var border in SelectedProvince.borders)
                {
                    GL.Vertex2(border.center.x, border.center.y);
                }
                GL.End();
            }

            if (RMBProvince != null)
            {
                GL.Color4(1f, 0.42f, 0f, 1f);
                GL.LineWidth(2.5f);

                foreach (var border in RMBProvince.borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2US vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }
            }
        }

        public static Province SelectProvince(int color)
        {
            SelectedProvince = _provincesByColor[color];
            StateManager.SelectedState = null;
            StrategicRegionManager.SelectedRegion = null;
            return SelectedProvince;
        }

        public static Province SelectRMBProvince(int color)
        {
            RMBProvince = _provincesByColor[color];
            StateManager.RMBState = null;
            StrategicRegionManager.RMBRegion = null;
            return SelectedProvince;
        }

        private static void ProcessDefinitionFile(string filePath)
        {
            ushort id = 0;
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                try
                {
                    string[] values = line.Split(';');

                    if (values.Length != 8)
                    {
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_INCORRECT_PARAMS_COUNT,
                            new Dictionary<string, string>
                            {
                                { "{lineIndex}", $"{i}" },
                                { "{currentCount}", $"{values.Length}" },
                                { "{correctCount}", "8" },
                            }
                        );
                        continue;
                    }

                    if (!ushort.TryParse(values[0], out id))
                    {
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_INCORRECT_ID_VALUE,
                            new Dictionary<string, string>
                            {
                                { "{lineIndex}", $"{i}" },
                                { "{provinceId}", values[0] }
                            }
                        );
                    }

                    if (id == 0) continue;

                    if (_provincesById.ContainsKey(id))
                    {
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_DUPLICATE_ID,
                            new Dictionary<string, string>
                            {
                                { "{lineIndex}", $"{i}" },
                                { "{provinceId}", $"{id}" }
                            }
                        );
                    }

                    if (id != NextVacantProvinceId)
                    {
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_INCORRECT_ID_SEQUENCE,
                            new Dictionary<string, string>
                            {
                                { "{lineIndex}", $"{i}" },
                                { "{provinceId}", $"{id}" },
                                { "{previousProvinceId}", $"{NextVacantProvinceId - 1}" }
                            }
                        );
                    }

                    if (id >= NextVacantProvinceId)
                    {
                        NextVacantProvinceId = (ushort)(id + 1);
                    }

                    int color = -1;
                    try
                    {
                        color = Utils.ArgbToInt(
                            255,
                            byte.Parse(values[1]),
                            byte.Parse(values[2]),
                            byte.Parse(values[3])
                        );
                    }
                    catch (Exception _)
                    {
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_INCORRECT_COLOR_VALUE,
                            new Dictionary<string, string>
                            {
                                { "{lineIndex}", $"{i}" },
                                { "{provinceId}", $"{id}" },
                                { "{provinceColor}", $"{values[1]};{values[2]};{values[3]}" }
                            }
                        );
                    }

                    if (_provincesByColor.ContainsKey(color))
                    {
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_DUPLICATE_COLOR,
                            new Dictionary<string, string>
                            {
                                { "{lineIndex}", $"{i}" },
                                { "{provinceId}", $"{id}" },
                                { "{provinceColor}", $"{values[1]};{values[2]};{values[3]}" }
                            }
                        );
                    }

                    if (!Enum.TryParse(values[4].ToUpper(), out EnumProvinceType type))
                        Logger.LogError(
                            EnumLocKey.ERROR_PROVINCE_INCORRECT_TYPE_VALUE,
                            new Dictionary<string, string>
                            {
                                    { "{lineIndex}", $"{i}" },
                                    { "{provinceId}", $"{id}" },
                                    { "{provinceType}", $"{values[4]}" }
                            }
                        );

                    if (!bool.TryParse(values[5], out bool isCoastal))
                    {
                        Logger.LogError(
                                EnumLocKey.ERROR_PROVINCE_INCORRECT_IS_COASTAL_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{lineIndex}", $"{i}" },
                                    { "{provinceId}", $"{id}" },
                                    { "{provinceIsCoastal}", $"{values[5]}" }
                                }
                            );
                    }

                    if (!TerrainManager.TryGetProvincialTerrain(values[6], out ProvincialTerrain terrain))
                    {
                        Logger.LogError(
                                EnumLocKey.ERROR_PROVINCE_INCORRECT_TERRAIN_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{lineIndex}", $"{i}" },
                                    { "{provinceId}", $"{id}" },
                                    { "{provinceTerrain}", $"{values[6]}" }
                                }
                            );
                    }

                    byte continentId;
                    if (!byte.TryParse(values[7], out continentId) || continentId > ContinentManager.GetContinentsCount())
                    {
                        Logger.LogError(
                                EnumLocKey.ERROR_PROVINCE_INCORRECT_CONTINENT_ID_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{lineIndex}", $"{i}" },
                                    { "{provinceId}", $"{id}" },
                                    { "{provinceContinentId}", $"{values[7]}" }
                                }
                            );
                    }

                    var province = new Province(id, color, type, isCoastal, terrain, continentId);
                    _provincesById.Add(id, province);
                    _provincesByColor.Add(color, province);
                }
                catch (Exception _)
                {
                    Logger.LogError(
                                EnumLocKey.ERROR_PROVINCE_INCORRECT_LINE,
                                new Dictionary<string, string>
                                {
                                    { "{lineIndex}", $"{i}" },
                                    { "{line}", line }
                                }
                            );
                    continue;
                }
            }

            _hasProcessedDefinitionFile = true;
        }

        /**
         * Метод для вычисления и установки центров провинций
         */
        public static void ProcessProvincesPixels(int[] values, int width, int height)
        {
            if (!_hasProcessedDefinitionFile)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_PROCESS_PROVINCES_PIXELS_WAS_BEFORE_PROCESS_DEFITION_FILE));

            int pixelCount = width * height;

            int color = 0, prevColor = 0;

            Province province = null;

            int x, y;

            for (int i = 0; i < pixelCount; i++)
            {
                //Вычисляем координаты пикселя
                x = i % width;
                y = i / width;

                color = values[i];
                //Если совпадает ли цвет пикселя с цветом предыдущего
                if (color != prevColor)
                {
                    //Если провинции нет в словаре провинций
                    if (!_provincesByColor.TryGetValue(color, out province))
                    {
                        //Создаём новую провинцию и добавляем её в словарь провинций
                        province = new Province(NextVacantProvinceId, color);
                        _provincesById.Add(NextVacantProvinceId, province);
                        _provincesByColor.Add(color, province);
                        NextVacantProvinceId++;
                        while (_provincesById.ContainsKey(NextVacantProvinceId)) NextVacantProvinceId++;

                        Logger.LogSingleMessage(
                            EnumLocKey.SINGLE_MESSAGE_NEW_PROVINCE_WITH_COLOR_WAS_CREATED,
                            new Dictionary<string, string> {
                                { "{provinceId}", $"{province.Id}" },
                                { "{color}", new Color3B(color).ToString() },
                                { "{position}", $"{x}; {y}" },
                            }
                        );
                    }

                    //Устанавливаем предыдущий цвет
                    prevColor = color;
                }
                //Добавляем пиксель в учёт центральной координаты провинции

                province?.AddPixel(x, y);
            }

            CheckDisplayProvincesCenters(values, width);

            StateManager.CalculateCenters();
            CheckDisplayStatesCenters(values, width);

            StrategicRegionManager.CalculateCenters();
            CheckDisplayRegionsCenters(values, width);
        }

        private static void CheckDisplayProvincesCenters(int[] values, int width)
        {
            int i;
            foreach (var province in _provincesByColor.Values)
            {
                ushort provinceId = province.Id;
                int color = province.Color;

                if (province.pixelsCount == 0 && provinceId != 0)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_PROVINCE_HAS_ZERO_PIXELS,
                        new Dictionary<string, string> {
                            { "{id}", $"{provinceId}" },
                            { "{color}", new Color3B(color).ToString() }
                        }
                    );
                    continue;
                }

                i = ((int)Math.Round(province.center.y) * width + (int)Math.Round(province.center.x));

                if ((i < 0 || i > values.Length - 2) && provinceId != 0)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_PROVINCE_HAS_CENTER_OUTSIDE_THE_MAP,
                        new Dictionary<string, string> {
                            { "{id}", $"{provinceId}" },
                            { "{color}", new Color3B(color).ToString() },
                            { "{centerPosition}", $"{province.center.x}; {province.center.y}" },
                            { "{pixelsCount}", $"{province.pixelsCount}" }
                        }
                    );
                    continue;
                }

                if (color == values[i]) province.dislayCenter = true;
            }
        }

        private static void CheckDisplayStatesCenters(int[] values, int width)
        {
            int i;
            foreach (var state in StateManager.GetStates())
            {
                int stateId = state.Id;
                if (state.pixelsCount == 0 && stateId != 0)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_STATE_HAS_ZERO_PIXELS,
                        new Dictionary<string, string> {
                            { "{id}", $"{stateId}" }
                        }
                    );
                    continue;
                }

                i = ((int)Math.Round(state.center.y) * width + (int)Math.Round(state.center.x));

                if ((i < 0 || i > values.Length) && stateId != 0)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_STATE_HAS_CENTER_OUTSIDE_THE_MAP,
                        new Dictionary<string, string> {
                            { "{id}", $"{stateId}" },
                            { "{centerPosition}", $"{state.center.x}; {state.center.y}" },
                            { "{pixelsCount}", $"{state.pixelsCount}" }
                        }
                    );
                    continue;
                }

                var province = _provincesByColor[values[i]];
                if (province != null && province.State != null && province.State.Equals(state)) state.dislayCenter = true;
            }
        }

        private static void CheckDisplayRegionsCenters(int[] values, int width)
        {
            int i;
            foreach (var region in StrategicRegionManager.GetRegions())
            {
                int regionId = region.Id;
                if (region.pixelsCount == 0 && regionId != 0)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_REGION_HAS_ZERO_PIXELS,
                        new Dictionary<string, string> {
                            { "{id}", $"{regionId}" }
                        }
                    );
                    continue;
                }

                i = ((int)Math.Round(region.center.y) * width + (int)Math.Round(region.center.x));

                if ((i < 0 || i > values.Length) && regionId != 0)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_REGION_HAS_CENTER_OUTSIDE_THE_MAP,
                        new Dictionary<string, string> {
                            { "{id}", $"{regionId}" },
                            { "{centerPosition}", $"{region.center.x}; {region.center.y}" },
                            { "{pixelsCount}", $"{region.pixelsCount}" }
                        }
                    );
                    continue;
                }

                Province province = _provincesByColor[values[i]];
                if (province != null && province.Region != null && province.Region.Equals(region))
                {
                    region.dislayCenter = true;
                }
            }
        }

        public static void AutoToolIsCoastal()
        {
            foreach (var p in _provincesById.Values)
                p.IsCoastal = p.CheckCoastalType();
        }

        public static void AutoToolRemoveSeaAndLakesContinents()
        {
            foreach (var p in _provincesById.Values)
                if (p.Type != EnumProvinceType.LAND) p.ContinentId = 0;
        }

        public static void AutoToolRemoveSeaProvincesFromStates()
        {
            foreach (var p in _provincesById.Values)
                if (p.Type == EnumProvinceType.SEA && p.State != null) p.State.RemoveProvince(p);
        }

        public static void GetMinMaxVictoryPoints(out uint min, out uint max)
        {
            min = 0;
            max = 0;

            foreach (var province in _provincesById.Values)
            {
                min = province.victoryPoints;
                max = province.victoryPoints;
                break;
            }

            foreach (var province in _provincesById.Values)
            {
                if (province.victoryPoints > max) max = province.victoryPoints;
                else if (province.victoryPoints < min) min = province.victoryPoints;
            }
        }

        private static void HandleDelete()
        {

        }

        private static void HandleEscape()
        {
            SelectedProvince = null;
            RMBProvince = null;
        }
    }

}
