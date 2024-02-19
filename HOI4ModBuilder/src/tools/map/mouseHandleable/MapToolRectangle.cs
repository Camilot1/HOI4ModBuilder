using HOI4ModBuilder.managers;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolRectangle : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.RECTANGLE;

        public MapToolRectangle(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;

            MainForm.SubscribeTabKeyEvent(
                MainForm.Instance.TabPage_Map,
                Keys.S,
                (sender, e) =>
                {
                    if (e.Control || e.Shift || e.Alt) return;
                    EnumTool currentTool = MainForm.Instance.SelectedTool;
                    if (e.Modifiers != Keys.Control && currentTool < EnumTool.RECTANGLE || currentTool > EnumTool.MAGIC_WAND)
                        MainForm.Instance.SetSelectedTool(enumTool);
                }
            );
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (buttons != MouseButtons.Left) return;

            ushort x = (ushort)pos.x;
            ushort y = (ushort)pos.y;
            if (x < 0) x = 0;
            else if (x > MapManager.MapSize.x) x = (ushort)MapManager.MapSize.x;

            if (y < 0) y = 0;
            else if (y > MapManager.MapSize.y) y = (ushort)MapManager.MapSize.y;

            switch (mouseState)
            {
                case EnumMouseState.DOWN:
                    MapManager.bounds.left = x;
                    MapManager.bounds.right = x;
                    MapManager.bounds.top = y;
                    MapManager.bounds.bottom = y;
                    break;
                case EnumMouseState.MOVE:
                    MapManager.bounds.right = (x < MapManager.bounds.left) ? x : (ushort)(x + 1);
                    MapManager.bounds.bottom = (y < MapManager.bounds.top) ? y : (ushort)(y + 1);
                    break;
                case EnumMouseState.UP:
                    MapManager.bounds.FixDimensions();
                    break;
            }
        }
    }
}
