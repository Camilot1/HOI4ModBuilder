using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion
{
    class StrategicRegionManager
    {
        public static StrategicRegionManager Instance { get; private set; }

        private static Dictionary<ushort, StrategicRegion> _regions = new Dictionary<ushort, StrategicRegion>();
        private static HashSet<ProvinceBorder> _regionsBorders = new HashSet<ProvinceBorder>();

        public static StrategicRegion SelectedRegion { get; set; }
        public static StrategicRegion RMBRegion { get; set; }

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            Instance = new StrategicRegionManager();

            _regions = new Dictionary<ushort, StrategicRegion>();
            _regionsBorders = new HashSet<ProvinceBorder>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"map\strategicregions\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new StrategicRegionFile(false, fileInfo, _regions));
            }
        }

        public static void Save(Settings settings)
        {
            var sb = new StringBuilder();
            foreach (var region in _regions.Values)
            {
                try
                {
                    if (!region.needToSave) continue;

                    if (string.IsNullOrEmpty(region.fileName))
                        throw new Exception(GuiLocManager.GetLoc(EnumLocKey.ERROR_REGION_HAS_NO_FILE));

                    region.Save(sb);
                    File.WriteAllText(settings.modDirectory + @"map\strategicregions\" + region.fileName, sb.ToString());
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

        public static void Draw(bool showCenters)
        {
            if (showCenters)
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                foreach (var region in _regions.Values)
                {
                    if (region.dislayCenter) GL.Vertex2(region.center.x, region.center.y);
                }
                GL.End();
            }


            if (SelectedRegion != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedRegion.Borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2US vertex in border.pixels)
                        GL.Vertex2(vertex.x, vertex.y);
                    GL.End();
                }
            }

            if (RMBRegion != null)
            {
                GL.Color4(1f, 0.42f, 0f, 1f);
                GL.LineWidth(2.5f);

                foreach (var border in RMBRegion.Borders)
                {
                    if (border.pixels.Length == 1) continue;
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Value2US vertex in border.pixels)
                        GL.Vertex2(vertex.x, vertex.y);
                    GL.End();
                }
            }
        }

        public static void AddRegionsBorder(ProvinceBorder border) => _regionsBorders.Add(border);

        public static bool TransferProvince(Province province, StrategicRegion src, StrategicRegion dest)
        {
            //Если нет провинции или обоих регионов
            if (province == null || src == null && dest == null) return false;
            //Если оба региона являются одним и тем же регионом
            if (src != null && dest != null && src.Equals(dest)) return false;
            //Если провинция уже в новом регионе
            if (province != null && dest != null && province.region == dest) return false;

            if (src != null) src.RemoveProvince(province);
            if (dest != null) dest.AddProvince(province);

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
        public static Dictionary<ushort, StrategicRegion>.ValueCollection GetRegions() => _regions.Values;
        public static bool GetRegion(ushort id, out StrategicRegion region)
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
            foreach (var region in _regions.Values) region.CalculateCenter();
        }


        public static void InitRegionsBorders()
        {
            _regionsBorders = new HashSet<ProvinceBorder>();
            foreach (var region in _regions.Values) region.InitBorders();
            TextureManager.InitRegionsBordersMap(_regionsBorders);
        }
        public static StrategicRegion SelectRegion(int color)
        {
            if (ProvinceManager.TryGetProvince(color, out Province province) && province.region != null)
            {
                SelectedRegion = province.region;
            }
            else SelectedRegion = null;
            ProvinceManager.SelectedProvince = null;
            StateManager.SelectedState = null;
            return SelectedRegion;
        }

        public static StrategicRegion SelectRMBRegion(int color)
        {
            if (ProvinceManager.TryGetProvince(color, out Province province) && province.region != null)
            {
                RMBRegion = province.region;
            }
            else RMBRegion = null;
            ProvinceManager.RMBProvince = null;
            StateManager.RMBState = null;
            return RMBRegion;
        }

        public static void ValidateAllRegions()
        {
            foreach (StrategicRegion region in _regions.Values) region.Validate();
        }

        private static void HandleDelete()
        {

        }

        private static void HandleEscape()
        {
            SelectedRegion = null;
            RMBRegion = null;
        }
    }
}
