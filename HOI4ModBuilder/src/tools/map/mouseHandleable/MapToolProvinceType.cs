using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceType : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_TYPE;

        public MapToolProvinceType(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
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

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newType),
                    () => action(prevType)
                );
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

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newSelectedIndex),
                    () => action(prevSelectedIndex)
                );
            }
        }
    }
}
