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
using static System.Windows.Forms.AxHost;

namespace HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion
{
    class StrategicRegionManager : IParadoxRead
    {
        public static StrategicRegionManager Instance { get; private set; }

        private static FileInfo _currentFile = null;
        private static Dictionary<string, List<StrategicRegion>> _regionsByFilesMap = new Dictionary<string, List<StrategicRegion>>();
        private static Dictionary<ushort, StrategicRegion> _regionsById = new Dictionary<ushort, StrategicRegion>();
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

            _regionsByFilesMap = new Dictionary<string, List<StrategicRegion>>(0);
            _regionsById = new Dictionary<ushort, StrategicRegion>(0);
            _regionsBorders = new HashSet<ProvinceBorder>(0);

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"map\strategicregions\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                _currentFile = fileInfo;
                var fs = new FileStream(fileInfo.filePath, FileMode.Open);
                ParadoxParser.Parse(fs, Instance);
            }
        }

        public static void Save(Settings settings)
        {
            var sb = new StringBuilder();
            bool needToSave = false;
            foreach (string fileName in _regionsByFilesMap.Keys)
            {
                foreach (var region in _regionsByFilesMap[fileName])
                {
                    try
                    {
                        if (needToSave || region.needToSave)
                        {
                            needToSave = true;
                            region.Save(sb);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_REGION_SAVING,
                            new Dictionary<string, string> { { "{regionId}", $"{region.Id}" } }
                        ), ex);
                    }
                }

                if (needToSave) File.WriteAllText(settings.modDirectory + @"map\strategicregions\" + fileName, sb.ToString());
                sb.Length = 0;
            }
        }

        public static void Draw(bool showCenters)
        {
            if (showCenters)
            {
                GL.Color3(1f, 0f, 0f);
                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                foreach (var region in _regionsById.Values)
                {
                    if (region.dislayCenter) GL.Vertex2(region.center.x, region.center.y);
                }
                GL.End();
            }


            if (SelectedRegion != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(5f);

                foreach (var border in SelectedRegion.borders)
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

            if (RMBRegion != null)
            {
                GL.Color4(1f, 0.42f, 0f, 1f);
                GL.LineWidth(2.5f);

                foreach (var border in RMBRegion.borders)
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

        public static void AddRegionsBorder(ProvinceBorder border)
        {
            _regionsBorders.Add(border);
        }

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

        public static void AddState(StrategicRegion region, out bool canAddById)
        {
            ushort id = region.Id;
            canAddById = _regionsById.ContainsKey(id);

            if (canAddById)
            {
                _regionsById[id] = region;
                region.needToSave = true;
            }
        }

        public static bool ContainsRegionIdKey(ushort id)
        {
            return _regionsById.ContainsKey(id);
        }

        public static Dictionary<ushort, StrategicRegion>.KeyCollection GetRegionsIds()
        {
            return _regionsById.Keys;
        }

        public static Dictionary<ushort, StrategicRegion>.ValueCollection GetRegions()
        {
            return _regionsById.Values;
        }

        public static bool GetRegion(ushort id, out StrategicRegion region)
        {
            return _regionsById.TryGetValue(id, out region);
        }

        public static void AddRegion(ushort id, StrategicRegion region)
        {
            _regionsById[id] = region;
            region.needToSave = true;
        }

        public static void RemoveRegion(ushort id)
        {
            var region = _regionsById[id];
            if (region != null)
            {
                _regionsById.Remove(id);
                region.needToSave = true;
            }
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            var regions = new List<StrategicRegion>();
            if (token == "strategic_region")
            {
                var region = new StrategicRegion();
                try
                {
                    parser.Parse(region);
                    region.needToSave = _currentFile.needToSave;
                    _regionsById[region.Id] = region;
                    regions.Add(region);
                }
                catch (Exception ex)
                {
                    string idString = region.Id == 0 ? GuiLocManager.GetLoc(EnumLocKey.ERROR_REGION_UNSUCCESSFUL_REGION_ID_PARSE_RESULT) : $"{region.Id}";
                    Logger.LogExceptionAsError(
                        EnumLocKey.ERROR_WHILE_REGION_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", idString },
                            { "{filePath}", _currentFile.filePath }
                        },
                        ex
                    );
                }
            }
            _regionsByFilesMap.Add(_currentFile.fileName, regions);
        }

        public static void CalculateCenters()
        {
            foreach (var region in _regionsById.Values) region.CalculateCenter();
        }


        public static void InitRegionsBorders()
        {
            _regionsBorders = new HashSet<ProvinceBorder>(0);
            foreach (var region in _regionsById.Values)
            {
                region.InitBorders();
            }
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
            foreach (StrategicRegion region in _regionsById.Values)
            {
                region.Validate();
            }
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
