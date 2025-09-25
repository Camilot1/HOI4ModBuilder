using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;
using System.Collections;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolProvinceCoastal : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_COASTAL;

        public MapToolProvinceCoastal(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
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

            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);
            bool prevCoastal = province.IsCoastal;
            bool newCoastal = false, doAction = false;

            if (mouseEventArgs.Button == MouseButtons.Left && !prevCoastal)
            {
                newCoastal = true;
                doAction = true;
            }
            else if (mouseEventArgs.Button == MouseButtons.Right && prevCoastal)
            {
                newCoastal = false;
                doAction = true;
            }

            if (doAction)
            {
                Action<bool> action = (b) =>
                {
                    province.IsCoastal = b;
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, null);
                };

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newCoastal),
                    () => action(prevCoastal)
                );
            }

            return true;
        }
    }
}
