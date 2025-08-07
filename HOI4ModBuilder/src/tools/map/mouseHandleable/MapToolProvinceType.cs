using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceType : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_TYPE;

        public MapToolProvinceType(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  new[] { EnumEditLayer.PROVINCES },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            if (!Enum.TryParse(parameter.ToUpper(), out EnumProvinceType newType))
                return false;

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return false;

            var prevType = province.Type;

            if (mouseEventArgs.Button == MouseButtons.Left && newType != prevType)
            {
                void action(EnumProvinceType type)
                {
                    province.Type = type;
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, null);
                }

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newType),
                    () => action(prevType)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                int prevSelectedIndex = MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex;
                int newSelectedIndex = (int)province.Type;

                if (prevSelectedIndex == newSelectedIndex)
                    return false;

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

            return true;
        }
    }
}
