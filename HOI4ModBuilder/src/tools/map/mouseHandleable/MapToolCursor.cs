using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolCursor : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.CURSOR;

        public MapToolCursor(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { key = Keys.C },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            Province selectedProvince = null;
            State selectedState = null;
            StrategicRegion selectedRegion = null;

            if (buttons == MouseButtons.Left && Control.ModifierKeys == Keys.Shift && MapManager.displayLayers[(int)EnumAdditionalLayers.OVERLAY_TEXTURES])
            {
                for (int i = MapManager.additionalMapTextures.Count - 1; i >= 0; i--)
                {
                    var info = MapManager.additionalMapTextures[i];
                    if (info.plane.Inbounds(pos))
                    {
                        MapManager.selectedTexturedPlane = info.plane;
                        return;
                    }
                }
            }

            if (buttons == MouseButtons.Left && MapManager.displayLayers[(int)EnumAdditionalLayers.ERRORS])
            {
                var codes = ErrorManager.GetErrorCodes(pos, 2);
                if (codes.Count != 0)
                {
                    var sb = new StringBuilder();
                    foreach (EnumMapErrorCode code in codes) sb.Append(code.ToString()).Append('\n');
                    Logger.LogSingleMessage(sb.ToString());
                    return;
                }
            }

            if (!pos.InboundsPositiveBox(MapManager.MapSize))
            {
                if (buttons == MouseButtons.Right) ProvinceManager.RMBProvince = null;
                return;
            }

            int color = MapManager.GetColor(pos);

            if (enumEditLayer == EnumEditLayer.PROVINCES)
            {
                if (buttons == MouseButtons.Left) selectedProvince = ProvinceManager.SelectProvince(color);
                else if (buttons == MouseButtons.Right) selectedProvince = ProvinceManager.SelectRMBProvince(color);
            }
            else if (enumEditLayer == EnumEditLayer.STATES)
            {
                if (buttons == MouseButtons.Left) selectedState = StateManager.SelectState(color);
                else if (buttons == MouseButtons.Right) selectedState = StateManager.SelectRMBState(color);
            }
            else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)
            {
                if (buttons == MouseButtons.Left) selectedRegion = StrategicRegionManager.SelectRegion(color);
                else if (buttons == MouseButtons.Right) selectedRegion = StrategicRegionManager.SelectRMBRegion(color);
            }

            if (MapManager.displayLayers[(int)EnumAdditionalLayers.ADJACENCIES]) AdjacenciesManager.HandleCursor(buttons, pos);
            if (MapManager.displayLayers[(int)EnumAdditionalLayers.RAILWAYS]) SupplyManager.HandleCursor(buttons, pos);

            if (selectedProvince != null) MainForm.Instance.textBox_SelectedObjectId.Text = "" + selectedProvince.Id;
            else if (selectedState != null) MainForm.Instance.textBox_SelectedObjectId.Text = "" + selectedState.Id;
            else if (selectedRegion != null) MainForm.Instance.textBox_SelectedObjectId.Text = "" + selectedRegion.Id;

            MainForm.Instance.textBox_PixelPos.Text = (int)pos.x + "; " + (int)pos.y;
            MainForm.Instance.textBox_HOI4PixelPos.Text = (int)pos.x + "; " + (MapManager.MapSize.y - (int)pos.y);

        }
    }
}
