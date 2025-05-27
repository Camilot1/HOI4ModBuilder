using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    class MapToolStateController : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STATE_CONTROLLER;

        public MapToolStateController(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (enumEditLayer != EnumEditLayer.STATES)
                return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                return;

            CountryManager.TryGetCountry(parameter, out var newStateController);

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (province.State == null)
                    return;

                var history = province.State.History.GetValue();
                var prevStateController = history.Controller.GetValue();

                if (prevStateController == newStateController)
                    return;

                Action<StateHistory, Country> action = (stateHistory, stateController) =>
                {
                    stateHistory.Controller.SetValue(stateController);
                    province.State.UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                    MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(history, newStateController),
                    () => action(history, prevStateController)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                string newParameter = "";
                if (province.State != null && province.State.History.GetValue().Controller.GetValue() != null)
                    newParameter = province.State.History.GetValue().Controller.GetValue().Tag;

                MainForm.Instance.ComboBox_Tool_Parameter.Text = newParameter;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}