using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.classes;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static HOI4ModBuilder.src.tools.autotools.AutoToolRegenerateProvincesColors;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion
{
    class StrategicRegionManager
    {
        public static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map", "strategicregions" });

        private static Dictionary<ushort, StrategicRegion> _regions = new Dictionary<ushort, StrategicRegion>();
        public static void ForEachRegion(Action<StrategicRegion> action)
        {
            foreach (var r in _regions.Values)
                action(r);
        }
        private static HashSet<ProvinceBorder> _regionsBorders = new HashSet<ProvinceBorder>();

        public static HashSet<StrategicRegion> GroupSelectedRegions { get; private set; } = new HashSet<StrategicRegion>();
        public static Point2F GetGroupSelectedRegionsCenter()
        {
            var commonCenter = new CommonCenter();
            foreach (var obj in GroupSelectedRegions)
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
            DeselectRegions();

            _regions = new Dictionary<ushort, StrategicRegion>();
            _regionsBorders = new HashSet<ProvinceBorder>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            foreach (FileInfo fileInfo in fileInfoPairs.Values)
                LoadFile(fileInfo);
        }

        public static void LoadFile(FileInfo fileInfo)
        {
            using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                ParadoxParser.Parse(fs, new StrategicRegionFile(false, fileInfo, _regions));
        }

        public static void Save(BaseSettings settings)
        {
            var sb = new StringBuilder();
            foreach (var region in _regions.Values)
            {
                try
                {
                    if (!region.needToSave)
                        continue;

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

            if (GroupSelectedRegions != null && GroupSelectedRegions.Count > 0)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(8f);

                foreach (var region in GroupSelectedRegions)
                {
                    foreach (var border in region.Borders)
                    {
                        if (border.pixels.Length == 1)
                            continue;

                        if (
                            border.provinceA.Region != null &&
                            border.provinceA.Region.Id == region.Id &&
                            GroupSelectedRegions.Contains(border.provinceB.Region)
                            ||
                            border.provinceB.Region != null &&
                            border.provinceB.Region.Id == region.Id &&
                            GroupSelectedRegions.Contains(border.provinceA.Region))
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
        public static void RegenerateRegionsColors()
        {
            var random = new Random(0);
            var modSettings = SettingsManager.Settings.GetModSettings();

            var newColors = new HashSet<int>(256);
            var adjacentColors = new List<int>(16);

            var regeneratedRegionToColor = new Dictionary<StrategicRegion, int>(256);
            var hsvRanges = modSettings.GetRegionHSVRanges();

            var progressCallback = new ProgressCallback(EnumLocKey.AUTOTOOL_REGENERATE_PROVINCES_COLORS_REGIONS);

            var regions = AssembleRegions(GetRegions());
            int counter = 0;
            foreach (var region in regions)
            {
                counter++;
                progressCallback.Execute(counter, regions.Count);

                adjacentColors.Clear();

                int newColor;
                foreach (var borderRegion in region.GetBorderRegions())
                {
                    if (!regeneratedRegionToColor.TryGetValue(borderRegion, out newColor))
                        continue;
                    adjacentColors.Add(newColor);
                }

                newColor = ColorUtils.GenerateDistinctColor(
                    random, adjacentColors, hsvRanges, c => !newColors.Contains(c)
                );

                regeneratedRegionToColor[region] = newColor;
                region.color = newColor;
                newColors.Add(newColor);
            }
        }

        public static void AddRegionsBorder(ProvinceBorder border) => _regionsBorders.Add(border);

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

        public static bool TryAddRegion(StrategicRegion region)
        {
            if (!_regions.ContainsKey(region.Id))
            {
                _regions[region.Id] = region;
                region.needToSave = true;
                return true;
            }
            else return false;
        }

        public static bool ContainsRegionIdKey(ushort id) => _regions.ContainsKey(id);
        public static Dictionary<ushort, StrategicRegion>.KeyCollection GetRegionsIds() => _regions.Keys;
        public static List<ushort> GetRegionsIdsSorted()
        {
            var list = new List<ushort>(GetRegionsIds());
            list.Sort();
            return list;
        }
        public static Dictionary<ushort, StrategicRegion>.ValueCollection GetRegions() => _regions.Values;
        public static bool TryGetRegion(ushort id, out StrategicRegion region)
            => _regions.TryGetValue(id, out region);

        public static void AddRegion(ushort id, StrategicRegion region)
        {
            _regions[id] = region;
            region.needToSave = true;
        }

        public static void RemoveRegion(ushort id)
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


        public static void InitRegionsBorders()
        {
            _regionsBorders = new HashSet<ProvinceBorder>();
            foreach (var region in _regions.Values)
                region.InitBorders();
            TextureManager.InitRegionsBordersMap(_regionsBorders);

            var stopwatch = Stopwatch.StartNew();
            RegenerateRegionsColors();
            Logger.Log($"RegenerateRegionsColors = {stopwatch.ElapsedMilliseconds} ms");
        }

        private static void HandleDelete()
        {

        }

        private static void HandleEscape() => DeselectRegions();

        public static void DeselectRegions()
        {
            GroupSelectedRegions.Clear();
            SelectedRegion = null;
            RMBRegion = null;
        }


        public static void SelectRegions(ushort[] ids)
        {
            GroupSelectedRegions.Clear();

            if (ids.Length == 1)
            {
                if (TryGetRegion(ids[0], out var region))
                    SelectedRegion = region;
            }

            foreach (var id in ids)
            {
                if (!TryGetRegion(id, out var region))
                    continue;

                GroupSelectedRegions.Add(region);
            }
        }

        public static StrategicRegion SelectRegion(int color)
        {
            if (ProvinceManager.TryGetProvince(color, out Province province) && province.Region != null)
            {
                if (MainForm.Instance.IsShiftPressed())
                {
                    if (SelectedRegion != null)
                        GroupSelectedRegions.Add(SelectedRegion);

                    if (GroupSelectedRegions.Contains(province.Region))
                        GroupSelectedRegions.Remove(province.Region);
                    else
                        GroupSelectedRegions.Add(province.Region);

                    SelectedRegion = null;
                    return province.Region;
                }
                else
                {
                    GroupSelectedRegions.Clear();
                    SelectedRegion = province.Region;
                }
            }
            else
            {
                SelectedRegion = null;
                GroupSelectedRegions.Clear();
            }
            ProvinceManager.DeselectProvinces();
            StateManager.DeselectStates();
            return SelectedRegion;
        }

        public static StrategicRegion SelectRMBRegion(int color)
        {
            if (ProvinceManager.TryGetProvince(color, out Province province) && province.Region != null)
            {
                RMBRegion = province.Region;
            }
            else RMBRegion = null;
            ProvinceManager.DeselectProvinces();
            StateManager.DeselectStates();
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
