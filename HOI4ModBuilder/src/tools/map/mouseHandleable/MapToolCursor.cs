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
using System.Collections;
using System;
using HOI4ModBuilder.src.managers.mapChecks.errors.checkers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolCursor : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.CURSOR;

        public MapToolCursor(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      key = Keys.C,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  0
              )
        { }

        public override bool isHandlingMouseMove() => false;

        public override EnumEditLayer[] GetAllowedEditLayers() => null;
        public override Func<ICollection> GetParametersProvider() => null;
        public override Func<ICollection> GetValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            if (HandeMapAdditionalTextures(mouseEventArgs, pos))
                return true;

            if (HandleMapOverlayInfoPoints(mouseEventArgs, pos, sizeFactor))
                return true;

            if (!pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                return true;

            if (enumEditLayer == EnumEditLayer.PROVINCES)
                HandleProvinceSelection(mouseEventArgs, pos);
            else if (enumEditLayer == EnumEditLayer.STATES)
                HandleStateSelection(mouseEventArgs, pos);
            else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)
                HandleRegionSelection(mouseEventArgs, pos);

            MainForm.Instance.textBox_PixelPos.Text = (int)pos.x + "; " + (int)pos.y;
            MainForm.Instance.textBox_HOI4PixelPos.Text = (int)pos.x + "; " + (MapManager.MapSize.y - (int)pos.y);

            if (MapManager.displayLayers[(int)EnumAdditionalLayers.ADJACENCIES])
                AdjacenciesManager.HandleCursor(mouseEventArgs.Button, pos);
            if (MapManager.displayLayers[(int)EnumAdditionalLayers.RAILWAYS] ||
                MapManager.displayLayers[(int)EnumAdditionalLayers.SUPPLY_HUBS])
                SupplyManager.HandleCursor(mouseEventArgs.Button, pos);

            return true;
        }

        private void HandleProvinceSelection(MouseEventArgs mouseEventArgs, Point2D pos)
        {
            int color = MapManager.GetColor(pos);
            if (mouseEventArgs.Button == MouseButtons.Left)
                ProvinceManager.Select(color);
            else if (mouseEventArgs.Button == MouseButtons.Right)
                ProvinceManager.SelectRMB(color);

            var selectedIds = new List<ushort>();
            foreach (var obj in ProvinceManager.GroupSelectedProvinces)
                selectedIds.Add(obj.Id);

            if (selectedIds.Count == 0 && ProvinceManager.SelectedProvince != null)
                selectedIds.Add(ProvinceManager.SelectedProvince.Id);

            SortAndDisplayIDs(selectedIds);
        }
        private void HandleStateSelection(MouseEventArgs mouseEventArgs, Point2D pos)
        {
            int color = MapManager.GetColor(pos);
            if (mouseEventArgs.Button == MouseButtons.Left)
                StateManager.Select(color);
            else if (mouseEventArgs.Button == MouseButtons.Right)
                StateManager.SelectRMB(color);

            var selectedIds = new List<ushort>();
            foreach (var obj in StateManager.GroupSelectedStates)
                selectedIds.Add(obj.Id.GetValue());

            if (selectedIds.Count == 0 && StateManager.SelectedState != null)
                selectedIds.Add(StateManager.SelectedState.Id.GetValue());

            SortAndDisplayIDs(selectedIds);
        }

        private void HandleRegionSelection(MouseEventArgs mouseEventArgs, Point2D pos)
        {
            int color = MapManager.GetColor(pos);
            if (mouseEventArgs.Button == MouseButtons.Left)
                StrategicRegionManager.Select(color);
            else if (mouseEventArgs.Button == MouseButtons.Right)
                StrategicRegionManager.Select(color);

            var selectedIds = new List<ushort>();
            foreach (var obj in StrategicRegionManager.SelectedGroup)
                selectedIds.Add(obj.Id);

            if (selectedIds.Count == 0 && StrategicRegionManager.SelectedRegion != null)
                selectedIds.Add(StrategicRegionManager.SelectedRegion.Id);

            SortAndDisplayIDs(selectedIds);
        }

        private void SortAndDisplayIDs(List<ushort> ids)
        {
            ids.Sort();
            MainForm.Instance.textBox_SelectedObjectId.Text = string.Join(" ", ids);
        }

        private bool HandeMapAdditionalTextures(MouseEventArgs mouseEventArgs, Point2D pos)
        {
            if (!(mouseEventArgs.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift && MapManager.displayLayers[(int)EnumAdditionalLayers.OVERLAY_TEXTURES]))
                return false;

            for (int i = MapManager.additionalMapTextures.Count - 1; i >= 0; i--)
            {
                var info = MapManager.additionalMapTextures[i];
                if (info.plane.Inbounds(pos))
                {
                    MapManager.selectedTexturedPlane = info.plane;
                    return true;
                }
            }

            return false;
        }

        private bool HandleMapOverlayInfoPoints(MouseEventArgs mouseEventArgs, Point2D pos, Point2D sizeFactor)
        {
            if (mouseEventArgs.Button != MouseButtons.Left)
                return false;

            var warningCodes = new List<EnumMapWarningCode>();
            var errorCodes = new List<EnumMapErrorCode>();

            if (MapManager.displayLayers[(int)EnumAdditionalLayers.WARNINGS])
                warningCodes = WarningsManager.Instance.GetWarningCodes(pos, 2);
            if (MapManager.displayLayers[(int)EnumAdditionalLayers.ERRORS])
                errorCodes = ErrorManager.Instance.GetErrorCodes(pos, 2);

            var buildings = MapPositionsManager.GetWarningCodes(pos, 2);

            if (warningCodes.Count == 0 && errorCodes.Count == 0 && buildings.Count == 0)
                return false;

            var sb = new StringBuilder();

            HandleWarnings();
            HandleErrors();
            HandleBuildings();

            Logger.LogSingleErrorMessage(sb.ToString());
            return true;

            void HandleWarnings()
            {
                if (warningCodes.Count == 0)
                    return;

                sb.Append(GuiLocManager.GetLoc(EnumLocKey.WARNINGS)).Append(':').Append(Constants.NEW_LINE);
                foreach (EnumMapWarningCode code in warningCodes)
                    sb.Append("    ").Append(code.ToString()).Append('\n');
            }

            void HandleErrors()
            {
                if (errorCodes.Count == 0)
                    return;

                sb.Append(GuiLocManager.GetLoc(EnumLocKey.ERRORS)).Append(':').Append(Constants.NEW_LINE);
                foreach (EnumMapErrorCode code in errorCodes)
                {
                    sb.Append("    ").Append(code.ToString());
                    HandleMeta(code);
                    sb.Append('\n');
                }
            }

            void HandleMeta(EnumMapErrorCode code)
            {
                if (!(code == EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_LAND_PROVINCE ||
                    code == EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_STATE))
                    return;

                if (!pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                    return;

                int color = MapManager.GetColor(pos);
                if (!ProvinceManager.TryGet(color, out var province))
                    return;

                List<Building> errorBuildings = null;
                if (code == EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_LAND_PROVINCE)
                    errorBuildings = MapCheckerCoastalBuildingInNotCoastalPlace.GetErrorBuildings(province);
                else if (code == EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_STATE)
                    errorBuildings = MapCheckerCoastalBuildingInNotCoastalPlace.GetErrorBuildings(province.State);

                if (errorBuildings.Count == 0)
                    return;

                sb.Append(" (");
                foreach (var building in errorBuildings)
                    sb.Append(building.Name).Append(", ");
                if (errorBuildings.Count > 0)
                    sb.Length = sb.Length - 2;
                sb.Append(")");
            }

            void HandleBuildings()
            {
                if (buildings.Count == 0)
                    return;

                sb.Append(GuiLocManager.GetLoc(EnumLocKey.BUILDINGS)).Append(':').Append(Constants.NEW_LINE);
                foreach (var data in buildings)
                    sb.Append("    ").Append(data).Append('\n');
            }
        }
    }
}
