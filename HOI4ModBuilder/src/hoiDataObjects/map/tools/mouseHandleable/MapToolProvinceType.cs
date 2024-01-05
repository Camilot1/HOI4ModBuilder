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

            byte newTypeId = 0;
            switch (parameter)
            {
                case "land": newTypeId = 0; break;
                case "sea": newTypeId = 1; break;
                case "lake": newTypeId = 2; break;
            }

            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);
            byte prevTypeId = province.TypeId;

            if (buttons == MouseButtons.Left && newTypeId != prevTypeId && province != null)
            {
                Action<byte> action = (t) =>
                {
                    province.UpdateTypeId(t);
                    MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                action(newTypeId);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevTypeId), () => action(newTypeId)));
            }
            else if (buttons == MouseButtons.Right && province != null)
            {
                int prevSelectedIndex = MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex;
                int newSelectedIndex = province.TypeId;

                if (prevSelectedIndex == newSelectedIndex) return;

                Action<int> action = (i) =>
                {
                    MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex = i;
                    MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
                };
                action(newSelectedIndex);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevSelectedIndex), () => action(newSelectedIndex)));
            }
        }
    }
}
