using HOI4ModBuilder.managers;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;
using System.Collections;
using System;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolRectangle : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.RECTANGLE;

        public MapToolRectangle(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      hotKeyEvent = (e) =>
                      {
                          EnumTool currentTool = MainForm.Instance.SelectedTool;
                          if (e.Modifiers != Keys.Control &&
                                currentTool < EnumTool.RECTANGLE ||
                                currentTool > EnumTool.MAGIC_WAND)
                              MainForm.Instance.SetSelectedToolWithRefresh(enumTool);
                      }
                  },
                  0
              )
        { }

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

            if (mouseEventArgs.Button != MouseButtons.Left)
                return false;

            ushort x = (ushort)pos.x;
            ushort y = (ushort)pos.y;
            if (x < 0) x = 0;
            else if (x > MapManager.MapSize.x) x = (ushort)MapManager.MapSize.x;

            if (y < 0) y = 0;
            else if (y > MapManager.MapSize.y) y = (ushort)MapManager.MapSize.y;

            switch (mouseState)
            {
                case EnumMouseState.DOWN:
                    MapManager.selectBounds.left = x;
                    MapManager.selectBounds.right = x;
                    MapManager.selectBounds.top = y;
                    MapManager.selectBounds.bottom = y;
                    break;
                case EnumMouseState.MOVE:
                    MapManager.selectBounds.right = (x < MapManager.selectBounds.left) ? x : (ushort)(x + 1);
                    MapManager.selectBounds.bottom = (y < MapManager.selectBounds.top) ? y : (ushort)(y + 1);
                    break;
                case EnumMouseState.UP:
                    MapManager.selectBounds.FixDimensions();
                    break;
            }

            return true;
        }
    }
}
