using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceContinent : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_CONTINENT;

        public MapToolProvinceContinent(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);

            if (buttons == MouseButtons.Left && province != null)
            {
                byte prevContinentId = province.ContinentId;
                byte newContinentId = (byte)ContinentManager.GetContinentId(parameter);

                Action<List<Tuple<byte, byte, Province>>> undoAction = null;
                Action<List<Tuple<byte, byte, Province>>> redoAction = null;
                var tuples = new List<Tuple<byte, byte, Province>>(0);

                switch (enumEditLayer)
                {
                    case EnumEditLayer.PROVINCES:
                        if (prevContinentId != newContinentId)
                            tuples.Add(new Tuple<byte, byte, Province>(prevContinentId, newContinentId, province));
                        break;
                    case EnumEditLayer.STATES:
                        if (province.State != null)
                        {
                            foreach (var p in province.State.provinces)
                            {
                                if (p.ContinentId != newContinentId)
                                    tuples.Add(new Tuple<byte, byte, Province>(p.ContinentId, newContinentId, p));
                            }
                        }
                        break;
                    case EnumEditLayer.STRATEGIC_REGIONS:
                        province.Region?.ForEachProvince((r, p) =>
                        {
                            if (p.ContinentId != newContinentId)
                                tuples.Add(new Tuple<byte, byte, Province>(p.ContinentId, newContinentId, p));
                        });
                        break;
                }

                if (tuples.Count > 0)
                {
                    redoAction = (list) =>
                    {
                        foreach (var tuple in list) tuple.Item3.ContinentId = tuple.Item2;
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                    };
                    undoAction = (list) =>
                    {
                        foreach (var tuple in list) tuple.Item3.ContinentId = tuple.Item1;
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                    };
                }


                if (redoAction != null && undoAction != null)
                {
                    redoAction(tuples);
                    MapManager.actionPairs.Add(new ActionPair(() => undoAction(tuples), () => redoAction(tuples)));
                }
            }
            else if (buttons == MouseButtons.Right && province != null)
            {
                int prevSelectedIndex = MainForm.Instance.ComboBox_Tool_Parameter.SelectedIndex;
                int newSelectedIndex = province.ContinentId;

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
