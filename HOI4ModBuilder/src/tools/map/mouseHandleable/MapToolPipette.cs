using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolPipette : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PIPETTE;

        public MapToolPipette(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;

            MainForm.SubscribeTabKeyEvent(
                MainForm.Instance.TabPage_Map,
                Keys.K,
                (sender, e) =>
                {
                    if (e.Control || e.Shift || e.Alt) return;
                    MainForm.Instance.SetSelectedTool(enumTool);
                }
            );
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            if (Control.ModifierKeys == Keys.Shift) return;

            int prevColor, newColor;
            Action<int> action;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES:
                    newColor = TextureManager.provinces.GetColor(pos);
                    break;
                case EnumEditLayer.RIVERS:
                    newColor = TextureManager.rivers.GetColor(pos);
                    break;
                case EnumEditLayer.TERRAIN_MAP:
                    newColor = TextureManager.terrain.GetColor(pos);
                    break;
                case EnumEditLayer.HEIGHT_MAP:
                    newColor = TextureManager.height.GetColor(pos);
                    break;
                default:
                    return;
            }

            if (buttons == MouseButtons.Left)
            {
                prevColor = MainForm.Instance.GetBrushFirstColor().ToArgb();

                action = (c) => MainForm.Instance.SetBrushFirstColor(Color.FromArgb(c));

                action(newColor);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevColor), () => action(newColor)));
            }
            else if (buttons == MouseButtons.Right)
            {
                prevColor = MainForm.Instance.GetBrushSecondColor().ToArgb(); ;

                action = (c) => MainForm.Instance.SetBrushSecondColor(Color.FromArgb(c));
                action(newColor);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevColor), () => action(newColor)));
            }
        }
    }
}
