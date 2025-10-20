using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.classes;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.managers
{

    public class ProvinceManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map" });
        private static readonly string DEFINITION_FILE_NAME = "definition.csv";

        public static bool NeedToSave { get; set; }
        private static bool _hasProcessedDefinitionFile;
        public static ushort NextVacantProvinceId { get; set; }
        public static List<Province> GroupSelectedProvinces { get; private set; } = new List<Province> { };
        public static Point2F GetGroupSelectedProvincesCenter()
        {
            var commonCenter = new CommonCenter();
            foreach (var obj in GroupSelectedProvinces)
                commonCenter.Push((uint)obj.pixelsCount, obj.center);
            commonCenter.Get(out var _, out var center);
            return center;
        }
        public static Province SelectedProvince { get; set; }

        public static Province RMBProvince { get; set; }
        private static Province[] _provincesById = new Province[ushort.MaxValue];
        private static Dictionary<int, Province> _provincesByColor = new Dictionary<int, Province>();
        public static void ForEachProvince(Action<Province> action)
        {
            foreach (var p in _provincesById)
                if (p != null)
                    action(p);
        }

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            NextVacantProvinceId = 1;

            DeselectProvinces();

            _provincesById = new Province[ushort.MaxValue];
            _provincesByColor = new Dictionary<int, Province>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);

            if (!fileInfoPairs.TryGetValue(DEFINITION_FILE_NAME, out src.FileInfo fileInfo))
                throw new FileNotFoundException(DEFINITION_FILE_NAME);

            NeedToSave = fileInfo.needToSave;
            ProcessDefinitionFile(fileInfo.filePath);
        }
        public static void Save(Settings settings)
        {
            if (!NeedToSave) return;

            string filePath = settings.modDirectory + FOLDER_PATH + DEFINITION_FILE_NAME;

            var sb = new StringBuilder();
            if (_provincesById[0] == null)
                sb.Append("0;0;0;0;land;false;unknown;0").Append(Constants.NEW_LINE);

            for (ushort id = 1; id < _provincesById.Length; id++)
            {
                var province = _provincesById[id];
                if (province == null)
                    continue;
                province.Save(sb);
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        public static bool GetClosestPoint(Province provinceA, Value2S posB, out Value2S posA)
        {
            posA = default;

            if (provinceA == null || provinceA.borders.Count == 0)
                return false;

            posA = provinceA.borders[0].pixels[0];
            var closestSq = posA.GetSquareDistanceTo(posB);

            foreach (var borderA in provinceA.borders)
            {
                foreach (var pixelA in borderA.pixels)
                {
                    int distance = pixelA.GetSquareDistanceTo(posB);
                    if (distance < closestSq)
                    {
                        closestSq = distance;
                        posA = pixelA;
                    }
                }
            }

            return true;
        }

        public static bool GetClosestPoints(Province provinceA, Province provinceB, out Value2S posA, out Value2S posB)
        {
            posA = default;
            posB = default;

            if (provinceA == null || provinceA.borders.Count == 0 ||
                provinceB == null || provinceB.borders.Count == 0)
                return false;

            posA = provinceA.borders[0].pixels[0];
            posB = provinceB.borders[0].pixels[0];

            var closestSq = posA.GetSquareDistanceTo(posB);

            foreach (var borderA in provinceA.borders)
            {
                foreach (var borderB in provinceB.borders)
                {
                    foreach (var pixelA in borderA.pixels)
                    {
                        foreach (var pixelB in borderB.pixels)
                        {
                            int distance = pixelA.GetSquareDistanceTo(pixelB);
                            if (distance < closestSq)
                            {
                                closestSq = distance;
                                posA = pixelA;
                                posB = pixelB;
                            }
                        }
                    }
                }
            }

            return true;

        }

        public static void AddProvince(Province province, out bool canAddById, out bool canAddByColor)
        {
            ushort id = province.Id;
            int color = province.Color;
            canAddById = _provincesById[id] == null;
            canAddByColor = !_provincesByColor.ContainsKey(color);

            if (canAddById && canAddByColor)
            {
                _provincesById[id] = province;
                _provincesByColor[color] = province;
                NeedToSave = true;
            }
        }

        public static void GetMinMaxMapProvinceSizes(out int minCount, out int maxCount)
        {
            minCount = 0;
            maxCount = 0;
            foreach (var province in _provincesByColor.Values)
            {
                minCount = province.pixelsCount;
                maxCount = province.pixelsCount;
                break;
            }

            foreach (var province in _provincesByColor.Values)
            {
                if (province.pixelsCount < minCount)
                    minCount = province.pixelsCount;
                else if (province.pixelsCount > maxCount)
                    maxCount = province.pixelsCount;
            }
        }

        public static List<Province> FindPathAStar(Province start, Province goal)
            => FindPathAStar(start, goal, (p) => true);
        public static List<Province> FindPathAStar(Province start, Province goal, Func<Province, bool> provinceChecker)
        {
            if (start == null || goal == null)
                return new List<Province>();

            if (provinceChecker == null)
                provinceChecker = (p) => true;

            if (!provinceChecker(start) || !provinceChecker(goal))
                return new List<Province>();

            if (start.Id == goal.Id)
                return new List<Province> { start };

            var closedSet = new HashSet<Province>();
            var openSet = new List<Province> { start };

            var cameFrom = new Dictionary<Province, Province>();
            var gScore = new Dictionary<Province, double> { [start] = 0d };
            var fScore = new Dictionary<Province, double>
            {
                [start] = start.center.GetDistanceTo(goal.center)
            };

            while (openSet.Count > 0)
            {
                Province current = openSet[0];
                double currentFScore = fScore[current];
                for (int i = 1; i < openSet.Count; i++)
                {
                    Province p = openSet[i];
                    double score = fScore.TryGetValue(p, out double val) ? val : double.PositiveInfinity;
                    if (score < currentFScore)
                    {
                        current = p;
                        currentFScore = score;
                    }
                }

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                current.ForEachAdjacentProvince((from, neighbor) =>
                {
                    if (!provinceChecker(neighbor))
                        return;

                    if (closedSet.Contains(neighbor))
                        return;

                    double tentativeG = gScore[current] + from.center.GetDistanceTo(neighbor.center);

                    if (!gScore.TryGetValue(neighbor, out double neighborG) || tentativeG < neighborG)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + neighbor.center.GetDistanceTo(goal.center);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                });
            }

            return new List<Province>();
        }

        private static List<Province> ReconstructPath(Dictionary<Province, Province> cameFrom, Province current)
        {
            var totalPath = new List<Province> { current };
            while (cameFrom.TryGetValue(current, out Province previous))
            {
                current = previous;
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath;
        }

        public static Color3B GetNewRandomColor()
        {
            Color3B color = Color3B.GetRandowColor();

            int iter = 0;

            while (iter < 10000 && _provincesByColor.ContainsKey(color.ToArgb()))
            {
                color = Color3B.GetRandowColor();
                iter++;
            }

            return color;

        }

        public static Color3B GetNewLandColor()
        {
            Color3B color = Color3B.GetRandowColor();

            int max = color.red;
            if (color.green > max) max = color.green;
            if (color.blue > max) max = color.blue;
            int sum = color.red + color.green + color.blue;

            int iter = 0;

            while (iter < 10000 && (sum < 300 || _provincesByColor.ContainsKey(color.ToArgb())))
            {
                color = Color3B.GetRandowColor();
                max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;
                sum = color.red + color.green + color.blue;

                iter++;
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

            int iter = 0;

            while (iter < 10000 && (sum > 299 || max > 127 || _provincesByColor.ContainsKey(color.ToArgb())))
            {
                color = Color3B.GetRandowColor();
                max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;
                sum = color.red + color.green + color.blue;

                iter++;
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

            int iter = 0;

            while (iter < 10000 && (sum > 299 || max > 127 || _provincesByColor.ContainsKey(color.ToArgb())))
            {
                color = Color3B.GetRandowColor();
                max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;
                sum = color.red + color.green + color.blue;

                iter++;
            }

            return color;
        }

        public static bool ContainsProvinceIdKey(ushort id) => _provincesById[id] != null;
        public static List<ushort> GetProvincesIds()
        {
            var ids = new List<ushort>(_provincesByColor.Count);
            for (ushort id = 0; id < _provincesById.Length; id++)
            {
                if (_provincesById[id] != null)
                    ids.Add(id);
            }
            return ids;
        }
        public static bool TryGetProvince(ushort id, out Province province)
        {
            province = _provincesById[id];
            return province != null;
        }
        public static bool TryGetProvince(int color, out Province province) => _provincesByColor.TryGetValue(color, out province);
        public static Province GetProvince(ushort id) => _provincesById[id];
        public static Province GetProvince(int color) => _provincesByColor[color];
        public static bool TryGetProvince(Point2D point, out Province province) => TryGetProvince(MapManager.GetColor(point), out province);
        public static Dictionary<int, Province>.ValueCollection GetProvinces() => _provincesByColor.Values;
        public static int ProvincesCount => _provincesByColor.Count;

        public static void AddProvince(ushort id, Province province)
        {
            _provincesById[id] = province;
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
            Province p;
            ushort newId;
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

        public static bool RemoveProvinceByColor(int color)
        {
            if (!_provincesByColor.TryGetValue(color, out var province))
                return false;

            return RemoveProvinceData(province);
        }

        public static void ChangeProvinceID(ushort fromID, ushort toID)
        {
            if (_provincesById[toID] != null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_PROVINCE_ID_UPDATE_VALUE_IS_USED,
                    new Dictionary<string, string> { { "{id}", $"{toID}" } }
                ));
            _provincesById[toID] = _provincesById[fromID];
            _provincesById[fromID] = null;

            if (fromID == NextVacantProvinceId - 1)
            {
                for (int i = NextVacantProvinceId - 1; i > 0; i--)
                {
                    if (_provincesById[i] == null)
                        NextVacantProvinceId = (ushort)i;
                    else
                        break;
                }
            }
        }

        public static bool RemoveProvinceById(ushort id)
        {
            if (_provincesById[id] == null)
                return false;

            return RemoveProvinceData(_provincesById[id]);
        }

        private static bool RemoveProvinceData(Province province)
        {
            if (province == null)
                return false;

            _provincesById[province.Id] = null;
            _provincesByColor.Remove(province.Color);
            province.ResetPixels();

            SupplyManager.RemoveProvinceData(province);
            AdjacenciesManager.RemoveProvinceData(province);
            StateManager.RemoveProvinceData(province);
            StrategicRegionManager.RemoveProvinceData(province);

            NeedToSave = true;
            return true;
        }


        public static void Draw(bool showCenters, bool showCollisions)
        {
            if (showCenters)
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                foreach (var province in _provincesByColor.Values)
                {
                    if (province.dislayCenter)
                        GL.Vertex2(province.center.x + 0.5f, province.center.y + 0.5f);
                }

                GL.End();
            }

            if (GroupSelectedProvinces != null && GroupSelectedProvinces.Count > 0)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(8f);

                foreach (var province in GroupSelectedProvinces)
                {
                    foreach (var border in province.borders)
                    {
                        if (border.pixels.Length == 1)
                            continue;

                        if (border.provinceA.Id == province.Id && GroupSelectedProvinces.Contains(border.provinceB) ||
                            border.provinceB.Id == province.Id && GroupSelectedProvinces.Contains(border.provinceA))
                            continue;

                        GL.Begin(PrimitiveType.LineStrip);
                        foreach (Value2S vertex in border.pixels)
                        {
                            GL.Vertex2(vertex.x, vertex.y);
                        }
                        GL.End();
                    }
                }
            }

            if (SelectedProvince != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedProvince.borders)
                {
                    if (border.pixels.Length == 1)
                        continue;

                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2S vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }

                if (showCollisions)
                {
                    GL.Color4(0f, 0f, 1f, 1f);
                    GL.LineWidth(3f);
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Vertex2(SelectedProvince.bounds.left, SelectedProvince.bounds.top);
                    GL.Vertex2(SelectedProvince.bounds.right + 1, SelectedProvince.bounds.top);
                    GL.Vertex2(SelectedProvince.bounds.right + 1, SelectedProvince.bounds.bottom + 1);
                    GL.Vertex2(SelectedProvince.bounds.left, SelectedProvince.bounds.bottom + 1);
                    GL.End();
                }

                GL.Color4(0f, 0f, 1f, 1f);
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
                    foreach (Value2S vertex in border.pixels)
                    {
                        GL.Vertex2(vertex.x, vertex.y);
                    }
                    GL.End();
                }
            }
        }


        private static void HandleDelete() { }

        private static void HandleEscape() => DeselectProvinces();

        public static void SelectProvinces(ushort[] ids)
        {
            GroupSelectedProvinces.Clear();

            if (ids.Length == 1)
            {
                if (TryGetProvince(ids[0], out var province))
                    SelectedProvince = province;
            }

            foreach (var id in ids)
            {
                if (!TryGetProvince(id, out var province))
                    continue;

                GroupSelectedProvinces.Add(province);
            }
        }

        public static void DeselectProvinces()
        {
            SelectedProvince = null;
            RMBProvince = null;
            GroupSelectedProvinces.Clear();
        }

        public static Province SelectProvince(int color)
        {
            if (_provincesByColor.TryGetValue(color, out var province))
            {
                if (MainForm.Instance.IsShiftPressed())
                {
                    if (SelectedProvince != null)
                        GroupSelectedProvinces.Add(SelectedProvince);

                    if (GroupSelectedProvinces.Contains(province))
                        GroupSelectedProvinces.Remove(province);
                    else
                        GroupSelectedProvinces.Add(province);

                    SelectedProvince = null;
                    return province;
                }
                else
                {
                    GroupSelectedProvinces.Clear();
                    SelectedProvince = province;
                }
            }
            else
            {
                SelectedProvince = null;
                GroupSelectedProvinces.Clear();
            }

            StateManager.DeselectStates();
            StrategicRegionManager.DeselectRegions();
            return SelectedProvince;
        }

        public static Province SelectRMBProvince(int color)
        {
            RMBProvince = _provincesByColor[color];
            StateManager.DeselectStates();
            StrategicRegionManager.DeselectRegions();
            return SelectedProvince;
        }

        private static void ProcessDefinitionFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Trim().Length == 0) continue;
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

                    ushort id;
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

                    if (_provincesById[id] != null)
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
                    catch (Exception)
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
                    _provincesById[id] = province;
                    _provincesByColor.Add(color, province);
                }
                catch (Exception)
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

            int color, prevColor = 0;

            Province province = null;

            foreach (var p in _provincesById)
            {
                if (p != null)
                    p.ResetPixels();
            }

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
                        _provincesById[NextVacantProvinceId] = province;
                        _provincesByColor.Add(color, province);
                        NextVacantProvinceId++;
                        while (_provincesById[NextVacantProvinceId] != null)
                            NextVacantProvinceId++;

                        Logger.LogSingleErrorMessage(
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
                int stateId = state.Id.GetValue();
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

        public static void GetMinMaxVictoryPoints(out uint min, out uint max)
        {
            min = 0;
            max = 0;

            foreach (var province in _provincesById)
            {
                if (province == null)
                    continue;

                min = province.victoryPoints;
                max = province.victoryPoints;
                break;
            }

            foreach (var province in _provincesById)
            {
                if (province == null)
                    continue;

                if (province.victoryPoints > max)
                    max = province.victoryPoints;
                else if (province.victoryPoints < min)
                    min = province.victoryPoints;
            }
        }
    }

}
