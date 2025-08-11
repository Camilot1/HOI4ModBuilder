using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.positions;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.map.buildings
{
    class MapPositionsManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map" });
        private static readonly string BUILDINGS_FILE_NAME = "buildings.txt";
        private static readonly string BUILDINGS_FILE_PATH = FileManager.AssembleFolderPath(new[] { FOLDER_PATH, BUILDINGS_FILE_NAME });
        private static readonly string UNITSTACKS_FILE_NAME = "unitstacks.txt";

        private static readonly Random _random = new Random();
        private static Dictionary<int, Info> _pixelsByProvincesIds = new Dictionary<int, Info>(0);

        private static Dictionary<string, BuildingInfo> _buildingsInfos = new Dictionary<string, BuildingInfo>(0);

        public static Dictionary<State, BuildingsPositionsData> statesPositions = new Dictionary<State, BuildingsPositionsData>(0);
        public static Dictionary<Province, BuildingsPositionsData> provincesPositions = new Dictionary<Province, BuildingsPositionsData>(0);

        private static Dictionary<Color3B, List<Point2F>> _renderPoints = new Dictionary<Color3B, List<Point2F>>(0);

        public static Dictionary<Point2F, List<BuildingErrorInfo>> statesErrors = new Dictionary<Point2F, List<BuildingErrorInfo>>(0);
        public static Dictionary<Point2F, List<BuildingErrorInfo>> provincesErrors = new Dictionary<Point2F, List<BuildingErrorInfo>>(0);

        //TODO Доделать
        public static void Load(Settings settings)
        {
            //InitPixels();
            //CalculateAndSaveBuildings(settings);
            //CalculateAndSaveUnitStacks(settings);
            Clear();
            //ParseBuildingsPoses(settings);
            //CheckBuildings(settings);
        }

        private static void InitPixels()
        {
            _pixelsByProvincesIds = new Dictionary<int, Info>(ProvinceManager.ProvincesCount);

            foreach (var p in ProvinceManager.GetProvinces())
            {
                _pixelsByProvincesIds[p.Color] = new Info(0, 0, 0, 0, 0, new Value2US[p.pixelsCount]);
            }

            int[] pixelColors = MapManager.ProvincesPixels;
            int index;
            ushort x, y;
            int width = MapManager.MapSize.x;
            int height = MapManager.MapSize.y;

            int pixelCount = pixelColors.Count();
            int color, prevColor = -1;
            Info info = null;

            for (int i = 0; i < pixelCount; i++)
            {
                color = pixelColors[i];
                if (prevColor != color)
                {
                    info = _pixelsByProvincesIds[color];
                    prevColor = color;
                }

                if (info != null)
                {
                    x = (ushort)(i % width);
                    y = (ushort)(i / width);

                    if (info.count == 0)
                    {
                        info.min.x = x;
                        info.min.y = y;
                        info.max.x = x;
                        info.max.y = y;
                    }
                    else
                    {
                        if (x > info.max.x) info.max.x = x;
                        else if (x < info.min.x) info.min.x = x;

                        if (y > info.max.y) info.max.y = y;
                        else if (y < info.min.y) info.min.y = y;
                    }

                    info.values[info.count] = new Value2US((ushort)(i % height), (ushort)(i / width));
                    info.count++;
                }
            }
        }

        public static void Clear()
        {
            _pixelsByProvincesIds = new Dictionary<int, Info>(0);
            _buildingsInfos = new Dictionary<string, BuildingInfo>(0);

            statesPositions = new Dictionary<State, BuildingsPositionsData>(0);
            provincesPositions = new Dictionary<Province, BuildingsPositionsData>(0);

            _renderPoints = new Dictionary<Color3B, List<Point2F>>(0);

            statesErrors = new Dictionary<Point2F, List<BuildingErrorInfo>>(0);
            provincesErrors = new Dictionary<Point2F, List<BuildingErrorInfo>>(0);
        }

        private static void CheckBuildings(Settings settings)
        {
            foreach (var building in BuildingManager.GetBuildings())
            {
                CheckBuilding(building);
            }
        }

        private static void CheckBuilding(Building building)
        {
            if (building == null)
                return;

            string name = building.Name;

            var spawnPoint = building.SpawnPoint.GetValue();
            if (spawnPoint.value != null)
                name = spawnPoint.value.name;

            var info = RequestBuildingInfo(name);

            foreach (var state in StateManager.GetStates())
            {
                if (info.isProvincial)
                {
                    foreach (var p in state.Provinces)
                    {
                        if (p.Type == EnumProvinceType.LAKE || info.isOnlyCoastal && !p.IsCoastal)
                            continue;

                        if (provincesPositions.TryGetValue(p, out var data))
                            data.Check(info, state.Id.GetValue(), p.Id);
                        else
                            BuildingsPositionsData.PushError(provincesErrors, p.center, new BuildingErrorInfo
                            {
                                buildingInfo = info,
                                error = EnumPositionError.NOT_PRESENT,
                                data = info.count
                            });
                    }
                }
                else
                {
                    if (statesPositions.TryGetValue(state, out var data))
                        data.Check(info, state.Id.GetValue(), 0);
                    else
                        BuildingsPositionsData.PushError(statesErrors, state.center, new BuildingErrorInfo
                        {
                            buildingInfo = info,
                            error = EnumPositionError.NOT_PRESENT,
                            data = info.count
                        });
                }
            }
        }

        public static void Draw()
        {
            /*
            foreach (var entry in _renderPoints)
            {
                var color = entry.Key;
                GL.Color4(color.red / 255f, color.green / 255f, color.blue / 255f, 1f);
                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                foreach (var pos in entry.Value)
                    GL.Vertex2(pos.x, MapManager.MapSize.y - pos.y);

                GL.End();
            }
            */

            GL.Color4(1f, 0f, 0f, 1f);
            GL.PointSize(10f);
            GL.Begin(PrimitiveType.Points);

            foreach (var entry in statesErrors)
                GL.Vertex2(entry.Key.x, entry.Key.y);

            GL.End();


            GL.Color4(0f, 0f, 1f, 1f);
            GL.PointSize(10f);
            GL.Begin(PrimitiveType.Points);

            foreach (var entry in provincesErrors)
                GL.Vertex2(entry.Key.x, entry.Key.y);

            GL.End();
        }


        public static List<string> GetWarningCodes(Point2D pos, double distance)
        {
            var output = new List<string>();

            foreach (var obj in statesErrors)
            {
                if (obj.Key.GetDistanceTo(pos) <= distance)
                {
                    foreach (var inner in obj.Value)
                    {
                        output.Add("STATE ERROR: " + inner.ToString());
                    }
                }
            }

            foreach (var obj in provincesErrors)
            {
                if (obj.Key.GetDistanceTo(pos) <= distance)
                {
                    foreach (var inner in obj.Value)
                    {
                        output.Add("PROVINCE ERROR: " + inner.ToString());
                    }
                }
            }

            return output;
        }

        private static void ParseBuildingsPoses(Settings settings)
        {
            var files = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            if (!files.TryGetValue(BUILDINGS_FILE_NAME, out var fileInfo))
            {
                Logger.LogWarning($"File {BUILDINGS_FILE_PATH} not found");
                return;
            }

            var lines = File.ReadAllLines(fileInfo.filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                var args = lines[i].Trim().Split(';');

                if (args.Length != 7)
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args count is not equal to 7");
                    continue;
                }

                int index = 0;
                if (!ushort.TryParse(args[index], out var stateId))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] is invalid ushort StateID value = {args[index]}");
                    continue;
                }

                if (!StateManager.TryGetState(stateId, out var state))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] Not found State with id = {stateId}");
                    continue;
                }

                index = 1;
                var name = args[index];

                var buildingInfo = RequestBuildingInfo(name);

                if (buildingInfo == null)
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] Not found Building/SpawnPoint with name = {name}");
                    continue;
                }


                index = 2;
                if (!Utils.TryParseFloat(args[index], out var x))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] is invalid float value x = {args[index]}");
                    continue;
                }

                index = 3;
                if (!Utils.TryParseFloat(args[index], out var y))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] is invalid float value y = {args[index]}");
                    continue;
                }

                index = 4;
                if (!Utils.TryParseFloat(args[index], out var z))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] is invalid float value z = {args[index]}");
                    continue;
                }

                int provinceColor;
                try
                {
                    provinceColor = MapManager.GetColor(x, MapManager.MapSize.y - z - 1);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Position {x} {z} is out of map bounds: " + ex.Message);
                    continue;
                }

                if (!ProvinceManager.TryGetProvince(provinceColor, out var province))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] Not found Province with Color = {provinceColor}");
                    continue;
                }

                index = 5;
                if (!Utils.TryParseFloat(args[index], out var rotation))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] is invalid float value rotation = {args[index]}");
                    continue;
                }

                index = 6;
                if (!ushort.TryParse(args[index], out var adjacentProvinceId))
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] is invalid adjacent province id value = {args[index]}");
                    continue;
                }

                Province adjacentProvince = null;
                if (buildingInfo.shouldHaveAdjacentProvince && buildingInfo.isOnlyCoastal)
                {
                    if (!ProvinceManager.TryGetProvince(adjacentProvinceId, out adjacentProvince))
                    {
                        Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Args[{index}] Not found Province with id = {adjacentProvinceId}");
                        continue;
                    }

                    if (!province.HasBorderWith(adjacentProvince))
                    {
                        Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Building province id = {province.Id} is not adjacent with province id = {adjacentProvinceId}");
                        continue;
                    }

                    if (name == "floating_harbor" && (province.Type != EnumProvinceType.SEA || adjacentProvince.Type != EnumProvinceType.LAND) ||
                        name != "floating_harbor" && (province.Type == EnumProvinceType.SEA || province.Type == EnumProvinceType.LAKE || adjacentProvince.Type != EnumProvinceType.SEA))
                    {
                        Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Adjacent province with id = {adjacentProvinceId} has invalid Type = {adjacentProvince.Type}");
                        continue;
                    }
                }
                else if (buildingInfo.shouldHaveAdjacentProvince && adjacentProvinceId != 0)
                {
                    Logger.LogWarning($"{BUILDINGS_FILE_PATH}:{i} Building province id = {province.Id} should not have adjacent province id defined");
                    continue;
                }

                var buildingPosition = new BuildingPosition
                {
                    state = state,
                    x = x,
                    y = y,
                    z = z,
                    rotation = rotation,
                    adjacentProvince = adjacentProvince,

                    buildingInfo = buildingInfo,
                    province = province
                };

                if (buildingInfo.isProvincial)
                {
                    if (!provincesPositions.TryGetValue(province, out var provinceBuildings))
                    {
                        provinceBuildings = new BuildingsPositionsData();
                        provincesPositions[province] = provinceBuildings;
                    }
                    provinceBuildings.Push(buildingPosition);
                }
                else
                {
                    if (!statesPositions.TryGetValue(state, out var statePositions))
                    {
                        statePositions = new BuildingsPositionsData();
                        statesPositions[state] = statePositions;
                    }
                    statePositions.Push(buildingPosition);
                }

                if (!_renderPoints.TryGetValue(buildingInfo.color, out var renderPoses))
                {
                    renderPoses = new List<Point2F>();
                    _renderPoints[buildingInfo.color] = renderPoses;
                }
                renderPoses.Add(new Point2F(x, z));
            }
        }

        private static BuildingInfo RequestBuildingInfo(string name)
        {
            if (_buildingsInfos.TryGetValue(name, out var buildingInfo))
                return buildingInfo;

            if (BuildingManager.TryGetBuilding(name, out var building))
            {
                buildingInfo = new BuildingInfo
                {
                    name = name,
                    origin = building,
                    isProvincial = building.LevelCap.GetValue()?.GetSlotCategory() == EnumBuildingSlotCategory.PROVINCIAL,
                    isOnlyCoastal = building.IsOnlyCoastal.GetValue(),
                    count = building.ShowOnMapCount.GetValue(),
                };
            }
            else if (BuildingManager.TryGetSpawnPoint(name, out var spawnPoint))
            {
                buildingInfo = new BuildingInfo
                {
                    name = name,
                    origin = spawnPoint,
                    isProvincial = spawnPoint.Type.GetValue().stringValue == "province",
                    isOnlyCoastal = spawnPoint.OnlyCoastal.GetValue(),
                    shouldHaveAdjacentProvince = name == "naval_base_spawn",
                    count = spawnPoint.Max.GetValue(),
                };
            }
            else if (name == "floating_harbor")
            {
                buildingInfo = new BuildingInfo
                {
                    name = name,
                    origin = "floating_harbor",
                    isProvincial = true,
                    isOnlyCoastal = true,
                    shouldHaveAdjacentProvince = true,
                    count = 1
                };
            }
            else return null;

            var random = new Random(name.GetHashCode());
            buildingInfo.color = new Color3B(
                (byte)(random.NextDouble() * 256),
                (byte)(random.NextDouble() * 256),
                (byte)(random.NextDouble() * 256)
            );

            _buildingsInfos[name] = buildingInfo;

            return buildingInfo;
        }

        private static void CalculateAndSaveBuildings(Settings settings)
        {

        }

        private static void CalculateAndSaveUnitStacks(Settings settings)
        {
            var lines = new List<UnitStackLine>(0);

            foreach (var p in ProvinceManager.GetProvinces())
            {
                CalculateUnitStacks(lines, p);
            }

            lines.OrderBy(x => x.type).ThenBy(x => x.id);
            string[] exportLines = new string[lines.Count()];
            var sb = new StringBuilder();

            int count = lines.Count;
            UnitStackLine line;

            for (int i = 0; i < count; i++)
            {
                line = lines[i];
                sb.Append(line.id).Append(';')
                    .Append(line.type).Append(';')
                    .Append(line.x.ToString("#.##")).Append(';')
                    .Append(line.y.ToString("#.##")).Append(';')
                    .Append(line.z.ToString("#.##")).Append(';')
                    .Append(line.radians.ToString("#.##")).Append(';')
                    .Append(line.offset.ToString("#.##")).Append(Constants.NEW_LINE);
                exportLines[i] = sb.ToString();
                sb.Length = 0;
            }

            File.WriteAllLines(settings.modDirectory + FOLDER_PATH + UNITSTACKS_FILE_NAME, exportLines);
            lines.Clear();
        }

        private static void CalculateUnitStacks(List<UnitStackLine> list, Province p)
        {
            ushort id = p.Id;
            EnumProvinceType type = p.Type;
            float x = p.center.x;
            float y = p.center.y;
            float radians = 0;
            float offset = GetOffset();

            //standstill #0
            list.Add(new UnitStackLine(id, 0, x, GetY(x, y), y, radians, offset));
            //standstill RG #21
            x -= 3;
            list.Add(new UnitStackLine(id, 21, x, GetY(x, y), y, radians, offset));

            var borders = new List<ProvinceBorder>(p.borders);
            borders.OrderBy(o => o.center.y).ThenBy(o => o.center.x);

            if (type != EnumProvinceType.LAKE)
            {
                for (int i = 0; i < 8 && i < borders.Count; i++)
                {
                    var b = borders[i];
                    CalculateForUnit(p.center, b.center, EnumCalculateType.PERCENT, 0.5f, out x, out y, out radians);
                    //moving #1-8
                    list.Add(new UnitStackLine(id, (byte)(1 + i), x, GetY(x, y), y, radians, offset));
                    //moving RG #22-29
                    list.Add(new UnitStackLine(id, (byte)(22 + i), x, GetY(x, y), y, radians, offset));
                }
            }

            var info = _pixelsByProvincesIds[p.Color.GetHashCode()];
            float rValue = info.max.x - p.center.x;
            float lValue = p.center.x - info.min.x;
            float value = lValue < rValue ? lValue : rValue;

            //attacking #9
            x = p.center.x - 0.4f * value;
            y = p.center.y;
            list.Add(new UnitStackLine(id, 9, x, GetY(x, y), y, 1.57f, offset));

            //defending #10
            x = p.center.x + 0.4f * value;
            list.Add(new UnitStackLine(id, 10, x, GetY(x, y), y, -1.57f, offset));

            if (type == EnumProvinceType.SEA) //Если sea
            {
                //disembark
                //ship in port
                //ship in port moving
            }

            //victory point #38
            x = p.center.x + 0.4f * value;
            list.Add(new UnitStackLine(id, 10, x, GetY(x, y), y, -1.57f, offset));

        }

        private static float GetY(float x, float y)
            => MapManager.HeightsPixels[(int)y * MapManager.MapSize.x + (int)x];

        private static void CalculateForUnit(Point2F start, Point2F finish, EnumCalculateType type, float value, out float x, out float y, out float radians)
        {
            float x1 = finish.x - start.x;
            float y1 = finish.y - start.y;
            radians = (float)(Math.Acos(y1 / (Math.Sqrt(x1 * x1 + y1 * y1))));

            if (type == EnumCalculateType.PERCENT)
            {
                x = start.x + value * x1;
                y = start.y + value * y1;
            }
            else if (type == EnumCalculateType.PX_FROM_START)
            {
                x = (float)(value * Math.Sin(radians));
                y = (float)(value * Math.Cos(radians));
            }
            else
            {
                x = (float)(finish.x - value * Math.Sin(radians));
                y = (float)(finish.y - value * Math.Cos(radians));
            }
        }

        private static float GetOffset()
        {
            return 0.02f * _random.Next(127);
        }

        enum EnumCalculateType
        {
            PERCENT,
            PX_FROM_START,
            PX_FROM_END
        }
    }

    public class Info
    {
        public int count;
        public Value2US min, max;
        public Value2US[] values;

        public Info(int count, ushort minX, ushort minY, ushort maxX, ushort maxY, Value2US[] values)
        {
            this.count = count;
            min.x = minX;
            min.y = minY;
            max.x = maxX;
            max.y = maxY;
            this.values = values;
        }
    }


    class UnitStackLine
    {
        public ushort id;
        public byte type;
        public float x, y, z;
        public float radians;
        public float offset;

        public UnitStackLine(ushort id, byte type, float x, float y, float z, float radians, float offset)
        {
            this.id = id;
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = z;
            this.radians = radians;
            this.offset = offset;
        }
    }
}
