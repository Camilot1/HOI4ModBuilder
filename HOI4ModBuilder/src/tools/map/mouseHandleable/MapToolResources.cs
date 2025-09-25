using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.hoiDataObjects.common.resources;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    public class MapToolResources : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.RESOURCES;

        public MapToolResources(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      key = Keys.R,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override bool isHandlingMouseMove() => false;

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.PROVINCES, EnumEditLayer.STATES
        };
        public override Func<ICollection> GetParametersProvider() =>
            () => ResourceManager.GetResourcesTags();
        public override Func<ICollection> GetValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return false;

            if (province.State == null)
                return false;

            var state = province.State;

            int changeCount = 0;

            if (mouseEventArgs.Button == MouseButtons.Left)
                changeCount = 1;
            else if (mouseEventArgs.Button == MouseButtons.Right)
                changeCount = -1;

            if (MainForm.Instance.IsShiftPressed())
                changeCount *= 10;
            if (MainForm.Instance.IsControlPressed())
                changeCount *= 100;

            if (changeCount == 0)
                return false;

            uint prevCount = state.GetResourceCount(parameter);
            int newCount = (int)prevCount + changeCount;
            if (newCount < 0)
                newCount = 0;

            Action<uint> action;

            action = (c) =>
            {
                if (state.SetResourceCount(parameter, c))
                {
                    MapManager.FontRenderController.AddEventData(EnumMapRenderEvents.RESOURCES, state);
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, parameter);
                }
            };

            MapManager.ActionsBatch.AddWithExecute(
                () => action((uint)newCount),
                () => action(prevCount)
            );

            return true;
        }
    }
}
