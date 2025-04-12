using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolPipette : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PIPETTE;

        public MapToolPipette(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { key = Keys.K },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (!pos.InboundsPositiveBox(MapManager.MapSize))
                return;
            if (Control.ModifierKeys == Keys.Shift)
                return;

            int prevColor, newColor;
            Action<int> action;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES: newColor = TextureManager.provinces.GetColor(pos); break;
                case EnumEditLayer.RIVERS: newColor = TextureManager.rivers.GetColor(pos); break;
                case EnumEditLayer.TERRAIN_MAP: newColor = TextureManager.terrain.GetColor(pos); break;
                case EnumEditLayer.TREES_MAP: newColor = TextureManager.trees.GetColor(pos); break;
                case EnumEditLayer.CITIES_MAP: newColor = TextureManager.cities.GetColor(pos); break;
                case EnumEditLayer.HEIGHT_MAP: newColor = TextureManager.height.GetColor(pos); break;
                default:
                    return;
            }

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                prevColor = MainForm.Instance.GetBrushFirstColor().ToArgb();
                action = (c) => MainForm.Instance.SetBrushFirstColor(Color.FromArgb(c));

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newColor),
                    () => action(prevColor)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                prevColor = MainForm.Instance.GetBrushSecondColor().ToArgb();
                action = (c) => MainForm.Instance.SetBrushSecondColor(Color.FromArgb(c));

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newColor),
                    () => action(prevColor)
                );
            }
        }
    }
}
