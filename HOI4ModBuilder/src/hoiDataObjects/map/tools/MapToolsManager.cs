using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.hoiDataObjects.map.tools;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    class MapToolsManager
    {
        private static Dictionary<EnumTool, IMouseHandleableMapTool> _mapTools = new Dictionary<EnumTool, IMouseHandleableMapTool>();

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
            new MapToolProvinceTerrain(_mapTools);
            new MapToolProvinceContinent(_mapTools);
            new MapToolProvinceState(_mapTools);
            new MapToolProvinceRegion(_mapTools);
            new MapToolStateCategory(_mapTools);
            new MapToolBuildings(_mapTools);
        }

        public static void HandleTool(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, EnumTool enumTool, Bounds4US bounds, string toolParameter)
        {
            Logger.TryOrLog(() =>
            {
                if (_mapTools.TryGetValue(enumTool, out IMouseHandleableMapTool mapTool)) 
                    mapTool.Handle(buttons, mouseState, pos, enumEditLayer, bounds, toolParameter);

                switch (enumTool)
                {
                    case EnumTool.ELLIPSE: break;
                    case EnumTool.MAGIC_WAND: break;
                }
            });
        }
    }

    public interface IMouseHandleableMapTool
    {
        void Handle(MouseButtons button, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter);
    }
}
