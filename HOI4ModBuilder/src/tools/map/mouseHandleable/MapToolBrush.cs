using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.tools.brushes;

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

        public override void Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            int newColor;

            if (!pos.InboundsPositiveBox(MapManager.MapSize))
                return;
            if (Control.ModifierKeys == Keys.Shift)
                return;
            if (bounds.HasSpace() && !bounds.Inbounds(pos))
                return;

            if (mouseEventArgs.Button == MouseButtons.Left)
                newColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
            else if (mouseEventArgs.Button == MouseButtons.Right)
                newColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
            else
                return;

            if (!BrushManager.TryGetBrush(parameter, out var brush))
                return;

            List<Action> redoActions = new List<Action>();
            List<Action> undoActions = new List<Action>();

            double snappedCenterX;
            double snappedCenterY;

            if (brush.OriginalWidth % 2 != 0)
                snappedCenterX = Math.Floor(pos.x);
            else
                snappedCenterX = Math.Round(pos.x);

            if (brush.OriginalHeight % 2 != 0)
                snappedCenterY = Math.Floor(pos.y);
            else
                snappedCenterY = Math.Round(pos.y);

            foreach (var point in brush.pixels)
            {
                int targetX = (int)(point.x + snappedCenterX);
                int targetY = (int)(point.y + snappedCenterY);

                if (HandlePixel(targetX, targetY, enumEditLayer, newColor, out var redo, out var undo))
                {
                    redoActions.Add(redo);
                    undoActions.Add(undo);
                }
            }

            if (redoActions.Count > 0)
            {
                MapManager.ActionsBatch.AddWithExecute(
                    () =>
                    {
                        foreach (var redo in redoActions)
                            redo();
                    },
                    () =>
                    {
                        foreach (var undo in undoActions)
                            undo();
                    }
                );
            }
        }

        private bool HandlePixel(int x, int y, EnumEditLayer enumEditLayer, int newColor, out Action redo, out Action undo)
        {
            redo = null;
            undo = null;

            if (x < 0 || x > MapManager.MapSize.x || y < 0 || y > MapManager.MapSize.y)
                return false;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES:
                    return HandlePixelProvinces(x, y, newColor, out redo, out undo);
                case EnumEditLayer.RIVERS:
                    return HandlePixelRivers(x, y, newColor, out redo, out undo);
                case EnumEditLayer.TERRAIN_MAP:
                    return HandlePixelTerrainMap(x, y, newColor, out redo, out undo);
                case EnumEditLayer.TREES_MAP:
                    return HandlePixelTreesMap(x, y, newColor, out redo, out undo);
                case EnumEditLayer.CITIES_MAP:
                    return HandlePixelCitiesMap(x, y, newColor, out redo, out undo);
                case EnumEditLayer.HEIGHT_MAP:
                    return HandlePixelHeightMap(x, y, newColor, out redo, out undo);
            }

            return false;
        }

        private bool HandlePixelProvinces(int x, int y, int newColor, out Action redo, out Action undo)
        {
            redo = null; undo = null;
            int i = x + y * MapManager.MapSize.x;

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
                return false;
            }

            int[] pixels = MapManager.ProvincesPixels;
            int prevColor = pixels[i];
            if (prevColor == newColor)
                return false;

            redo = () =>
            {
                pixels[i] = newColor;
                TextureManager.provinces.SetColor(x, y, newColor);
                byte[] data = { (byte)newColor, (byte)(newColor >> 8), (byte)(newColor >> 16) }; //BGR
                TextureManager.provinces.texture.Update(TextureManager._24bppRgb, x, y, 1, 1, data);
            };
            undo = () =>
            {
                pixels[i] = prevColor;
                TextureManager.provinces.SetColor(x, y, prevColor);
                byte[] data = { (byte)prevColor, (byte)(prevColor >> 8), (byte)(prevColor >> 16) }; //BGR
                TextureManager.provinces.texture.Update(TextureManager._24bppRgb, x, y, 1, 1, data);
            };
            return true;
        }

        private bool HandlePixelRivers(int x, int y, int newColor, out Action redo, out Action undo)
        {
            redo = null; undo = null;

            int prevColor = TextureManager.rivers.GetColor(x, y);
            if (prevColor == newColor)
                return false;

            redo = () =>
            {
                TextureManager.rivers.SetColor(x, y, newColor);
                byte[] data = { (byte)newColor, (byte)(newColor >> 8), (byte)(newColor >> 16), (byte)(newColor >> 24) }; //BGRA
                TextureManager.rivers.texture.Update(TextureManager._32bppArgb, x, y, 1, 1, data);
            };
            undo = () =>
            {
                TextureManager.rivers.SetColor(x, y, prevColor);
                byte[] data = { (byte)prevColor, (byte)(prevColor >> 8), (byte)(prevColor >> 16), (byte)(prevColor >> 24) }; //BGRA
                TextureManager.rivers.texture.Update(TextureManager._32bppArgb, x, y, 1, 1, data);
            };
            return true;
        }

        private bool HandlePixelTerrainMap(int x, int y, int newColor, out Action redo, out Action undo)
        {
            redo = null; undo = null;

            byte prevByte = TextureManager.terrain.GetByte(x, y);
            int prevColor = TextureManager.terrain.GetColor(x, y);
            if (prevColor == newColor)
                return false;

            if (!TextureManager.terrain.GetIndex(newColor, out var newByte))
                return false;

            redo = () =>
            {
                TextureManager.terrain.WriteByte(x, y, newByte);
                byte[] data = { (byte)newColor, (byte)(newColor >> 8), (byte)(newColor >> 16) }; //BGR
                TextureManager.terrain.texture.Update(TextureManager._8bppIndexed, x, y, 1, 1, data);
            };
            undo = () =>
            {
                TextureManager.terrain.WriteByte(x, y, prevByte);
                byte[] data = { (byte)prevColor, (byte)(prevColor >> 8), (byte)(prevColor >> 16) }; //BGR
                TextureManager.terrain.texture.Update(TextureManager._8bppIndexed, x, y, 1, 1, data);
            };
            return true;
        }

        private bool HandlePixelTreesMap(int x, int y, int newColor, out Action redo, out Action undo)
        {
            redo = null; undo = null;

            byte prevByte = TextureManager.trees.GetByte(x, y);
            int prevColor = TextureManager.trees.GetColor(x, y);
            if (prevColor == newColor)
                return false;

            if (!TextureManager.trees.GetIndex(newColor, out var newByte))
                return false;

            redo = () =>
            {
                TextureManager.trees.WriteByte(x, y, newByte);
                byte[] data = { (byte)newColor, (byte)(newColor >> 8), (byte)(newColor >> 16) }; //BGR
                TextureManager.trees.texture.Update(TextureManager._8bppIndexed, x, y, 1, 1, data);
            };
            undo = () =>
            {
                TextureManager.trees.WriteByte(x, y, prevByte);
                byte[] data = { (byte)prevColor, (byte)(prevColor >> 8), (byte)(prevColor >> 16) }; //BGR
                TextureManager.trees.texture.Update(TextureManager._8bppIndexed, x, y, 1, 1, data);
            };
            return true;
        }

        private bool HandlePixelCitiesMap(int x, int y, int newColor, out Action redo, out Action undo)
        {
            redo = null; undo = null;

            byte prevByte = TextureManager.cities.GetByte(x, y);
            int prevColor = TextureManager.cities.GetColor(x, y);
            if (prevColor == newColor)
                return false;

            if (!TextureManager.cities.GetIndex(newColor, out var newByte))
                return false;

            redo = () =>
            {
                TextureManager.cities.WriteByte(x, y, newByte);
                byte[] data = { (byte)newColor, (byte)(newColor >> 8), (byte)(newColor >> 16) }; //BGR
                TextureManager.cities.texture.Update(TextureManager._8bppIndexed, x, y, 1, 1, data);
            };
            return true;

        }
        private bool HandlePixelHeightMap(int x, int y, int newColor, out Action redo, out Action undo)
        {
            redo = null; undo = null;

            byte prevByte = TextureManager.height.GetByte(x, y);
            byte newByte = (byte)newColor;
            if (prevByte == newByte)
                return false;

            redo = () =>
            {
                TextureManager.height.WriteByte(x, y, newByte);
                TextureManager.height.texture.Update(TextureManager._8bppGrayscale, x, y, 1, 1, new byte[] { newByte });
            };
            undo = () =>
            {
                TextureManager.height.WriteByte(x, y, prevByte);
                TextureManager.height.texture.Update(TextureManager._8bppGrayscale, x, y, 1, 1, new byte[] { prevByte });
            };
            return true;

        }
    }
}
