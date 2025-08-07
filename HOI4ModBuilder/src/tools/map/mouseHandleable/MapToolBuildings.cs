using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolBuildings : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.BUILDINGS;

        public MapToolBuildings(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { shift = true, key = Keys.B },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool),
                  new[] { EnumEditLayer.PROVINCES, EnumEditLayer.STATES, EnumEditLayer.BUILDINGS },
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

            int changeCount = 0;

            if (!BuildingManager.TryGetBuilding(parameter, out Building building))
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
                changeCount = 1;
            else if (mouseEventArgs.Button == MouseButtons.Right)
                changeCount = -1;

            if (MainForm.Instance.IsShiftPressed())
                changeCount *= 10;

            if (changeCount == 0)
                return false;

            //TODO в state постройках currentCount обозначает общее количество всех построек, а не прошлое выбранной постройки
            // нужно исправить
            uint currentCount = 0;
            uint maxCount = 0;
            uint freeSlots = 0;
            int newCount = 0;
            uint prevCount = 0;

            var buildingLevelCap = building.LevelCap.GetValue();
            var buildingSlotCategory = buildingLevelCap.GetSlotCategory();
            if (buildingSlotCategory == EnumBuildingSlotCategory.PROVINCIAL)
            {
                province.TryGetBuildingCount(building, out currentCount);
                maxCount = buildingLevelCap.GetProvinceMaxCount();
                freeSlots = maxCount - currentCount;
                prevCount = currentCount;
            }
            else if (buildingSlotCategory == EnumBuildingSlotCategory.SHARED)
            {
                foreach (var b in province.State.stateBuildings.Keys)
                {
                    if (b.LevelCap.GetValue().GetSlotCategory() == EnumBuildingSlotCategory.SHARED)
                        currentCount += province.State.stateBuildings[b];
                }

                maxCount = (uint)Math.Round(province.State.CurrentStateCategory.localBuildingsSlots * province.State.BuildingsMaxLevelFactor.GetValueRaw(1f));
                freeSlots = maxCount - currentCount;
                province.State.stateBuildings.TryGetValue(building, out prevCount);
            }
            else //NOT_SHARED 
            {
                province.State.stateBuildings.TryGetValue(building, out currentCount);
                maxCount = buildingLevelCap.GetStateMaxCount();
                freeSlots = maxCount - currentCount;
                prevCount = currentCount;
            }

            newCount = (int)prevCount + changeCount;

            if (newCount < 0)
                newCount = 0;
            else if (newCount > maxCount)
                newCount = (int)maxCount;

            if (prevCount == newCount)
                return false;

            Action<uint> action;

            if (buildingSlotCategory == EnumBuildingSlotCategory.PROVINCIAL)
            {
                action = (c) =>
                {
                    province.State.SetProvinceBuildingLevel(province, building, c);
                    MapManager.FontRenderController.AddEventData((int)EnumMapRenderEvents.BUILDINGS, province);
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, parameter);
                };
            }
            else
            {
                action = (c) =>
                {
                    province.State.SetStateBuildingLevel(building, c);
                    MapManager.FontRenderController.AddEventData((int)EnumMapRenderEvents.BUILDINGS, province.State);
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.enumMainLayer, parameter);
                };
            }

            MapManager.ActionsBatch.AddWithExecute(
                () => action((uint)newCount),
                () => action(prevCount)
            );

            return true;
        }
    }
}
