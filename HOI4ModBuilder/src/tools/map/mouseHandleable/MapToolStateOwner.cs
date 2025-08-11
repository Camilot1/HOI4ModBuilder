using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    class MapToolStateOwner : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STATE_OWNER;

        public MapToolStateOwner(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  new[] { EnumEditLayer.STATES },
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

            CountryManager.TryGetCountry(parameter, out var newStateOwner);

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (province.State == null)
                    return false;

                var history = province.State.History.GetValue();
                var prevStateOwner = history.Owner.GetValue();

                if (prevStateOwner == newStateOwner)
                    return false;

                Action<StateHistory, Country> action = (stateHistory, stateOwner) =>
                {
                    stateHistory.Owner.SetValue(stateOwner);
                    province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, null);
                };

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(history, newStateOwner),
                    () => action(history, prevStateOwner)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                string newParameter = "";
                if (province.State != null && province.State.History.GetValue().Owner.GetValue() != null)
                    newParameter = province.State.History.GetValue().Owner.GetValue().Tag;

                MainForm.Instance.ComboBox_Tool_Parameter.Text = newParameter;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }

            return true;
        }
    }
}