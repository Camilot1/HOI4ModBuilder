using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.tools.brushes;
using HOI4ModBuilder.hoiDataObjects.map;
using System.Threading.Tasks;
using System.Collections;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolEraser : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.ERASER;

        public MapToolEraser(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      key = Keys.E,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX | (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_SELECTED_BOUND
              )
        { }

        public override bool isHandlingMouseMove() => true;

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.PROVINCES, EnumEditLayer.RIVERS, EnumEditLayer.TREES_MAP
        };
        public override Func<ICollection> GetParametersProvider()
            => () => BrushManager.GetBrushesNames(SettingsManager.Settings);
        public override Func<ICollection> GetValuesProvider() => null;

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
                newColor = Utils.ArgbToInt(255, 255, 255, 255);
            else
                return false;

            if (!BrushManager.TryGetBrush(SettingsManager.Settings, parameter, out var brush))
                return false;

            List<Action> redoActions = new List<Action>();
            List<Action> undoActions = new List<Action>();

            brush.ForEachPixel(value, pos, (x, y) =>
            {
                if (HandlePixel(x, y, enumEditLayer, newColor, out var redo, out var undo))
                {
                    redoActions.Add(redo);
                    undoActions.Add(undo);
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
                    return HandlePixelProvinces(x, y, out redo, out undo);
                case EnumEditLayer.RIVERS:
                    return HandlePixelRivers(x, y, out redo, out undo);
            }

            return false;
        }

        private bool HandlePixelProvinces(int x, int y, out Action redo, out Action undo)
        {
            redo = null; undo = null;
            int i = x + y * MapManager.MapSize.x;

            int[] pixels = MapManager.ProvincesPixels;
            int prevColor = pixels[i];
            int newColor = Utils.ArgbToInt(255, 255, 255, 255);

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

        private bool HandlePixelRivers(int x, int y, out Action redo, out Action undo)
        {
            redo = null; undo = null;

            int prevColor = TextureManager.rivers.GetColor(x, y);
            int newColor = Utils.ArgbToInt(255, 255, 255, 255);

            if (prevColor == newColor)
                return false;

            redo = () =>
            {
                TextureManager.rivers.SetColor(x, y, newColor);
                byte[] data = { (byte)newColor, (byte)(newColor >> 8), (byte)(newColor >> 16), 0 }; //BGRA
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
    }
}
