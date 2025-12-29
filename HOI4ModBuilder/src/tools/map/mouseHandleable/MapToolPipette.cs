using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;
using System.Collections;
using HOI4ModBuilder.src.managers.texture;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolPipette : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PIPETTE;

        public MapToolPipette(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      key = Keys.K,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override bool isHandlingMouseMove() => true;

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.PROVINCES, EnumEditLayer.RIVERS, EnumEditLayer.TERRAIN_MAP,
            EnumEditLayer.TREES_MAP, EnumEditLayer.CITIES_MAP, EnumEditLayer.HEIGHT_MAP
        };
        public override Func<ICollection> GetParametersProvider() => null;
        public override Func<ICollection> GetParameterValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;
            if (Control.ModifierKeys == Keys.Shift)
                return false;

            int prevColor, newColor = 0;
            Action<int> action;

            switch (enumEditLayer)
            {
                case EnumEditLayer.PROVINCES: newColor = TextureManager.provinces.GetColor(pos); break;
                case EnumEditLayer.RIVERS: newColor = TextureManager.rivers.GetColor(pos); break;
                case EnumEditLayer.TERRAIN_MAP: newColor = TextureManager.terrain.GetColor(pos); break;
                case EnumEditLayer.TREES_MAP: newColor = TextureManager.trees.GetColor(pos); break;
                case EnumEditLayer.CITIES_MAP: newColor = TextureManager.cities.GetColor(pos); break;
                case EnumEditLayer.HEIGHT_MAP: newColor = TextureManager.height.GetColor(pos); break;
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

            return true;
        }
    }
}
