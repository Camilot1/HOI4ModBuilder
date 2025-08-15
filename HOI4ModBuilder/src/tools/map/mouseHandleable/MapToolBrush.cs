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
using HOI4ModBuilder.src.managers.mapChecks.warnings.checkers;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolBrush : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.BRUSH;

        public MapToolBrush(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey { key = Keys.B },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  new[] {
                      EnumEditLayer.PROVINCES, EnumEditLayer.RIVERS, EnumEditLayer.TERRAIN_MAP,
                      EnumEditLayer.TREES_MAP, EnumEditLayer.CITIES_MAP, EnumEditLayer.HEIGHT_MAP
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX | (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_SELECTED_BOUND
              )
        { }

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            int newColor;

            if (Control.ModifierKeys == Keys.Shift)
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
                newColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
            else if (mouseEventArgs.Button == MouseButtons.Right)
                newColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
            else return false;

            if (!BrushManager.TryGetBrush(SettingsManager.Settings, parameter, out var brush))
                return false;

            List<Action> redoActions = new List<Action>();
            List<Action> undoActions = new List<Action>();

            brush.ForEachPixel(value, pos, (x, y) =>
            {
                if (!_isInDialog[0] && HandlePixel(x, y, enumEditLayer, newColor, out var redo, out var undo))
                {
                    redoActions.Add(redo);
                    undoActions.Add(undo);

                    if (enumEditLayer == EnumEditLayer.HEIGHT_MAP)
                    {
                        redoActions.Add(() => MapCheckerHeightMapMismatches.HandlePixel(x, y));
                        undoActions.Add(() => MapCheckerHeightMapMismatches.HandlePixel(x, y));
                    }
                }
            });


            MapManager.ActionsBatch.AddWithExecute(redoActions, undoActions);
            return true;
        }

        private bool HandlePixel(int x, int y, EnumEditLayer enumEditLayer, int newColor, out Action redo, out Action undo)
        {
            redo = null;
            undo = null;

            if (x < 0 || x >= MapManager.MapSize.x || y < 0 || y >= MapManager.MapSize.y)
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
                _isInDialog[0] = true;
                Task.Run(() =>
                {
                    var title = GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION);
                    var text = GuiLocManager.GetLoc(
                        EnumLocKey.WARNINGS_TRIED_TO_PAINT_WITH_UNKNOWN_PROVINCE_COLOR,
                        new Dictionary<string, string> { { "{color}", new Color3B(newColor).ToString() } }
                    );

                    if (MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        ProvinceManager.CreateNewProvince(newColor);
                    _isInDialog[0] = false;
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
            int i = x + y * MapManager.MapSize.x;

            byte prevByte = TextureManager.height.GetByte(x, y);
            byte newByte = (byte)newColor;
            if (prevByte == newByte)
                return false;

            byte[] pixels = MapManager.HeightsPixels;
            redo = () =>
            {
                pixels[i] = newByte;
                TextureManager.height.WriteByte(x, y, newByte);
                TextureManager.height.texture.Update(TextureManager._8bppGrayscale, x, y, 1, 1, new byte[] { newByte });
            };
            undo = () =>
            {
                pixels[i] = prevByte;
                TextureManager.height.WriteByte(x, y, prevByte);
                TextureManager.height.texture.Update(TextureManager._8bppGrayscale, x, y, 1, 1, new byte[] { prevByte });
            };
            return true;

        }
    }
}
