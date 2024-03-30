using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolBrush : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.BRUSH;

        public MapToolBrush(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { key = Keys.B },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
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
                            var title = GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION);
                            var text = GuiLocManager.GetLoc(
                                EnumLocKey.WARNINGS_TRIED_TO_PAINT_WITH_UNKNOWN_PROVINCE_COLOR,
                                new Dictionary<string, string> { { "{color}", new Color3B(newColor).ToString() } }
                            );

                            if (MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                ProvinceManager.CreateNewProvince(newColor);
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

                    if (!TextureManager.terrain.GetIndex(newColor, out newByte)) break;

                    action = (p, b, c) =>
                    {
                        TextureManager.terrain.WriteByte(p, b);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16) }; //BGR
                        TextureManager.terrain.texture.Update(TextureManager._8bppIndexed, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;
                case EnumEditLayer.TREES_MAP:
                    prevByte = TextureManager.trees.GetByte(pos);
                    prevColor = TextureManager.trees.GetColor(pos);
                    if (prevColor == newColor) break;

                    if (!TextureManager.trees.GetIndex(newColor, out newByte)) break;

                    action = (p, b, c) =>
                    {
                        TextureManager.trees.WriteByte(p, b);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16) }; //BGR
                        TextureManager.trees.texture.Update(TextureManager._8bppIndexed, (int)p.x, (int)p.y, 1, 1, data);
                    };
                    break;
                case EnumEditLayer.CITIES_MAP:
                    prevByte = TextureManager.cities.GetByte(pos);
                    prevColor = TextureManager.cities.GetColor(pos);
                    if (prevColor == newColor) break;

                    if (!TextureManager.cities.GetIndex(newColor, out newByte)) break;

                    action = (p, b, c) =>
                    {
                        TextureManager.cities.WriteByte(p, b);
                        byte[] data = { (byte)c, (byte)(c >> 8), (byte)(c >> 16) }; //BGR
                        TextureManager.cities.texture.Update(TextureManager._8bppIndexed, (int)p.x, (int)p.y, 1, 1, data);
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
                MapManager.ActionsBatch.AddWithExecute(
                    () => action(pos, newByte, newColor),
                    () => action(pos, prevByte, prevColor)
                );
            }
        }
    }
}
