using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.mapChecks.warnings.checkers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolFill : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.FILL;

        public MapToolFill(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      key = Keys.F,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX | (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_SELECTED_BOUND
              )
        { }

        public override bool isHandlingMouseMove() => true;

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.PROVINCES, EnumEditLayer.HEIGHT_MAP
        };
        public override Func<ICollection> GetParametersProvider() => null;
        public override Func<ICollection> GetValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            int newColor = 0;
            if (Control.ModifierKeys == Keys.Shift)
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
                newColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
            else if (mouseEventArgs.Button == MouseButtons.Right)
                newColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
            else
                return false;

            if (HandlePixel(pos, bounds, enumEditLayer, newColor, out var redo, out var undo))
            {
                if (redo != null && undo != null)
                    MapManager.ActionsBatch.AddWithExecute(redo, undo);
            }
            return true;
        }

        private bool HandlePixel(Point2D pos, Bounds4US bounds, EnumEditLayer enumEditLayer, int newColor, out Action redo, out Action undo)
        {
            redo = null;
            undo = null;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES:
                    return HandlePixelProvinces(pos, bounds, newColor, out redo, out undo);
                case EnumEditLayer.HEIGHT_MAP:
                    return HandlePixelHeightMap(pos, bounds, newColor, out redo, out undo);
            }

            return false;
        }

        private bool HandlePixelProvinces(Point2D pos, Bounds4US bounds, int newColor, out Action redo, out Action undo)
        {
            redo = null;
            undo = null;

            if (!ProvinceManager.TryGetProvince(newColor, out Province province))
            {
                var text = GuiLocManager.GetLoc(
                    EnumLocKey.WARNINGS_TRIED_TO_PAINT_WITH_UNKNOWN_PROVINCE_COLOR,
                    new Dictionary<string, string> { { "{color}", new Color3B(newColor).ToString() } }
                );

                var result = MessageBoxUtils.ShowQuestionChooseAction(text, MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                    throw new CancelException();

                ProvinceManager.CreateNewProvince(newColor);
            }

            HashSet<Value2US> positions = null;

            int prevColor = TextureManager.provinces.GetColor(pos);

            if (bounds.HasSpace())
                positions = bounds.ToPositions((ushort)MapManager.MapSize.x, (ushort)MapManager.MapSize.y);
            else if (newColor != prevColor)
                positions = TextureManager.provinces.NewGetRGBPositions((ushort)pos.x, (ushort)pos.y, TextureManager._24bppRgb);
            if (positions == null || positions.Count == 0)
                return false;


            Action<int, int> action = (p, n) =>
            {
                if (p == n)
                    return;
                TextureManager.provinces.RGBFill(MapManager.ProvincesPixels, positions, n, TextureManager._24bppRgb);
                foreach (var fillPos in positions)
                    MapCheckerHeightMapMismatches.HandlePixel(fillPos.x, fillPos.y);
            };

            redo = () => action(prevColor, newColor);
            undo = () => action(newColor, prevColor);

            return true;
        }

        private bool HandlePixelHeightMap(Point2D pos, Bounds4US bounds, int newColor, out Action redo, out Action undo)
        {
            redo = null;
            undo = null;

            HashSet<Value2US> positions = null;
            int prevColor = TextureManager.height.GetColor(pos);

            if (bounds.HasSpace())
                positions = bounds.ToPositions((ushort)MapManager.MapSize.x, (ushort)MapManager.MapSize.y);
            else if (newColor != prevColor)
                positions = TextureManager.height.NewGetRGBPositions((ushort)pos.x, (ushort)pos.y, TextureManager._8bppGrayscale);
            if (positions == null || positions.Count == 0)
                return false;


            Action<int, int> action = (p, n) =>
            {
                if (p == n)
                    return;
                TextureManager.height.RGBFill(MapManager.HeightsPixels, positions, n, TextureManager._8bppGrayscale);
                foreach (var fillPos in positions)
                    MapCheckerHeightMapMismatches.HandlePixel(fillPos.x, fillPos.y);
            };

            redo = () => action(prevColor, newColor);
            undo = () => action(newColor, prevColor);

            return true;
        }
    }
}
