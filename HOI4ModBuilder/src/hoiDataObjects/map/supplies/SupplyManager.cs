using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.supply;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static HOI4ModBuilder.utils.Structs;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced;

namespace HOI4ModBuilder.src.hoiDataObjects.map.railways
{
    class SupplyManager
    {
        public static bool NeedToSaveSupplyNodes { get; set; }
        public static SupplyNode SelectedSupplyNode = null;
        public static List<SupplyNode> SupplyNodes { get; private set; }

        public static bool NeedToSaveRailways { get; set; }
        public static Railway SelectedRailway { get; set; }
        public static Railway RMBRailway { get; set; }

        public static List<Railway> Railways { get; private set; }

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(MainForm.Instance.TabPage_Map, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Load(Settings settings)
        {
            HandleEscape();

            var fileInfos = FileManager.ReadMultiFileInfos(settings, @"map\");

            if (!fileInfos.TryGetValue("railways.txt", out FileInfo railwaysFileInfo))
                throw new FileNotFoundException("railways.txt");

            if (!fileInfos.TryGetValue("supply_nodes.txt", out FileInfo supplyNodesFileInfo))
                throw new FileNotFoundException("supply_nodes.txt");

            NeedToSaveRailways = railwaysFileInfo.needToSave;
            NeedToSaveSupplyNodes = supplyNodesFileInfo.needToSave;

            LoadRailways(railwaysFileInfo.filePath);
            LoadSupplyNodes(supplyNodesFileInfo.filePath);
        }

        public static void SaveAll(Settings settings)
        {
            SaveRailways(settings.modDirectory + @"map\");
            SaveSupplyNodes(settings.modDirectory + @"map\");
        }

        public static void SaveRailways(string dirPath)
        {
            if (!NeedToSaveRailways) return;

            string railwaysPath = dirPath + "railways.txt";
            var sb = new StringBuilder();
            foreach (var railway in Railways) railway.Save(sb);
            File.WriteAllText(railwaysPath, sb.ToString());
        }

        public static void SaveSupplyNodes(string dirPath)
        {
            if (!NeedToSaveSupplyNodes) return;

            string supplyNodesPath = dirPath + "supply_nodes.txt";

            var sb = new StringBuilder();
            foreach (var supplyNode in SupplyNodes) supplyNode.Save(sb);
            File.WriteAllText(supplyNodesPath, sb.ToString());

        }

        public static void Draw(bool showRailways, bool showSupplyHubs)
        {
            if (showRailways)
            {
                GL.Color4(1f, 0f, 0f, 1f);

                foreach (var railway in Railways)
                {
                    if (railway.ProvincesCount < 2) continue;
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
                    GL.Vertex2(province.center.x, province.center.y);
                }
                GL.End();
            }

            if (SelectedSupplyNode != null)
            {
                var province = SelectedSupplyNode.GetProvince();
                GL.Color4(1f, 0f, 0f, 1f);
                GL.PointSize(19f);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex2(province.center.x, province.center.y);
                GL.End();

                GL.Color4(0f, 0f, 1f, 1f);
                GL.PointSize(16f);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex2(province.center.x, province.center.y);
                GL.End();
            }
        }

        public static Railway SelectRailway(Point2D point)
        {
            if (!ProvinceManager.TryGetProvince(point, out Province p))
            {
                SelectedRailway = null;
                return null;
            }

            var product = p.ForEachRailway((railway) =>
            {
                if (railway.IsOnRailway(point))
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
            if (!ProvinceManager.TryGetProvince(point, out Province p))
            {
                RMBRailway = null;
                return null;
            }

            var result = p.ForEachRailway((railway) =>
            {
                if (railway.IsOnRailway(point))
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

            double distance = ProvinceManager.SelectedProvince.center.GetDistanceTo(point);

            if (distance <= 18f / MapManager.zoomFactor / 1000f)
            {
                SelectedSupplyNode = ProvinceManager.SelectedProvince.SupplyNode;
                return true;
            }

            SelectedSupplyNode = null;
            return false;
        }

        public static SupplyNode CreateSupplyNode(byte level, Province province)
        {
            if (province == null) return null;
            if (province.SupplyNode != null) return null;
            if (province.TypeId != 0) return null;
            return new SupplyNode(level, province);
        }

        public static bool AddSupplyNode(SupplyNode node)
        {
            if (node == null) return false;
            if (node.AddToProvince())
            {
                SupplyNodes.Add(node);
                return true;
            }
            return false;
        }

        public static bool RemoveSupplyNode(SupplyNode node)
        {
            if (node == null) return false;
            if (node.RemoveFromProvince())
            {
                SupplyNodes.Remove(node);
                return true;
            }
            return false;
        }

        private static void LoadSupplyNodes(string filePath)
        {
            string[] supplyNodesData = File.ReadAllLines(filePath);

            bool tempNeedToSave = NeedToSaveSupplyNodes;

            if (SupplyNodes != null)
            {
                foreach (var supplyNode in SupplyNodes) supplyNode.RemoveFromProvince();
            }
            SupplyNodes = new List<SupplyNode>();

            string[] data;
            byte level;
            foreach (string supplyNodeData in supplyNodesData)
            {
                data = supplyNodeData.Trim().Split(' ');
                if (data.Length < 2) continue;

                level = byte.Parse(data[0]);
                ProvinceManager.TryGetProvince(ushort.Parse(data[1]), out Province province);
                if (province != null && province.SupplyNode == null)
                {
                    AddSupplyNode(new SupplyNode(level, province));
                }
            }
            NeedToSaveSupplyNodes = tempNeedToSave;
        }

        private static void LoadRailways(string filePath)
        {
            bool tempNeedToSave = NeedToSaveRailways;
            string[] railwaysData = File.ReadAllLines(filePath);

            //Очищаем старые данные
            if (Railways != null)
            {
                foreach (var railway in Railways) railway.RemoveFromProvinces();
            }
            Railways = new List<Railway>();

            string[] data;
            byte level;
            int provinceCount;
            List<Province> provinces;

            foreach (string railwayData in railwaysData)
            {
                data = railwayData.Trim().Split(' ');
                if (data.Length < 2) continue;

                level = byte.Parse(data[0]);
                provinceCount = int.Parse(data[1]);
                provinces = new List<Province>(data.Length);

                for (int i = 0; i < provinceCount; i++)
                {
                    if (ProvinceManager.TryGetProvince(ushort.Parse(data[i + 2]), out Province province))
                    {
                        provinces.Add(province);
                    }
                }

                RailwayTool.AddRailway(new Railway(level, provinces));
            }

            NeedToSaveRailways = tempNeedToSave;
        }

        public static void HandleCursor(MouseButtons button, Point2D pos)
        {
            if (button == MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    if (SelectedRailway == null) return;
                    if (SelectedRailway.CanAddProvince(ProvinceManager.SelectedProvince))
                    {
                        RailwayTool.AddProvinceToRailway(SelectedRailway, ProvinceManager.SelectedProvince);
                    }
                    else if (SelectedRailway.CanRemoveProvince(ProvinceManager.SelectedProvince))
                    {
                        RailwayTool.RemoveProvinceFromRailway(SelectedRailway, ProvinceManager.SelectedProvince);
                    }
                }
                else
                {
                    SelectRailway(pos);

                    if (SelectSupplyNode(pos)) SelectedRailway = null;
                }
            }
            else if (button == MouseButtons.Right) SelectRMBRailway(pos);
        }

        private static void HandleDelete()
        {
            RailwayTool.RemoveRailway(SelectedRailway);

            if (RemoveSupplyNode(SelectedSupplyNode)) SelectedSupplyNode = null;
        }

        private static void HandleEscape()
        {
            SelectedRailway = null;
            SelectedSupplyNode = null;
            RMBRailway = null;
        }
    }
}
