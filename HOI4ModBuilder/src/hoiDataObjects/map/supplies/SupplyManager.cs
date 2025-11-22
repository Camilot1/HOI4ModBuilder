using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.supply;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Shapes;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.railways
{
    class SupplyManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map" });
        private static readonly string RAILWAYS_FILE_NAME = "railways.txt";
        private static readonly string SUPPLY_NODES_FILE_NAME = "supply_nodes.txt";

        private static bool _needToSaveSupplyNodes;
        public static bool NeedToSaveSupplyNodes
        {
            get => _needToSaveSupplyNodes;
            set => _needToSaveSupplyNodes = value;
        }
        public static SupplyNode SelectedSupplyNode = null;
        public static List<SupplyNode> SupplyNodes { get; private set; }

        public static void RemoveSupplyNode(SupplyNode supplyNode)
        {
            SupplyNodes.Remove(supplyNode);
            supplyNode.RemoveFromProvince();
            NeedToSaveSupplyNodes = true;
        }

        private static bool _needToSaveRailways;
        public static bool NeedToSaveRailways
        {
            get => _needToSaveRailways;
            set => _needToSaveRailways = value;
        }
        public static Railway SelectedRailway { get; set; }
        public static Railway RMBRailway { get; set; }

        public static List<Railway> Railways { get; private set; }

        public static void RemoveRailway(Railway railway)
        {
            railway.RemoveFromProvinces();
            Railways.Remove(railway);
            NeedToSaveRailways = true;
        }

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(BaseSettings settings)
        {
            HandleEscape();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.ANY_FORMAT);

            if (!fileInfoPairs.TryGetValue(RAILWAYS_FILE_NAME, out FileInfo railwaysFileInfo))
                throw new FileNotFoundException(RAILWAYS_FILE_NAME);

            if (!fileInfoPairs.TryGetValue(SUPPLY_NODES_FILE_NAME, out FileInfo supplyNodesFileInfo))
                throw new FileNotFoundException(SUPPLY_NODES_FILE_NAME);

            NeedToSaveRailways = railwaysFileInfo.needToSave;
            NeedToSaveSupplyNodes = supplyNodesFileInfo.needToSave;

            LoadRailways(railwaysFileInfo.filePath);
            LoadSupplyNodes(supplyNodesFileInfo.filePath);
        }

        public static void SaveAll(BaseSettings settings)
        {
            SaveRailways(settings.modDirectory + FOLDER_PATH);
            SaveSupplyNodes(settings.modDirectory + FOLDER_PATH);
        }

        public static void SaveRailways(string dirPath)
        {
            if (!NeedToSaveRailways)
                return;

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string railwaysPath = dirPath + RAILWAYS_FILE_NAME;
            var sb = new StringBuilder();
            foreach (var railway in Railways)
                if (railway.ProvincesCount > 1)
                    railway.Save(sb);
            File.WriteAllText(railwaysPath, sb.ToString());
        }

        public static void SaveSupplyNodes(string dirPath)
        {
            if (!NeedToSaveSupplyNodes)
                return;

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string supplyNodesPath = dirPath + SUPPLY_NODES_FILE_NAME;

            var sb = new StringBuilder();
            foreach (var supplyNode in SupplyNodes)
                supplyNode.Save(sb);
            File.WriteAllText(supplyNodesPath, sb.ToString());
        }

        public static void Draw(bool showRailways, bool showSupplyHubs)
        {
            if (showRailways)
            {
                GL.Color4(1f, 0f, 0f, 1f);

                foreach (var railway in Railways)
                {
                    if (railway.ProvincesCount < 2)
                        continue;
                    GL.LineWidth(2f + railway.Level);
                    GL.Begin(PrimitiveType.LineStrip);
                    railway.Draw();
                    GL.End();
                }
            }

            if (SelectedRailway != null)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth(10f);
                GL.Begin(PrimitiveType.LineStrip);
                SelectedRailway.Draw();
                GL.End();

                GL.Color4(0f, 0f, 1f, 1f);
                GL.LineWidth(4f);
                GL.Begin(PrimitiveType.LineStrip);
                SelectedRailway.Draw();
                GL.End();
            }

            if (RMBRailway != null)
            {
                GL.Color4(0f, 0f, 1f, 1f);
                GL.LineWidth(7f);
                GL.Begin(PrimitiveType.LineStrip);
                RMBRailway.Draw();
                GL.End();

                GL.Color4(1f, 0.42f, 0f, 1f);
                GL.LineWidth(3f);
                GL.Begin(PrimitiveType.LineStrip);
                RMBRailway.Draw();
                GL.End();
            }

            if (showSupplyHubs)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.PointSize(16f);
                GL.Begin(PrimitiveType.Points);
                foreach (var supplyNode in SupplyNodes)
                {
                    var province = supplyNode.GetProvince();
                    GL.Vertex2(province.center.x + 0.5f, province.center.y + 0.5f);
                }
                GL.End();
            }

            if (SelectedSupplyNode != null)
            {
                var province = SelectedSupplyNode.GetProvince();
                GL.Color4(1f, 0f, 0f, 1f);
                GL.PointSize(19f);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex2(province.center.x + 0.5f, province.center.y + 0.5f);
                GL.End();

                GL.Color4(0f, 0f, 1f, 1f);
                GL.PointSize(16f);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex2(province.center.x + 0.5f, province.center.y + 0.5f);
                GL.End();
            }
        }

        public static Railway SelectRailway(Point2D point)
        {
            if (!ProvinceManager.TryGet(point, out Province p))
            {
                SelectedRailway = null;
                return null;
            }

            Point2D movedPoint = new Point2D { x = point.x - 0.5d, y = point.y - 0.5d };

            var product = p.ForEachRailway((railway) =>
            {
                if (railway.IsOnRailway(movedPoint))
                {
                    SelectedRailway = railway;
                    return railway;
                }
                return null;
            });

            return product;
        }

        public static Railway SelectRMBRailway(Point2D point)
        {
            if (!ProvinceManager.TryGet(point, out Province p))
            {
                RMBRailway = null;
                return null;
            }

            Point2D movedPoint = new Point2D { x = point.x - 0.5d, y = point.y - 0.5d };

            var result = p.ForEachRailway((railway) =>
            {
                if (railway.IsOnRailway(movedPoint))
                {
                    RMBRailway = railway;
                    return railway;
                }
                return null;
            });

            return result;
        }

        public static bool SelectSupplyNode(Point2D point)
        {
            if (ProvinceManager.SelectedProvince == null || ProvinceManager.SelectedProvince.SupplyNode == null)
                return false;

            Point2D movedPoint = new Point2D { x = point.x - 0.5d, y = point.y - 0.5d };

            double distance = ProvinceManager.SelectedProvince.center.GetDistanceTo(movedPoint);

            if (distance <= 18f / MapManager.zoomFactor / 1000f)
            {
                SelectedSupplyNode = ProvinceManager.SelectedProvince.SupplyNode;
                return true;
            }

            SelectedSupplyNode = null;
            return false;
        }

        private static void LoadSupplyNodes(string filePath)
        {
            string[] supplyNodesData = File.ReadAllLines(filePath);

            if (SupplyNodes != null)
                foreach (var supplyNode in SupplyNodes)
                    supplyNode.RemoveFromProvince();

            SupplyNodes = new List<SupplyNode>();

            string[] data;
            byte level;

            var invalidProvincesIDs = new List<string>();

            foreach (string supplyNodeData in supplyNodesData)
            {
                data = supplyNodeData.Trim().Split(' ');
                if (data.Length < 2)
                    continue;

                level = byte.Parse(data[0]);

                var rawProvinceID = data[1];
                if (!ushort.TryParse(rawProvinceID, out var provinceID) ||
                    !ProvinceManager.TryGet(provinceID, out Province province) ||
                    !SupplyNode.CanAddToProvince(province))
                {
                    invalidProvincesIDs.Add(rawProvinceID);
                    continue;
                }

                var node = new SupplyNode(1, province);
                province.SupplyNode = node;
                SupplyNodes.Add(node);
            }

            if (invalidProvincesIDs.Count > 0)
                Logger.LogWarning(
                    EnumLocKey.WARNING_SUPPLY_HUB_IS_NOT_VALID_PROVINCES_AND_WILL_BE_REMOVED,
                    new Dictionary<string, string> {
                        { "{filePath}", filePath },
                        { "{invalidProvincesIDs}", string.Join(", ", invalidProvincesIDs) }
                    }
                );
        }

        private static void LoadRailways(string filePath)
        {
            string[] railwaysData = File.ReadAllLines(filePath);

            //Очищаем старые данные
            if (Railways != null)
                foreach (var railway in Railways)
                    railway.RemoveFromProvinces();

            Railways = new List<Railway>();

            string[] data;
            byte level;
            int provinceCount;
            List<Province> provinces;

            var invalidProvincesIDs = new List<string>();

            int lineIndex = 0;
            foreach (string railwayData in railwaysData)
            {
                lineIndex++;
                var line = railwayData.Trim();
                data = line.Split(' ');
                if (data.Length < 2)
                    continue;

                level = byte.Parse(data[0]);
                provinceCount = int.Parse(data[1]);
                int realProvincesCount = data.Length - 2;
                provinces = new List<Province>(data.Length);
                invalidProvincesIDs.Clear();

                for (int i = 0; i < provinceCount && i < realProvincesCount; i++)
                {
                    var rawProvinceID = data[i + 2];
                    if (!ushort.TryParse(rawProvinceID, out var provinceID) ||
                        !ProvinceManager.TryGet(provinceID, out Province province) ||
                        province.Type != EnumProvinceType.LAND)
                    {
                        invalidProvincesIDs.Add(rawProvinceID);
                        continue;
                    }
                    provinces.Add(province);
                }

                if (invalidProvincesIDs.Count > 0)
                    Logger.LogWarning(
                        EnumLocKey.WARNING_RAILWAY_NOT_VALID_PROVINCES_WILL_BE_REMOVED,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", filePath },
                            { "{lineIndex}", $"{lineIndex}" },
                            { "{invalidProvincesIDs}", string.Join(", ", invalidProvincesIDs) }
                        }
                    );

                if (provinces.Count < 2)
                {
                    var invalidProvincesIDsStr = invalidProvincesIDs.Count == 0 ?
                        GuiLocManager.GetLoc(EnumLocKey.NONE) :
                        string.Join(", ", invalidProvincesIDs);
                    Logger.LogWarning(
                        EnumLocKey.WARNING_RAILWAY_HAS_NOT_ENOUGH_VALID_PROVINCES_AND_WILL_BE_REMOVED,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", filePath },
                            { "{lineIndex}", $"{lineIndex}" },
                            { "{invalidProvincesIDs}", invalidProvincesIDsStr }
                        }
                    );
                    continue;
                }

                var railway = new Railway(level, provinces);
                railway.AddToProvinces();
                Railways.Add(railway);
            }
        }

        public static void HandleCursor(MouseButtons button, Point2D pos)
        {
            if (button == MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    if (!MapManager.displayLayers[(int)EnumAdditionalLayers.RAILWAYS])
                        return;
                    if (SelectedRailway == null)
                        return;
                    if (SelectedRailway.CanAddProvince(ProvinceManager.SelectedProvince))
                        RailwaysTool.AddProvinceToRailway(SelectedRailway, ProvinceManager.SelectedProvince);
                    else if (SelectedRailway.CanRemoveProvince(ProvinceManager.SelectedProvince))
                        RailwaysTool.RemoveProvinceFromRailway(SelectedRailway, ProvinceManager.SelectedProvince);
                }
                else
                {
                    if (MapManager.displayLayers[(int)EnumAdditionalLayers.RAILWAYS])
                        SelectRailway(pos);

                    if (MapManager.displayLayers[(int)EnumAdditionalLayers.SUPPLY_HUBS] && SelectSupplyNode(pos))
                        SelectedRailway = null;
                }
            }
            else if (button == MouseButtons.Right)
                SelectRMBRailway(pos);
        }

        private static void HandleEscape()
        {
            SelectedRailway = null;
            SelectedSupplyNode = null;
            RMBRailway = null;
        }

        public static void RemoveProvinceData(Province province)
        {
            if (province == null)
                return;

            if (province.GetRailwaysCount() > 0)
            {
                NeedToSaveRailways = true;
                province.ForEachRailway(r => r.RemoveProvince(province));
            }

            if (province.SupplyNode != null)
            {
                NeedToSaveSupplyNodes = true;
                RemoveSupplyNode(province.SupplyNode);
            }
        }
    }
}
