using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceState : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_STATE;

        public MapToolProvinceState(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.PROVINCES) return;

            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);

            if (buttons == MouseButtons.Left && province != null)
            {
                var prevState = province.State;
                StateManager.TryGetState(ushort.Parse(parameter), out State newState);

                Action<State, State> action = (src, dest) =>
                {
                    if (StateManager.TransferProvince(province, src, dest))
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                action(prevState, newState);
                MapManager.actionPairs.Add(new ActionPair(() => action(newState, prevState), () => action(prevState, newState)));
            }
            else if (buttons == MouseButtons.Right && province != null && province.State != null)
            {
                MainForm.Instance.ComboBox_Tool_Parameter.Text = "" + province.State.Id;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}
