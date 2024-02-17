using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.managers
{
    class ErrorManager
    {
        private static Dictionary<Point2F, ulong> _poses = new Dictionary<Point2F, ulong>(0);
        private static HashSet<EnumMapErrorCode> _enabledErrorCodes = new HashSet<EnumMapErrorCode>();

        public static void Init(Settings settings)
        {
            _poses = new Dictionary<Point2F, ulong>(0);
            InitFilters();

            Tuple<EnumLocKey, Action>[] actions =
            {
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesWrongColors()),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesXCrosses()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckDividedProvinces()),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesTerrains()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesContinents()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesCoastalMismatches()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesBordersMismatches()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckLandProvincesWithNoStates()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesWithNoRegion()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesWithMultiStates()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesWithMultiRegions()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckProvincesMultiVictoryPoints()),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckStatesMultiRegions()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckRegionWithNotNavalTerrain()),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckRailways()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckSupplyHubsMismatches()),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckFrontlinePossibleErrors()),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckHeightMapMismatches(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING, () => CheckRiversMismatches(settings))
            };

            MainForm.ExecuteActions(actions);
        }

        public static void Draw()
        {
            //TODO Оптимизировать: отправлять на видеокарту все точки за один вызов, а не каждую по отдельности
            GL.Color3(0f, 0f, 1f);
            GL.PointSize(12f);
            GL.Begin(PrimitiveType.Points);
            foreach (Point2F p in _poses.Keys) GL.Vertex2(p.x, p.y);
            GL.End();

            GL.Color3(1f, 0f, 0f);
            GL.PointSize(8f);
            GL.Begin(PrimitiveType.Points);
            foreach (Point2F p in _poses.Keys) GL.Vertex2(p.x, p.y);
            GL.End();
        }

        private static void InitFilters()
        {
            _enabledErrorCodes.Clear();

            var errorsFilter = SettingsManager.settings.GetErrorsFilter();
            if (errorsFilter == null) return;

            foreach (EnumMapErrorCode enumObj in Enum.GetValues(typeof(EnumMapErrorCode)))
            {
                if (errorsFilter.Contains(enumObj.ToString())) _enabledErrorCodes.Add(enumObj);
            }
        }

        private static bool CheckFilter(EnumMapErrorCode code) => _enabledErrorCodes.Contains(code);

        public static List<EnumMapErrorCode> GetErrorCodes(Point2D pos, double distance)
        {
            var codes = new List<EnumMapErrorCode>();

            foreach (var p in _poses.Keys)
            {
                if (p.GetDistanceTo(pos) <= distance)
                {
                    ulong value = _poses[p];

                    foreach (EnumMapErrorCode code in Enum.GetValues(typeof(EnumMapErrorCode)))
                    {
                        if ((value & (1uL << (int)code)) != 0) codes.Add(code);
                    }
                }
            }

            return codes;
        }

        private static void AddErrorInfo(float x, float y, EnumMapErrorCode code) => AddErrorInfo(new Point2F(x, y), code);

        private static void AddErrorInfo(Point2F pos, EnumMapErrorCode code)
        {
            _poses.TryGetValue(pos, out ulong errorCode);
            errorCode |= (1uL << (int)code);
            _poses[pos] = errorCode;
        }

        private static void CheckProvincesXCrosses()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_X_CROSS)) return;

            int[] pixels = MapManager.ProvincesPixels;
            for (int x = 0; x < MapManager.MapSize.x - 1; x++)
            {
                for (int y = 0; y < MapManager.MapSize.y - 1; y++)
                {
                    int lu = (x + y * MapManager.MapSize.x);
                    int ru = lu + 1;
                    int ld = lu + MapManager.MapSize.x;
                    int rd = ld + 1;

                    if (
                        (pixels[lu] != pixels[ru]) && //lu != ru
                        (pixels[lu] != pixels[ld]) && //lu != ld
                        (pixels[ld] != pixels[rd]) && //ld != rd
                        (pixels[rd] != pixels[ru])    //rd != ru
                    ) AddErrorInfo(x + 1f, y + 1f, EnumMapErrorCode.PROVINCE_X_CROSS);
                }
            }
        }

        private static void CheckDividedProvinces()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_DIVIDED)) return;

            int width = MapManager.MapSize.x;
            int height = MapManager.MapSize.y;
            int x, y;

            bool[] usedProvinces = new bool[ushort.MaxValue];
            bool[] usedPixels = new bool[width * height];
            int[] values = MapManager.ProvincesPixels;
            int pixelCount = values.Length;

            for (int i = 0; i < pixelCount; i++)
            {
                if (!usedPixels[i])
                {
                    UseProvinceRegionPixels(ref usedPixels, i);
                    if (ProvinceManager.TryGetProvince(values[i], out Province p))
                    {
                        ushort id = p.Id;
                        if (usedProvinces[id])
                        {
                            x = i % width;
                            y = i / width;
                            AddErrorInfo(x + 0.5f, y + 0.5f, EnumMapErrorCode.PROVINCE_DIVIDED);
                        }
                        else usedProvinces[id] = true;
                    }
                }
            }
        }


        private static void UseProvinceRegionPixels(ref bool[] usedPixels, int i)
        {
            var nextPoses = new Queue<int>();
            int width = MapManager.MapSize.x;
            int height = MapManager.MapSize.y;
            int x, y;

            int color = MapManager.ProvincesPixels[i];
            nextPoses.Enqueue(i);

            int[] values = MapManager.ProvincesPixels;

            while (nextPoses.Count > 0)
            {
                i = nextPoses.Dequeue();
                x = i % width;
                y = i / width;

                //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
                if (x < 0 || y < 0 || x >= width || y >= height || usedPixels[i]) continue;

                i = x + y * width;

                if (values[i] == color)
                {
                    usedPixels[i] = true;

                    nextPoses.Enqueue(i - 1);
                    nextPoses.Enqueue(i + width);
                    nextPoses.Enqueue(i + 1);
                    nextPoses.Enqueue(i - width);
                }
            }
        }

        private static void CheckProvincesWrongColors()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_WRONG_COLOR)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                var color = new Color3B(p.Color);

                int max = color.red;
                if (color.green > max) max = color.green;
                if (color.blue > max) max = color.blue;

                int sum = color.red + color.green + color.blue;

                //TODO Переделать: добавить поддержку кастомных условий
                //land
                if (p.TypeId == 0 && sum < 340)
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_WRONG_COLOR);
                //sea, lake
                else if (p.TypeId != 0 && (sum > 339 || max > 127))
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_WRONG_COLOR);
            }
        }

        private static void CheckProvincesTerrains()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_WITH_NO_TERRAIN)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                var terrain = p.Terrain;
                if (terrain == null || terrain.name == "unknown")
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_WITH_NO_TERRAIN);
            }
        }

        private static void CheckProvincesContinents()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_CONTINENT_ID_NOT_EXISTS)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                if (p.ContinentId > ContinentManager.GetContinentsCount())
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_CONTINENT_ID_NOT_EXISTS);
            }
        }

        private static void CheckProvincesCoastalMismatches()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_COASTAL_MISMATCH)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                if (p.IsCoastal != p.CheckCoastalType())
                {
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_COASTAL_MISMATCH);
                }
            }
        }

        private static void CheckProvincesBordersMismatches()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_BORDERS_MISMATCH)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                if (p.TypeId == 2 && p.HasBorderWithTypeId(1))
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_BORDERS_MISMATCH);
            }
        }

        private static void CheckLandProvincesWithNoStates()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_LAND_WITH_NO_STATE)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                if (p.TypeId == 0 && p.State == null)
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_LAND_WITH_NO_STATE);
            }
        }

        private static void CheckProvincesWithNoRegion()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_WITH_NO_REGION)) return;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                if (p.Region == null)
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_WITH_NO_REGION);
            }
        }

        private static void CheckProvincesWithMultiStates()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_MULTI_STATES)) return;

            foreach (var s in StateManager.GetStates())
            {
                foreach (var p in s.provinces)
                {
                    if (p.State.Id != s.Id)
                        AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_MULTI_STATES);
                }
            }
        }

        private static void CheckProvincesWithMultiRegions()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_MULTI_REGIONS)) return;

            Action<StrategicRegion, Province> action = (r, p) =>
            {
                if (p.Region != null && p.Region.Id != r.Id)
                    AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_MULTI_REGIONS);
            };

            foreach (var r in StrategicRegionManager.GetRegions())
                r.ForEachProvince(action);
        }

        private static void CheckProvincesMultiVictoryPoints()
        {
            if (!CheckFilter(EnumMapErrorCode.PROVINCE_MULTI_VICTORY_POINTS)) return;

            var usedIds = new HashSet<ushort>(0);
            foreach (var s in StateManager.GetStates())
            {
                foreach (var p in s.victoryPoints.Keys)
                {
                    if (usedIds.Contains(p.Id)) AddErrorInfo(p.center, EnumMapErrorCode.PROVINCE_MULTI_VICTORY_POINTS);
                    else usedIds.Add(p.Id);
                }
            }
        }

        private static void CheckStatesMultiRegions()
        {
            if (!CheckFilter(EnumMapErrorCode.STATE_MULTI_REGIONS)) return;

            foreach (var s in StateManager.GetStates())
            {
                int regionId = -1;
                foreach (var p in s.provinces)
                {
                    if (p.Region == null) continue;
                    if (regionId == -1)
                    {
                        regionId = p.Region.Id;
                        continue;
                    }
                    if (p.Region.Id != regionId)
                    {
                        AddErrorInfo(s.center, EnumMapErrorCode.STATE_MULTI_REGIONS);
                        break;
                    }
                }
            }
        }

        private static void CheckRegionWithNotNavalTerrain()
        {
            if (!CheckFilter(EnumMapErrorCode.REGION_USES_NOT_NAVAL_TERRAIN)) return;

            foreach (var r in StrategicRegionManager.GetRegions())
            {
                if (r.Terrain == null) continue;
                if (!r.Terrain.isNavalTerrain) AddErrorInfo(r.center, EnumMapErrorCode.REGION_USES_NOT_NAVAL_TERRAIN);
            }
        }

        private static void CheckFrontlinePossibleErrors()
        {
            if (!CheckFilter(EnumMapErrorCode.FRONTLINE_POSSIBLE_ERROR)) return;

            var borderProvincesIds = new HashSet<ushort>();
            ushort borderProvinceId;

            foreach (var p in ProvinceManager.GetProvinces())
            {
                //Пропускаем не наземные провинции
                if (p.TypeId != 0) continue;

                foreach (var b in p.borders)
                {
                    //Если текущая провинция является провинцией A в границе
                    if (b.provinceA.Id == p.Id)
                    {
                        //Если соседняя провинция не land, то переходим к следующей границе
                        if (b.provinceB.TypeId != 0) continue;

                        //Получаем id соседней провинции
                        borderProvinceId = b.provinceB.Id;
                        //Если id соседней провинции уже есть в списке id соседних провинций
                        if (borderProvincesIds.Contains(borderProvinceId))
                        { //То добавляем ошибку и переходим к следующей провинции в списке
                            AddErrorInfo(p.center.x, p.center.y, EnumMapErrorCode.FRONTLINE_POSSIBLE_ERROR);
                            break;
                        } //Иначе соседней провинции нет в списке id соседних провинций, а значит её надо добавить
                        else borderProvincesIds.Add(borderProvinceId);
                    }
                    else //Иначе текущая провинция является провинцией B в границе
                    {
                        //Если соседняя провинция не land, то переходим к следующей границе
                        if (b.provinceA.TypeId != 0) continue;

                        //Получаем id соседней провинции
                        borderProvinceId = b.provinceA.Id;
                        //Если id соседней провинции уже есть в списке id соседних провинций
                        if (borderProvincesIds.Contains(borderProvinceId))
                        { //То добавляем ошибку и переходим к следующей провинции в списке
                            AddErrorInfo(p.center.x, p.center.y, EnumMapErrorCode.FRONTLINE_POSSIBLE_ERROR);
                            break;
                        } //Иначе соседней провинции нет в списке id соседних провинций, а значит её надо добавить
                        else borderProvincesIds.Add(borderProvinceId);
                    }
                }

                //При переходе к след. провинции очищаем список соседних провинций
                borderProvincesIds.Clear();
            }
        }

        private static void CheckHeightMapMismatches(Settings settings)
        {
            if (!CheckFilter(EnumMapErrorCode.HEIGHTMAP_MISMATCH)) return;

            int width = MapManager.MapSize.x;
            int pixelCount = width * MapManager.MapSize.y;
            byte waterLevel = (byte)(settings.GetWaterHeight() * 10);

            int color = 0;
            int[] provincesPixels = MapManager.ProvincesPixels;
            byte[] heightPixels = MapManager.HeightsPixels;
            Province p = null;

            for (int i = 0; i < pixelCount; i++)
            {
                if (color != provincesPixels[i])
                    ProvinceManager.TryGetProvince(color = provincesPixels[i], out p);

                byte height = heightPixels[i];
                if (p != null)
                {
                    byte typeId = p.TypeId;
                    float x = i % width + 0.5f;
                    float y = i / width + 0.5f;

                    if (typeId == 0 && height <= waterLevel || typeId != 0 && height >= waterLevel)
                        AddErrorInfo(x, y, EnumMapErrorCode.HEIGHTMAP_MISMATCH);
                }
            }

        }

        private static void CheckRiversMismatches(Settings settings)
        {

            var files = FileManager.ReadMultiFileInfos(settings, @"map\");
            if (!files.TryGetValue("rivers.bmp", out FileInfo fileInfo)) return;

            var riversBitmap = new Bitmap(fileInfo.filePath);
            byte[] riversPixels = Utils.BitmapToArray(riversBitmap, ImageLockMode.ReadOnly, TextureManager._8bppIndexed);
            int width = riversBitmap.Width;
            int height = riversBitmap.Height;

            riversBitmap.Dispose();

            byte[,] riversMap = new byte[width, height];

            var correctPixels = new HashSet<int>();
            var starts = new Queue<int>();

            byte pixel;
            //Выделяем позиции истоков рек и корректных речных пикселей и подготавливаем карту пикселей
            int x, y;
            int correctColorsCount = TextureManager.riverColors.Length;
            for (int i = 0; i < riversPixels.Length; i++)
            {
                pixel = riversPixels[i];
                x = i % width;
                y = i / width;
                riversMap[x, y] = pixel;

                if (pixel == 0)
                {
                    starts.Enqueue(i);
                    correctPixels.Add(i);
                }
                else if (pixel < correctColorsCount) correctPixels.Add(i);
            }

            if (CheckFilter(EnumMapErrorCode.RIVER_CROSSING))
            {
                for (int tx = 0; tx < (width - 1); tx++)
                {
                    for (int ty = 0; ty < (height - 1); ty++)
                    {
                        if (
                            riversMap[tx, ty] < correctColorsCount && riversMap[tx + 1, ty + 1] < correctColorsCount && riversMap[tx + 1, ty] >= correctColorsCount && riversMap[tx, ty + 1] >= correctColorsCount ||
                            riversMap[tx, ty] >= correctColorsCount && riversMap[tx + 1, ty + 1] >= correctColorsCount && riversMap[tx + 1, ty] < correctColorsCount && riversMap[tx, ty + 1] < correctColorsCount
                        ) AddErrorInfo(tx + 1f, ty + 1f, EnumMapErrorCode.RIVER_CROSSING);
                    }
                }
            }

            var flows = new Queue<int>(); //Входящие и выходящие соединения (индексы = 1, 2)
            //Проверяем цепочки рек на корректность

            //Начинаем с зелёных пикселей
            foreach (int index in starts) TraceRiver(riversMap, correctPixels, flows, correctColorsCount, width, height, index);

            //Идём по всем доп участкам рек
            while (flows.Count > 0)
                TraceRiver(riversMap, correctPixels, flows, correctColorsCount, width, height, flows.Dequeue());

            //Добавляем ошибки на все неиспользованные пиксели
            if (CheckFilter(EnumMapErrorCode.RIVER_NO_START_POINT))
            {
                foreach (int index in correctPixels)
                    AddErrorInfo(index % width + 0.5f, index / width + 0.5f, EnumMapErrorCode.RIVER_NO_START_POINT);
            }
        }

        private static void TraceRiver(in byte[,] riversMap, in HashSet<int> correctPixels, in Queue<int> flows, int correctColorsCount, int width, int height, int startIndex)
        {
            int x = startIndex % width;
            int y = startIndex / width;

            if (!correctPixels.Contains(startIndex) && CheckFilter(EnumMapErrorCode.RIVER_START_POINT_ERROR))
            {
                AddErrorInfo(x + 0.5f, y + 0.5f, EnumMapErrorCode.RIVER_START_POINT_ERROR);
                return;
            }
            else correctPixels.Remove(startIndex);

            byte bordersCount = 1;
            while (bordersCount != 0)
            {
                TraceNextRiverPixel(
                    riversMap, correctPixels, flows, correctColorsCount, x, y, width, height,
                    out x, out y, out startIndex, out bordersCount, out bool hasFlowInOrOut
                );

                if (hasFlowInOrOut && bordersCount == 0 && CheckFilter(EnumMapErrorCode.RIVER_FLOW_IN_OR_OUT_ERROR))
                    AddErrorInfo(x + 0.5f, y + 0.5f, EnumMapErrorCode.RIVER_FLOW_IN_OR_OUT_ERROR);
            }

            //Если у истока реки нет продолжения или их несколько
            if (bordersCount > 1 && CheckFilter(EnumMapErrorCode.RIVER_START_POINT_ERROR))
            {
                AddErrorInfo(x + 0.5f, y + 0.5f, EnumMapErrorCode.RIVER_START_POINT_ERROR);
                return;
            }
        }

        private static void TraceNextRiverPixel(in byte[,] riversMap, in HashSet<int> correctPixels, in Queue<int> flows, int correctColorsCount, int x, int y, int width, int height, out int nextX, out int nextY, out int nextIndex, out byte bordersCount, out bool hasFlowInOrOut)
        {
            bordersCount = 0;
            hasFlowInOrOut = false;
            int newX;
            nextX = x;
            nextY = y;
            nextIndex = 0;

            //Убираем текущий пиксель из списка неиспользованных пикселей
            correctPixels.Remove(y * width + x);

            if (GetCorrectRiverPosition(width, height, x, y - 1, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains((y - 1) * width + newX))
            {
                if (riversMap[newX, y - 1] == 1 || riversMap[newX, y - 1] == 2)
                {
                    flows.Enqueue((y - 1) * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y - 1;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
            if (GetCorrectRiverPosition(width, height, x + 1, y, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains(y * width + newX))
            {

                if (riversMap[newX, y] == 1 || riversMap[newX, y] == 2)
                {
                    flows.Enqueue(y * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
            if (bordersCount < 2 && GetCorrectRiverPosition(width, height, x, y + 1, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains((y + 1) * width + newX))
            {
                if (riversMap[newX, y + 1] == 1 || riversMap[newX, y + 1] == 2)
                {
                    flows.Enqueue((y + 1) * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y + 1;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
            if (bordersCount < 2 && GetCorrectRiverPosition(width, height, x - 1, y, out newX) && riversMap[newX, y] < correctColorsCount && correctPixels.Contains((y) * width + newX))
            {

                if (riversMap[newX, y] == 1 || riversMap[newX, y] == 2)
                {
                    flows.Enqueue(y * width + newX);
                    hasFlowInOrOut = true;
                }
                else
                {
                    nextX = newX;
                    nextY = y;
                    nextIndex = nextY * width + nextX;
                    bordersCount++;
                }
            }
        }

        private static bool GetCorrectRiverPosition(int width, int height, int x, int y, out int newX)
        {
            if (x < 0) newX = width + x;
            else if (x >= width) newX = x - width;
            else newX = x;

            if (y < 0 || y >= height) return false;
            else return true;
        }

        private static void CheckRailways()
        {
            var connections = new HashSet<int>();

            foreach (var railway in SupplyManager.Railways)
            {
                for (int i = 1; i < railway.ProvincesCount; i++)
                {
                    var p0 = railway.GetProvince(i - 1);
                    var p1 = railway.GetProvince(i);

                    if (CheckFilter(EnumMapErrorCode.RAILWAY_OVERLAP_CONNECTION) && (!(p0.HasBorderWith(p1) || p0.HasSeaConnectionWith(p1))))
                        AddErrorInfo(
                            p0.center.x + (p1.center.x - p0.center.x) / 2f,
                            p0.center.y + (p1.center.y - p0.center.y) / 2f,
                            EnumMapErrorCode.RAILWAY_PROVINCES_CONNECTION
                        );

                    ushort id0 = p0.Id;
                    ushort id1 = p1.Id;

                    int connection = id0 < id1 ? ((id0 << 16) | id1) : (id0 | (id1 << 16));

                    if (!connections.Contains(connection)) connections.Add(connection);
                    else if (CheckFilter(EnumMapErrorCode.RAILWAY_OVERLAP_CONNECTION))
                        AddErrorInfo(
                            p0.center.x + (p1.center.x - p0.center.x) / 2f,
                            p0.center.y + (p1.center.y - p0.center.y) / 2f,
                            EnumMapErrorCode.RAILWAY_OVERLAP_CONNECTION
                        );
                }
            }
        }

        private static void CheckSupplyHubsMismatches()
        {
            if (!CheckFilter(EnumMapErrorCode.SUPPLY_HUB_NO_CONNECTION)) return;

            foreach (var node in SupplyManager.SupplyNodes)
            {
                var p = node.GetProvince();
                if (p.GetRailwaysCount() == 0) AddErrorInfo(p.center.x, p.center.y, EnumMapErrorCode.SUPPLY_HUB_NO_CONNECTION);
            }
        }

    }

    public enum EnumMapErrorCode
    {
        PROVINCE_WRONG_COLOR, //Слишком тёмный цвет наземной или слишком яркий цвет водной провинции
        PROVINCE_X_CROSS, //4 граничащих пикселя имеют разные цвета
        PROVINCE_DIVIDED, //Провинция разделена на несколько отдельных областей
        PROVINCE_CONTINENT_ID_NOT_EXISTS, //
        PROVINCE_WITH_NO_TERRAIN, //
        PROVINCE_COASTAL_MISMATCH, //Некорректная прибрежность провинции
        PROVINCE_BORDERS_MISMATCH, //Некорректный тип соседних провинций
        PROVINCE_LAND_WITH_NO_STATE, //Наземная провинция без области
        PROVINCE_WITH_NO_REGION, //Провинция без страт. региона
        PROVINCE_MULTI_STATES, //Провинция находится в нескольких областях
        PROVINCE_MULTI_REGIONS, //Провинция находится в нескольких страт. регионах
        PROVINCE_MULTI_VICTORY_POINTS, //Провинция имеет несколько разных викторипоинтов

        STATE_MULTI_REGIONS, //Область находится в нескольких страт. регионах

        REGION_USES_NOT_NAVAL_TERRAIN, //У региона установлена неморская местность

        HEIGHTMAP_MISMATCH, //Некорректная высота точки провинции
        RIVER_CROSSING, //Некорректная точка пересечения речных участков
        RIVER_NO_START_POINT, //Отсутствие точки начала реки
        RIVER_START_POINT_ERROR, //Точка начала реки имеет некорректных пикселей-соседей
        RIVER_FLOW_IN_OR_OUT_ERROR, //
        RIVER_MULTI_START_POINTS, //Несколько стартовых точек у реки
        RAILWAY_PROVINCES_CONNECTION, //Железная дорога соединяет несоседствующие провинции
        RAILWAY_OVERLAP_CONNECTION, //Железная дорога проходит по пути, который уже прошла другая дорога 
        SUPPLY_HUB_NO_CONNECTION, //Узел снабжения не соединён с Ж/Д

        FRONTLINE_POSSIBLE_ERROR //Вероятная ошибка с линией фронта
    }
}
