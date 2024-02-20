using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolEraser : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.ERASER;

        public MapToolEraser(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;

            MainForm.SubscribeTabKeyEvent(
                MainForm.Instance.TabPage_Map,
                Keys.E,
                (sender, e) =>
                {
                    if (e.Control || e.Shift || e.Alt) return;
                    MainForm.Instance.SetSelectedTool(enumTool);
                }
            );
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            int prevColor = 0, newColor;
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            if (Control.ModifierKeys == Keys.Shift) return;
            if (bounds.HasSpace() && !bounds.Inbounds(pos)) return;

            if (buttons == MouseButtons.Left) newColor = Utils.ArgbToInt(255, 255, 255, 255);
            else return;
            Action<Point2D, int> action = null;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES:
                    prevColor = TextureManager.provinces.GetColor(pos);

                    action = (p, c) =>
                    {
                        TextureManager.provinces.SetColor(p, c);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16) }; //BGR
                        TextureManager.provinces.texture.Update(TextureManager._24bppRgb, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;

                case EnumEditLayer.RIVERS:
                    prevColor = TextureManager.rivers.GetColor(pos);

                    action = (p, c) =>
                    {
                        TextureManager.rivers.SetColor(p, c);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16), 0 }; //BGRA
                        TextureManager.rivers.texture.Update(TextureManager._32bppArgb, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;

                case EnumEditLayer.HEIGHT_MAP:
                    prevColor = TextureManager.height.GetColor(pos);

                    action = (p, c) =>
                    {
                        TextureManager.height.WriteByte(p, (byte)c);
                        TextureManager.height.texture.Update(TextureManager._8bppIndexed, (int)p.x, (int)p.y, 1, 1, new byte[] { (byte)c });
                    };
                    break;
            }

            if (action != null)
            {
                MapManager.ActionsBatch.AddWithExecute(
                    () => action(pos, newColor),
                    () => action(pos, prevColor)
                );
            }
        }
    }
}
