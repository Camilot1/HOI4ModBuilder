using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceState : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_STATE;

        public MapToolProvinceState(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.PROVINCES)
                return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize))
                return;
            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                var prevState = province.State;
                StateManager.TryGetState(ushort.Parse(parameter), out State newState);

                Action<State, State> action = (src, dest) =>
                {
                    if (StateManager.TransferProvince(province, src, dest))
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(prevState, newState),
                    () => action(newState, prevState)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right && province.State != null)
            {
                MainForm.Instance.ComboBox_Tool_Parameter.Text = "" + province.State.IdNew.GetValue();
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}
