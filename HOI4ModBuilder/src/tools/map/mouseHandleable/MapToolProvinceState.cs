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
                  mapTools, enumTool, new HotKey { shift = true, key = Keys.S },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  new[] { EnumEditLayer.PROVINCES, EnumEditLayer.STATES, EnumEditLayer.STRATEGIC_REGIONS },
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

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                var prevState = province.State;
                if (!ushort.TryParse(parameter, out var newStateId) ||
                    !StateManager.TryGetState(newStateId, out var newState))
                    return false;

                Action<List<Tuple<State, State, Province>>> undoAction = null;
                Action<List<Tuple<State, State, Province>>> redoAction = null;
                var tuples = new List<Tuple<State, State, Province>>(0);

                switch (enumEditLayer)
                {
                    case EnumEditLayer.PROVINCES:
                        if (prevState != newState)
                            tuples.Add(new Tuple<State, State, Province>(prevState, newState, province));
                        break;
                    case EnumEditLayer.STATES:
                        if (province.State != null)
                        {
                            foreach (var p in province.State.Provinces)
                            {
                                if (p.State != newState)
                                    tuples.Add(new Tuple<State, State, Province>(p.State, newState, p));
                            }
                        }
                        break;
                    case EnumEditLayer.STRATEGIC_REGIONS:
                        province.Region?.ForEachProvince((r, p) =>
                        {
                            if (p.State != newState)
                                tuples.Add(new Tuple<State, State, Province>(prevState, newState, p));
                        });
                        break;
                }

                if (tuples.Count > 0)
                {
                    redoAction = (list) =>
                    {
                        bool needToRedraw = false;
                        foreach (var tuple in list)
                            needToRedraw |= StateManager.TransferProvince(tuple.Item3, tuple.Item1, tuple.Item2);
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                    };
                    undoAction = (list) =>
                    {
                        bool needToRedraw = false;
                        foreach (var tuple in list)
                            needToRedraw |= StateManager.TransferProvince(tuple.Item3, tuple.Item1, tuple.Item2);
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                    };
                }

                if (redoAction != null && undoAction != null)
                {
                    MapManager.ActionsBatch.AddWithExecute(
                        () => redoAction(tuples),
                        () => undoAction(tuples)
                    );
                }
            }
            else if (mouseEventArgs.Button == MouseButtons.Right && province.State != null)
            {
                MainForm.Instance.ComboBox_Tool_Parameter.Text = "" + province.State.Id.GetValue();
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }

            return true;
        }
    }
}
