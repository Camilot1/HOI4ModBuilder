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
using System.Collections;

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
            EnumEditLayer.PROVINCES
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

            int prevColor = 0, newColor = 0;
            if (Control.ModifierKeys == Keys.Shift)
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
                newColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
            else if (mouseEventArgs.Button == MouseButtons.Right)
                newColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
            else
                return false;

            Action<int, int> action;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES:

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
                        }); ;
                        return false;
                    }

                    HashSet<Value2US> positions = null;

                    prevColor = TextureManager.provinces.GetColor(pos);

                    if (bounds.HasSpace())
                        positions = bounds.ToPositions((ushort)MapManager.MapSize.x, (ushort)MapManager.MapSize.y);
                    else if (newColor != prevColor)
                        positions = TextureManager.provinces.NewGetRGBPositions((ushort)pos.x, (ushort)pos.y);
                    if (positions == null || positions.Count == 0)
                        return false;


                    action = (p, n) =>
                    {
                        if (p == n)
                            return;
                        TextureManager.provinces.RGBFill(MapManager.ProvincesPixels, positions, n);
                    };

                    MapManager.ActionsBatch.AddWithExecute(
                        () => action(prevColor, newColor),
                        () => action(newColor, prevColor)
                    );
                    break;
            }

            return true;
        }
    }
}
