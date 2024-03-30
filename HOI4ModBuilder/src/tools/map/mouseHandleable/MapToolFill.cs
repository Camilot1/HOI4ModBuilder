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
    class MapToolFill : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.FILL;

        public MapToolFill(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { key = Keys.F },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public new void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            int prevColor = 0, newColor = 0;
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            if (Control.ModifierKeys == Keys.Shift) return;
            if (bounds.HasSpace() && !bounds.Inbounds(pos)) return;

            if (buttons == MouseButtons.Left) newColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
            else if (buttons == MouseButtons.Right) newColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
            else return;

            Action<int, int> action;

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
                        }); ;
                        return;
                    }

                    HashSet<Value2US> positions = null;

                    if (bounds.HasSpace()) positions = bounds.ToPositions((ushort)MapManager.MapSize.x, (ushort)MapManager.MapSize.y);
                    else positions = TextureManager.provinces.NewGetRGBPositions((ushort)pos.x, (ushort)pos.y);
                    if (positions == null || positions.Count == 0) return;

                    prevColor = TextureManager.provinces.GetColor(pos);

                    action = (p, n) =>
                    {
                        if (p == n) return;
                        TextureManager.provinces.RGBFill(MapManager.ProvincesPixels, positions, n);
                    };

                    MapManager.ActionsBatch.AddWithExecute(
                        () => action(prevColor, newColor),
                        () => action(newColor, prevColor)
                    );
                    break;
            }
        }
    }
}
