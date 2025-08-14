using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    public class MapToolVictoryPoints : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.VICTORY_POINTS;

        public MapToolVictoryPoints(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey { shift = true, key = Keys.V },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  new[] { EnumEditLayer.PROVINCES },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

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

            var history = province.State.History.GetValue();
            if (history == null)
                return false;

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

            uint prevCount = province.victoryPoints;
            int newCount = (int)prevCount + changeCount;
            if (newCount < 0)
                newCount = 0;

            Action<uint> action;

            action = (c) =>
            {
                if (province.State.SetVictoryPoints(province, c))
                {
                    MapManager.FontRenderController.AddEventData(EnumMapRenderEvents.VICTORY_POINTS, province);
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, parameter);
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

