using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.hoiDataObjects.map.tools;
using System.Collections.Generic;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced;
using HOI4ModBuilder.src.tools.map.advanced;
using System;
using HOI4ModBuilder.src.tools.map.mouseHandleable;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.utils.structs;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    class MapToolsManager
    {
        private static Dictionary<EnumTool, MapTool> _mapTools = new Dictionary<EnumTool, MapTool>();

        public static void Init()
        {
            new MapToolCursor(_mapTools);
            new MapToolRectangle(_mapTools);
            new MapToolBrush(_mapTools);
            new MapToolFill(_mapTools);
            new MapToolEraser(_mapTools);
            new MapToolPipette(_mapTools);
            new MapToolProvinceType(_mapTools);
            new MapToolProvinceCoastal(_mapTools);
            new MapToolTerrain(_mapTools);
            new MapToolProvinceContinent(_mapTools);
            new MapToolProvinceState(_mapTools);
            new MapToolProvinceRegion(_mapTools);
            new MapToolStateCategory(_mapTools);
            new MapToolStateOwner(_mapTools);
            new MapToolStateController(_mapTools);
            new MapToolStateCoreOf(_mapTools);
            new MapToolStateClaimBy(_mapTools);
            new MapToolBuildings(_mapTools);
            new MapToolAiArea(_mapTools);
            new MapToolVictoryPoints(_mapTools);

            new DebugTool();

            new MergeProvincesTool();
            new RailwayTool();
            new SupplyNodeTool();
        }

        public static void HandleTool(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, EnumTool enumTool, Bounds4US bounds, string parameter, string value
        )
        {
            Logger.TryOrLog(() =>
            {
                if (!MainForm.IsFirstLoaded)
                    return;
                if (_mapTools.TryGetValue(enumTool, out MapTool mapTool))
                    mapTool.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value);

                switch (enumTool)
                {
                    case EnumTool.ELLIPSE: break;
                    case EnumTool.MAGIC_WAND: break;
                }
            });
        }

        public static bool TryGetMapTool(EnumTool enumTool, out MapTool mapTool)
            => _mapTools.TryGetValue(enumTool, out mapTool);

        public static bool TryGetMapToolParametersProvider(EnumTool enumTool, out Func<ICollection> parametersProvider)
        {
            if (!_mapTools.TryGetValue(enumTool, out var tool) || tool.GetParametersProvider() == null)
            {
                parametersProvider = null;
                return false;
            }

            parametersProvider = tool.GetParametersProvider();
            return true;
        }
        public static bool TryGetMapToolValuesProvider(EnumTool enumTool, out Func<ICollection> valuesProvider)
        {
            if (!_mapTools.TryGetValue(enumTool, out var tool) || tool.GetValuesProvider() == null)
            {
                valuesProvider = null;
                return false;
            }

            valuesProvider = tool.GetValuesProvider();
            return true;
        }

        public static bool ShouldRecalculateAllText(EnumMainLayer mainLayer, EnumTool enumTool)
        {
            if (mainLayer == EnumMainLayer.BUILDINGS)
                return true;

            TryGetMapTool(enumTool, out var tool);

            if (tool == null || tool.RecalculateTextOnParameterChange == null)
                return true;

            return Utils.Contains(tool.RecalculateTextOnParameterChange, mainLayer);
        }
    }

    public abstract class MapTool : IMouseHandleableMapTool
    {
        public EnumTool EnumToolType { get; private set; }
        public HotKey HotKey { get; private set; }
        public EnumMainLayer[] RecalculateTextOnParameterChange { get; private set; }
        protected static bool[] _isInDialog = new bool[1];
        private int _handleCheckFlags;

        public MapTool(Dictionary<EnumTool, MapTool> mapTools, EnumTool enumTool, EnumMainLayer[] recalculateTextOnParameterChange, HotKey hotKey, int handleCheckFlags)
        {
            EnumToolType = enumTool;
            HotKey = hotKey;

            RecalculateTextOnParameterChange = recalculateTextOnParameterChange;

            mapTools[EnumToolType] = this;

            HotKey?.SubscribeTabKeyEvent(EnumTabPage.MAP);
            _handleCheckFlags = handleCheckFlags;
        }

        public abstract EnumEditLayer[] GetAllowedEditLayers();
        public bool IsEditLayerAllowed(EnumEditLayer editLayer)
        {
            var values = GetAllowedEditLayers();
            if (values == null)
                return true;

            foreach (var layer in values)
                if (layer == editLayer)
                    return true;

            return false;
        }

        public string GetAllowedEditLayersLocalizedString()
        {
            var values = GetAllowedEditLayers();
            if (values == null)
                return GuiLocManager.GetLoc(EnumLocKey.ALL_EDIT_LAYERS_IS_ALLOWED);

            var sb = new StringBuilder();

            foreach (var layer in values)
                sb.Append('\"').Append(GuiLocManager.GetLoc(layer.ToString())).Append('\"').Append(", ");

            if (sb.Length > 2)
                sb.Length -= 2;

            return sb.ToString();
        }

        public virtual bool Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value)
        {
            if (_isInDialog[0])
                return false;


            if ((_handleCheckFlags & (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX) != 0 &&
                !pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                return false;

            if ((_handleCheckFlags & (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_SELECTED_BOUND) != 0 &&
                bounds.HasSpace() && !bounds.Inbounds(pos))
                return false;

            if (mouseState != EnumMouseState.DOWN ||
                mouseState == EnumMouseState.DOWN && mouseEventArgs.Button != MouseButtons.Left && mouseEventArgs.Button != MouseButtons.Right)
                return true;

            if (!IsEditLayerAllowed(enumEditLayer))
            {
                _isInDialog[0] = true;
                Task.Run(() =>
                {
                    var title = GuiLocManager.GetLoc(EnumLocKey.MAP_TOOL_EDIT_LAYER_IS_NOT_ALLOWED_TITLE);
                    var text = GuiLocManager.GetLoc(
                        EnumLocKey.MAP_TOOL_EDIT_LAYER_IS_NOT_ALLOWED,
                        new Dictionary<string, string> {
                            { "{currentEditLayer}", GuiLocManager.GetLoc(enumEditLayer.ToString()) },
                            { "{allowedEditLayers}", GetAllowedEditLayersLocalizedString() }
                        }
                    );

                    MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    _isInDialog[0] = false;
                });
                return false;
            }

            return true;
        }

        public abstract Func<ICollection> GetParametersProvider();
        public abstract Func<ICollection> GetValuesProvider();
    }

    public interface IMouseHandleableMapTool
    {
        bool Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value);
        EnumEditLayer[] GetAllowedEditLayers();
        Func<ICollection> GetParametersProvider();
        Func<ICollection> GetValuesProvider();
    }

    public enum EnumMapToolHandleChecks
    {
        CHECK_INBOUNDS_MAP_BOX = 0b0001,
        CHECK_INBOUNDS_SELECTED_BOUND = 0b0010
    }
}
