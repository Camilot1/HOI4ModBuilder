using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceRegion : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_REGION;

        public MapToolProvinceRegion(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { shift = true, key = Keys.R },
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
                var prevRegion = province.Region;
                if (!ushort.TryParse(parameter, out var newRegionId) ||
                    !StrategicRegionManager.TryGetRegion(newRegionId, out var newRegion))
                    return false;

                Action<List<Tuple<StrategicRegion, StrategicRegion, Province>>> undoAction = null;
                Action<List<Tuple<StrategicRegion, StrategicRegion, Province>>> redoAction = null;
                var tuples = new List<Tuple<StrategicRegion, StrategicRegion, Province>>(0);

                switch (enumEditLayer)
                {
                    case EnumEditLayer.PROVINCES:
                        if (prevRegion != newRegion)
                            tuples.Add(new Tuple<StrategicRegion, StrategicRegion, Province>(prevRegion, newRegion, province));
                        break;
                    case EnumEditLayer.STATES:
                        if (province.State != null)
                        {
                            foreach (var p in province.State.Provinces)
                            {
                                if (p.Region != newRegion)
                                    tuples.Add(new Tuple<StrategicRegion, StrategicRegion, Province>(p.Region, newRegion, p));
                            }
                        }
                        break;
                    case EnumEditLayer.STRATEGIC_REGIONS:
                        province.Region?.ForEachProvince((r, p) =>
                        {
                            if (p.Region != newRegion)
                                tuples.Add(new Tuple<StrategicRegion, StrategicRegion, Province>(p.Region, newRegion, p));
                        });
                        break;
                }

                if (tuples.Count > 0)
                {
                    redoAction = (list) =>
                    {
                        bool needToRedraw = false;
                        foreach (var tuple in list)
                            needToRedraw |= StrategicRegionManager.TransferProvince(tuple.Item3, tuple.Item1, tuple.Item2);
                        MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, null);
                    };
                    undoAction = (list) =>
                    {
                        bool needToRedraw = false;
                        foreach (var tuple in list)
                            needToRedraw |= StrategicRegionManager.TransferProvince(tuple.Item3, tuple.Item2, tuple.Item1);
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
            else if (mouseEventArgs.Button == MouseButtons.Right && province.Region != null)
            {
                MainForm.Instance.ComboBox_Tool_Parameter.Text = "" + province.Region.Id;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }

            return true;
        }
    }
}
