using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.managers.texture;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.classes;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using Pdoxcl2Sharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.src.tools.autotools.AutoToolRegenerateProvincesColors;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion
{
    class StrategicRegionManager
    {
        public static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map", "strategicregions" });

        private static Dictionary<ushort, StrategicRegion> _regions = new Dictionary<ushort, StrategicRegion>();
        public static void ForEach(Action<StrategicRegion> action)
        {
            foreach (var r in _regions.Values)
                action(r);
        }
        private static HashSet<ProvinceBorder> _regionsBorders = new HashSet<ProvinceBorder>();

        public static HashSet<StrategicRegion> SelectedGroup { get; private set; } = new HashSet<StrategicRegion>();
        public static Point2F GetSelectedGroupCenter()
        {
            var commonCenter = new CommonCenter();
            foreach (var obj in SelectedGroup)
                commonCenter.Push(obj.pixelsCount, obj.center);
            commonCenter.Get(out var _, out var center);
            return center;
        }
        public static StrategicRegion SelectedRegion { get; set; }
        public static StrategicRegion RMBRegion { get; set; }

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(BaseSettings settings)
        {
            Deselect();

            _regions = new Dictionary<ushort, StrategicRegion>();
            _regionsBorders = new HashSet<ProvinceBorder>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            var actions = new List<(string, Action)>(fileInfoPairs.Count);
            var addActions = new ConcurrentQueue<Action>();

            foreach (var fileInfo in fileInfoPairs.Values)
                actions.Add((null, () =>
                {
                    LoadFile(fileInfo, out var addAction);
                    addActions.Enqueue(addAction);
                }
                ));

            MainForm.ExecuteActionsParallelNoDisplay(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_REGIONS, actions);
            foreach (var addAction in addActions)
                addAction();
            foreach (var region in _regions.Values)
                region.Validate(out var _);
        }

        public static void LoadFile(FileInfo fileInfo)
        {
            LoadFile(fileInfo, out var addAction);
            addAction?.Invoke();
        }

        public static void LoadFile(FileInfo fileInfo, out Action addAction)
        {
            var region = new StrategicRegionFile(false, fileInfo, _regions);
            using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                ParadoxParser.Parse(fs, region);
            addAction = region.addAction;
        }

        public static void Save(BaseSettings settings)
        {
            var actions = new List<(string, Action)>(_regions.Count);
            foreach (var region in _regions.Values)
                actions.Add((null, () => Save(settings, region)));
            ParallelUtils.Execute(actions);
        }

        public static void Save(BaseSettings settings, StrategicRegion region)
        {
            var sb = new StringBuilder();
            try
            {
                if (!region.needToSave)
                    return;

                if (string.IsNullOrEmpty(region.FileInfo.fileName))
                    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.ERROR_REGION_HAS_NO_FILE));

                region.Save(sb);
                File.WriteAllText(settings.modDirectory + FOLDER_PATH + region.FileInfo.fileName, sb.ToString());
                sb.Length = 0;
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_WHILE_REGION_SAVING,
                    new Dictionary<string, string> { { "{regionId}", $"{region.Id}" } }
                ), ex);
            }
        }

        public static void Draw(bool showCenters, bool showCollisions)
        {
            if (showCenters)
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                foreach (var region in _regions.Values)
                {
                    if (region.dislayCenter)
                        GL.Vertex2(region.center.x, region.center.y);
                }
                GL.End();
            }

            if (SelectedGroup != null && SelectedGroup.Count > 0)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(8f);

                foreach (var region in SelectedGroup)
                {
                    foreach (var border in region.Borders)
                    {
                        if (border.pixels.Length == 1)
                            continue;

                        if (
                            border.provinceA.Region != null &&
                            border.provinceA.Region.Id == region.Id &&
                            SelectedGroup.Contains(border.provinceB.Region)
                            ||
                            border.provinceB.Region != null &&
                            border.provinceB.Region.Id == region.Id &&
                            SelectedGroup.Contains(border.provinceA.Region))
                            continue;

                        GL.Begin(PrimitiveType.LineStrip);
                        foreach (Value2S vertex in border.pixels)
                            GL.Vertex2(vertex.x, vertex.y);
                        GL.End();
                    }
                }
            }

            if (SelectedRegion != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedRegion.Borders)
                {
                    if (border.pixels.Length == 1)
                        continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2S vertex in border.pixels)
                        GL.Vertex2(vertex.x, vertex.y);
                    GL.End();
                }

                if (showCollisions)
                {
                    GL.Color4(0f, 0f, 1f, 1f);
                    GL.LineWidth(3f);
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Vertex2(SelectedRegion.bounds.left, SelectedRegion.bounds.top);
                    GL.Vertex2(SelectedRegion.bounds.right + 1, SelectedRegion.bounds.top);
                    GL.Vertex2(SelectedRegion.bounds.right + 1, SelectedRegion.bounds.bottom + 1);
                    GL.Vertex2(SelectedRegion.bounds.left, SelectedRegion.bounds.bottom + 1);
                    GL.End();
                }
            }

            if (RMBRegion != null)
            {
                GL.Color4(1f, 0.42f, 0f, 1f);
                GL.LineWidth(2.5f);

                foreach (var border in RMBRegion.Borders)
                {
                    if (border.pixels.Length == 1)
                        continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2S vertex in border.pixels)
                        GL.Vertex2(vertex.x, vertex.y);
                    GL.End();
                }
            }
        }
        public static void RegenerateColors(ProgressCallback progressCallback)
        {
            var stopwatch = Stopwatch.StartNew();
            var modSettings = SettingsManager.Settings.GetModSettings();

            var newColors = new HashSet<int>(256);
            var adjacentColors = new List<int>(16);

            var regeneratedRegionToColor = new Dictionary<StrategicRegion, int>(256);
            var hsvRanges = modSettings.GetRegionHSVRanges();

            var regions = AssembleRegions(GetValues());
            int counter = 0;
            foreach (var region in regions)
            {
                counter++;
                progressCallback?.Execute(counter, regions.Count);

                adjacentColors.Clear();

                int newColor;
                foreach (var borderRegion in region.GetBorderRegions())
                {
                    if (!regeneratedRegionToColor.TryGetValue(borderRegion, out newColor))
                        continue;
                    adjacentColors.Add(newColor);
                }

                newColor = ColorUtils.GenerateDistinctColor(
                    new Random(region.Id), adjacentColors, hsvRanges, c => !newColors.Contains(c)
                );

                regeneratedRegionToColor[region] = newColor;
                region.color = newColor;
                newColors.Add(newColor);
            }
            Logger.Log($"RegenerateRegionsColors = {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void AddBorder(ProvinceBorder border) => _regionsBorders.Add(border);

        public static bool TransferProvince(Province province, StrategicRegion src, StrategicRegion dest)
        {
            //Если нет провинции или обоих регионов
            if (province == null || src == null && dest == null)
                return false;
            //Если оба региона являются одним и тем же регионом
            if (src != null && dest != null && src.Equals(dest))
                return false;
            //Если провинция уже в новом регионе
            if (province != null && dest != null && province.Region == dest)
                return false;

            src?.RemoveProvince(province);
            dest?.AddProvince(province);

            return true;
        }

        public static bool TryAdd(StrategicRegion region)
        {
            if (!_regions.ContainsKey(region.Id))
            {
                _regions[region.Id] = region;
                region.needToSave = true;
                return true;
            }
            else return false;
        }

        public static bool Contains(ushort id)
            => _regions.ContainsKey(id);
        public static Dictionary<ushort, StrategicRegion>.KeyCollection GetIDs()
            => _regions.Keys;
        public static List<ushort> GetIDsSorted()
        {
            var list = new List<ushort>(GetIDs());
            list.Sort();
            return list;
        }
        public static Dictionary<ushort, StrategicRegion>.ValueCollection GetValues()
            => _regions.Values;
        public static bool TryGet(ushort id, out StrategicRegion region)
            => _regions.TryGetValue(id, out region);
        public static StrategicRegion Get(ushort id)
            => TryGet(id, out var region) ? region : null;

        public static void Add(ushort id, StrategicRegion region)
        {
            _regions[id] = region;
            region.needToSave = true;
        }

        public static void Remove(ushort id)
        {
            var region = _regions[id];
            if (region != null)
            {
                _regions.Remove(id);
                region.needToSave = true;
            }
        }

        public static void CalculateCenters()
        {
            foreach (var region in _regions.Values)
                region.CalculateCenter();
        }


        public static void InitBorders()
        {
            _regionsBorders = new HashSet<ProvinceBorder>();
            foreach (var region in _regions.Values)
                region.InitBorders();
            TextureManager.InitRegionsBordersMap(_regionsBorders);
        }

        private static void HandleDelete()
        {

        }

        private static void HandleEscape() => Deselect();

        public static void Deselect()
        {
            SelectedGroup.Clear();
            SelectedRegion = null;
            RMBRegion = null;
        }


        public static void Select(ushort[] ids)
        {
            SelectedGroup.Clear();

            if (ids.Length == 1)
            {
                if (TryGet(ids[0], out var region))
                    SelectedRegion = region;
            }

            foreach (var id in ids)
            {
                if (!TryGet(id, out var region))
                    continue;

                SelectedGroup.Add(region);
            }
        }

        public static StrategicRegion Select(int color)
        {
            if (ProvinceManager.TryGet(color, out Province province) && province.Region != null)
            {
                if (MainForm.Instance.IsShiftPressed())
                {
                    if (SelectedRegion != null)
                        SelectedGroup.Add(SelectedRegion);

                    if (SelectedGroup.Contains(province.Region))
                        SelectedGroup.Remove(province.Region);
                    else
                        SelectedGroup.Add(province.Region);

                    SelectedRegion = null;
                    return province.Region;
                }
                else
                {
                    SelectedGroup.Clear();
                    SelectedRegion = province.Region;
                }
            }
            else
            {
                SelectedRegion = null;
                SelectedGroup.Clear();
            }
            ProvinceManager.Deselect();
            StateManager.Deselect();
            return SelectedRegion;
        }

        public static StrategicRegion SelectRMB(int color)
        {
            if (ProvinceManager.TryGet(color, out Province province) && province.Region != null)
            {
                RMBRegion = province.Region;
            }
            else RMBRegion = null;
            ProvinceManager.Deselect();
            StateManager.Deselect();
            return RMBRegion;
        }

        public static void RemoveProvinceData(Province province)
        {
            if (province == null)
                return;
            foreach (var region in _regions.Values)
                region.RemoveProvinceData(province);
        }
    }
}
