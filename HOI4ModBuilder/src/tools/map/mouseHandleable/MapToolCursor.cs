using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System.Text;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.hoiDataObjects.map.buildings;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolCursor : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.CURSOR;

        public MapToolCursor(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey { key = Keys.C },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  null,
                  0
              )
        { }

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            Province selectedProvince = null;
            State selectedState = null;
            StrategicRegion selectedRegion = null;

            if (mouseEventArgs.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift && MapManager.displayLayers[(int)EnumAdditionalLayers.OVERLAY_TEXTURES])
            {
                for (int i = MapManager.additionalMapTextures.Count - 1; i >= 0; i--)
                {
                    var info = MapManager.additionalMapTextures[i];
                    if (info.plane.Inbounds(pos))
                    {
                        MapManager.selectedTexturedPlane = info.plane;
                        return true;
                    }
                }
            }

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                var warningCodes = new List<EnumMapWarningCode>();
                var errorCodes = new List<EnumMapErrorCode>();

                if (MapManager.displayLayers[(int)EnumAdditionalLayers.WARNINGS])
                    warningCodes = WarningsManager.Instance.GetWarningCodes(pos, 2);
                if (MapManager.displayLayers[(int)EnumAdditionalLayers.ERRORS])
                    errorCodes = ErrorManager.Instance.GetErrorCodes(pos, 2);

                var buildings = MapPositionsManager.GetWarningCodes(pos, 2);

                if (warningCodes.Count != 0 || errorCodes.Count != 0 || buildings.Count != 0)
                {
                    var sb = new StringBuilder();
                    if (warningCodes.Count > 0)
                    {
                        sb.Append(GuiLocManager.GetLoc(EnumLocKey.WARNINGS)).Append(':').Append(Constants.NEW_LINE);
                        foreach (EnumMapWarningCode code in warningCodes)
                            sb.Append("    ").Append(code.ToString()).Append('\n');
                    }
                    if (errorCodes.Count > 0)
                    {
                        sb.Append(GuiLocManager.GetLoc(EnumLocKey.ERRORS)).Append(':').Append(Constants.NEW_LINE);
                        foreach (EnumMapErrorCode code in errorCodes)
                            sb.Append("    ").Append(code.ToString()).Append('\n');
                    }
                    if (buildings.Count > 0)
                    {
                        sb.Append(GuiLocManager.GetLoc(EnumLocKey.BUILDINGS)).Append(':').Append(Constants.NEW_LINE);
                        foreach (var data in buildings)
                            sb.Append("    ").Append(data).Append('\n');
                    }
                    Logger.LogSingleErrorMessage(sb.ToString());
                    return true;
                }
            }

            if (!pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
            {
                if (mouseEventArgs.Button == MouseButtons.Right)
                    ProvinceManager.RMBProvince = null;
                return true;
            }

            int color = MapManager.GetColor(pos);

            if (enumEditLayer == EnumEditLayer.PROVINCES)
            {
                if (mouseEventArgs.Button == MouseButtons.Left)
                    selectedProvince = ProvinceManager.SelectProvince(color);
                else if (mouseEventArgs.Button == MouseButtons.Right)
                    selectedProvince = ProvinceManager.SelectRMBProvince(color);
            }
            else if (enumEditLayer == EnumEditLayer.STATES)
            {
                if (mouseEventArgs.Button == MouseButtons.Left)
                    selectedState = StateManager.SelectState(color);
                else if (mouseEventArgs.Button == MouseButtons.Right)
                    selectedState = StateManager.SelectRMBState(color);
            }
            else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)
            {
                if (mouseEventArgs.Button == MouseButtons.Left)
                    selectedRegion = StrategicRegionManager.SelectRegion(color);
                else if (mouseEventArgs.Button == MouseButtons.Right)
                    selectedRegion = StrategicRegionManager.SelectRMBRegion(color);
            }

            if (MapManager.displayLayers[(int)EnumAdditionalLayers.ADJACENCIES])
                AdjacenciesManager.HandleCursor(mouseEventArgs.Button, pos);
            if (MapManager.displayLayers[(int)EnumAdditionalLayers.RAILWAYS])
                SupplyManager.HandleCursor(mouseEventArgs.Button, pos);

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                var selectedIds = new List<ushort>();
                if (selectedProvince != null)
                {
                    foreach (var obj in ProvinceManager.GroupSelectedProvinces)
                        selectedIds.Add(obj.Id);

                    if (selectedIds.Count == 0)
                        selectedIds.Add(selectedProvince.Id);
                }
                else if (selectedState != null)
                {
                    foreach (var obj in StateManager.GroupSelectedStates)
                        selectedIds.Add(obj.Id.GetValue());

                    if (selectedIds.Count == 0)
                        selectedIds.Add(selectedState.Id.GetValue());
                }
                else if (selectedRegion != null)
                {
                    foreach (var obj in StrategicRegionManager.GroupSelectedRegions)
                        selectedIds.Add(obj.Id);

                    if (selectedIds.Count == 0)
                        selectedIds.Add(selectedRegion.Id);
                }

                selectedIds.Sort();
                var sbIds = new StringBuilder();
                foreach (var id in selectedIds)
                    sbIds.Append(id).Append(' ');
                MainForm.Instance.textBox_SelectedObjectId.Text = sbIds.ToString();

            }
            MainForm.Instance.textBox_PixelPos.Text = (int)pos.x + "; " + (int)pos.y;
            MainForm.Instance.textBox_HOI4PixelPos.Text = (int)pos.x + "; " + (MapManager.MapSize.y - (int)pos.y);

            return true;
        }
    }
}
