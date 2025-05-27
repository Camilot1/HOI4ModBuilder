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

        public MapTool(Dictionary<EnumTool, MapTool> mapTools, EnumTool enumTool, HotKey hotKey, Action<KeyEventArgs> hotKeyEvent)
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
        }

        public abstract void Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value);
    }

    public interface IMouseHandleableMapTool
    {
        void Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value);
    }
}
