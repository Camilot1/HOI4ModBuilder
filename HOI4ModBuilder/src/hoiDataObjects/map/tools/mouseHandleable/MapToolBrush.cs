using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolBrush : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.BRUSH;

        public MapToolBrush(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;

            MainForm.SubscribeTabKeyEvent(
                MainForm.Instance.TabPage_Map,
                Keys.B,
                (sender, e) => MainForm.Instance.SetSelectedTool(enumTool)
            );
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            byte newByte = 0, prevByte = 0;
            int newColor = 0, prevColor = 0;

            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            if (Control.ModifierKeys == Keys.Shift) return;
            if (bounds.HasSpace() && !bounds.Inbounds(pos)) return;

            if (buttons == MouseButtons.Left) newColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
            else if (buttons == MouseButtons.Right) newColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
            else return;

            int i = (int)pos.x + (int)pos.y * MapManager.MapSize.x;
            Action<Point2D, byte, int> action = null;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES:

                    if (!ProvinceManager.TryGetProvince(newColor, out Province province))
                    {
                        Task.Run(() =>
                        {
                            DialogResult result = MessageBox.Show(
                                "Выполнена попытка залить область карты цветом несуществующей провинции. \n\nВыберите действие:\n" +
                                "ОК - создать новую провинцию\n" +
                                "Cancel - отменить заливку"
                                , "Выберите действие!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question
                            );

                            if (result == DialogResult.OK)
                            {
                                ProvinceManager.CreateNewProvince(newColor);
                            }
                        });
                        return;
                    }

                    int[] pixels = MapManager.ProvincesPixels;
                    prevColor = pixels[i];
                    if (prevColor == newColor) break;

                    action = (p, b, c) =>
                    {
                        pixels[i] = c;
                        TextureManager.provinces.SetColor(p, c);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16) }; //BGR
                        TextureManager.provinces.texture.Update(TextureManager._24bppRgb, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;
                case EnumEditLayer.RIVERS:
                    prevColor = TextureManager.rivers.GetColor(pos);
                    if (prevColor == newColor) break;

                    action = (p, b, c) =>
                    {
                        TextureManager.rivers.SetColor(p, c);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16), (byte)(c >> 24) }; //BGRA
                        TextureManager.rivers.texture.Update(TextureManager._32bppArgb, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;
                case EnumEditLayer.TERRAIN_MAP:
                    prevByte = TextureManager.terrain.GetByte(pos);
                    prevColor = TextureManager.terrain.GetColor(pos);
                    if (prevColor == newColor) break;

                    short tempNewIndex = TextureManager.terrain.GetIndex(newColor);
                    if (tempNewIndex < 0 || tempNewIndex > 255) break;
                    newByte = (byte)tempNewIndex;

                    action = (p, b, c) =>
                    {
                        TextureManager.terrain.WriteByte(p, b);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16) }; //BGR
                        TextureManager.terrain.texture.Update(TextureManager._8bppIndexed, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;
                case EnumEditLayer.HEIGHT_MAP:
                    prevByte = TextureManager.height.GetByte(pos);
                    newByte = (byte)newColor;
                    if (prevByte == newByte) break;

                    action = (p, b, c) =>
                    {
                        TextureManager.height.WriteByte(p, b);
                        TextureManager.height.texture.Update(TextureManager._8bppGrayscale, (int)p.x, (int)p.y, 1, 1, new byte[] { b });
                    };
                    break;
            }

            if (action != null)
            {
                action(pos, newByte, newColor);
                MapManager.actionPairs.Add(new ActionPair(() => action(pos, prevByte, prevColor), () => action(pos, newByte, newColor)));
            }
        }
    }
}
