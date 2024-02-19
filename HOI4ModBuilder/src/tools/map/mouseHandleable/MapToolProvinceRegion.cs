using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceRegion : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_REGION;

        public MapToolProvinceRegion(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.PROVINCES) return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province)) return;

            if (buttons == MouseButtons.Left)
            {
                var prevRegion = province.Region;
                StrategicRegionManager.TryGetRegion(ushort.Parse(parameter), out StrategicRegion newRegion);

                Action<StrategicRegion, StrategicRegion> action = (src, dest) =>
                {
                    if (StrategicRegionManager.TransferProvince(province, src, dest))
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                action(prevRegion, newRegion);
                MapManager.actionPairs.Add(new ActionPair(() => action(newRegion, prevRegion), () => action(prevRegion, newRegion)));
            }
            else if (buttons == MouseButtons.Right && province.Region != null)
            {
                MainForm.Instance.ComboBox_Tool_Parameter.Text = "" + province.Region.Id;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}
