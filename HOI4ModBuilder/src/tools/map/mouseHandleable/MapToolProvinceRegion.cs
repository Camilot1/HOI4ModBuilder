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
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }


        public override void Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (enumEditLayer != EnumEditLayer.PROVINCES)
                return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                return;

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                var prevRegion = province.Region;
                StrategicRegionManager.TryGetRegion(ushort.Parse(parameter), out StrategicRegion newRegion);

                Action<StrategicRegion, StrategicRegion> action = (src, dest) =>
                {
                    if (StrategicRegionManager.TransferProvince(province, src, dest))
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(prevRegion, newRegion),
                    () => action(newRegion, prevRegion)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right && province.Region != null)
            {
                MainForm.Instance.ComboBox_Tool_Parameter.Text = "" + province.Region.Id;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}
