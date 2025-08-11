using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceContinent : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_CONTINENT;

        public MapToolProvinceContinent(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { shift = true, key = Keys.C },
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
                int prevContinentId = province.ContinentId;
                int newContinentId = ContinentManager.GetContinentId(parameter);

                Action<List<Tuple<int, int, Province>>> undoAction = null;
                Action<List<Tuple<int, int, Province>>> redoAction = null;
                var tuples = new List<Tuple<int, int, Province>>(0);

                switch (enumEditLayer)
                {
                    case EnumEditLayer.PROVINCES:
                        if (prevContinentId != newContinentId)
                            tuples.Add(new Tuple<int, int, Province>(prevContinentId, newContinentId, province));
                        break;
                    case EnumEditLayer.STATES:
                        if (province.State != null)
                        {
                            foreach (var p in province.State.Provinces)
                            {
                                if (p.ContinentId != newContinentId)
                                    tuples.Add(new Tuple<int, int, Province>(p.ContinentId, newContinentId, p));
                            }
                        }
                        break;
                    case EnumEditLayer.STRATEGIC_REGIONS:
                        province.Region?.ForEachProvince((r, p) =>
                        {
                            if (p.ContinentId != newContinentId)
                                tuples.Add(new Tuple<int, int, Province>(p.ContinentId, newContinentId, p));
                        });
                        break;
                }

                if (tuples.Count > 0)
                {
                    redoAction = (list) =>
                    {
                        foreach (var tuple in list)
                            tuple.Item3.ContinentId = tuple.Item2;
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, null);
                    };
                    undoAction = (list) =>
                    {
                        foreach (var tuple in list)
                            tuple.Item3.ContinentId = tuple.Item1;
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, null);
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
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                int prevSelectedIndex = MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex;
                int newSelectedIndex = province.ContinentId;

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
