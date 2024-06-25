using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.supply;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.tools.map.advanced
{
    class SupplyNodeTool
    {
        public SupplyNodeTool()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => RemoveNode(SupplyManager.SelectedSupplyNode));

            MainForm.SubscribeTabKeyEvent(
                EnumTabPage.MAP,
                Keys.H,
                (sender, e) =>
                {
                    if (e.Control && !(e.Alt || e.Shift))
                    {
                        if (ProvinceManager.SelectedProvince != null && ProvinceManager.SelectedProvince.SupplyNode != null)
                        {
                            RemoveNode(ProvinceManager.SelectedProvince.SupplyNode);
                        }
                        else
                        {
                            var node = CreateNode(1, ProvinceManager.SelectedProvince);
                            if (node != null) SupplyManager.SelectedSupplyNode = node;
                        }
                    }
                }
            );
        }

        public static SupplyNode CreateNode(byte level, Province province)
        {
            if (!SupplyNode.CanAddToProvince(province)) return null;
            var node = new SupplyNode(level, province);
            AddNode(node);
            return node;
        }

        public static void AddNode(SupplyNode node)
        {
            if (node == null) return;

            MapManager.ActionHistory.Add(
                () => AddNodeAction(node),
                () => RemoveNodeAction(node)
            );
        }

        public static void RemoveNode(SupplyNode node)
        {
            if (node == null) return;

            MapManager.ActionHistory.Add(
                () => RemoveNodeAction(node),
                () => AddNodeAction(node)
            );
        }

        private static void AddNodeAction(SupplyNode node)
        {
            if (node == null || !node.AddToProvince()) return;
            SupplyManager.SupplyNodes.Add(node);
            SupplyManager.NeedToSaveSupplyNodes = true;
        }

        private static void RemoveNodeAction(SupplyNode node)
        {
            if (node == null || !node.RemoveFromProvince()) return;
            if (SupplyManager.SelectedSupplyNode == node) SupplyManager.SelectedSupplyNode = null;

            SupplyManager.SupplyNodes.Remove(node);
            SupplyManager.NeedToSaveSupplyNodes = true;
        }
    }
}
