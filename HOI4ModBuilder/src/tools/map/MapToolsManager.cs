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
                if (!MainForm.firstLoad)
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
    }

    public abstract class MapTool : IMouseHandleableMapTool
    {
        public EnumTool EnumToolType { get; private set; }
        public HotKey HotKey { get; private set; }
        private readonly KeyEventHandler _hotKeyEvent;
        protected static bool[] _isInDialog = new bool[1];
        private EnumEditLayer[] _allowedEditLayers;
        private int _handleCheckFlags;

        public bool IsEditLayerAllowed(EnumEditLayer editLayer)
        {
            if (_allowedEditLayers == null)
                return true;

            foreach (var layer in _allowedEditLayers)
                if (layer == editLayer)
                    return true;

            return false;
        }
        public string GetAllowedEditLayersLocalizedString()
        {
            if (_allowedEditLayers == null)
                return GuiLocManager.GetLoc(EnumLocKey.ALL_EDIT_LAYERS_IS_ALLOWED);

            var sb = new StringBuilder();

            foreach (var layer in _allowedEditLayers)
                sb.Append('\"').Append(GuiLocManager.GetLoc(layer.ToString())).Append('\"').Append(", ");

            if (sb.Length > 2)
                sb.Length -= 2;

            return sb.ToString();
        }

        public MapTool(Dictionary<EnumTool, MapTool> mapTools, EnumTool enumTool, HotKey hotKey, Action<KeyEventArgs> hotKeyEvent, EnumEditLayer[] allowedEditLayers, int handleCheckFlags)
        {
            EnumToolType = enumTool;
            HotKey = hotKey;

            mapTools[EnumToolType] = this;

            _hotKeyEvent = (sender, e) =>
            {
                if (HotKey.CheckKeys(e)) hotKeyEvent(e);
            };

            if (HotKey.key != Keys.None)
            {
                MainForm.SubscribeTabKeyEvent(
                    EnumTabPage.MAP,
                    HotKey.key,
                    _hotKeyEvent
                );
            }

            _allowedEditLayers = allowedEditLayers;
            _handleCheckFlags = handleCheckFlags;
        }

        public virtual bool Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value)
        {
            if (_isInDialog[0])
                return false;

            if (mouseState != EnumMouseState.DOWN ||
                mouseState == EnumMouseState.DOWN && mouseEventArgs.Button != MouseButtons.Left && mouseEventArgs.Button != MouseButtons.Right)
                return true;

            if ((_handleCheckFlags & (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX) != 0 &&
                !pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                return false;

            if ((_handleCheckFlags & (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_SELECTED_BOUND) != 0 &&
                bounds.HasSpace() && !bounds.Inbounds(pos))
                return false;

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
    }

    public interface IMouseHandleableMapTool
    {
        bool Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value);
    }

    public enum EnumMapToolHandleChecks
    {
        CHECK_INBOUNDS_MAP_BOX = 0b0001,
        CHECK_INBOUNDS_SELECTED_BOUND = 0b0010
    }
}
