using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceType : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_TYPE;

        public MapToolProvinceType(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.PROVINCES) return;

            Enum.TryParse(parameter.ToUpper(), out EnumProvinceType newType);

            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);
            var prevType = province.Type;

            if (buttons == MouseButtons.Left && newType != prevType && province != null)
            {
                void action(EnumProvinceType type)
                {
                    province.Type = type;
                    MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                }

                action(newType);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevType), () => action(newType)));
            }
            else if (buttons == MouseButtons.Right && province != null)
            {
                int prevSelectedIndex = MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex;
                int newSelectedIndex = (int)province.Type;

                if (prevSelectedIndex == newSelectedIndex) return;

                void action(int i)
                {
                    MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex = i;
                    MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
                }
                action(newSelectedIndex);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevSelectedIndex), () => action(newSelectedIndex)));
            }
        }
    }
}
