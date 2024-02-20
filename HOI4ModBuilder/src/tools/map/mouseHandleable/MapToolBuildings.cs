using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolBuildings : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.BUILDINGS;

        public MapToolBuildings(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.BUILDINGS) return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);

            int changeCount = 0;

            if (!BuildingManager.TryGetBuilding(parameter, out Building building)) return;

            if (buttons == MouseButtons.Left && province != null && province.State != null)
                changeCount = 1;
            else if (buttons == MouseButtons.Right && province != null && province.State != null)
                changeCount = -1;

            if (changeCount != 0)
            {
                //TODO в state постройках currentCount обозначает общее количество всех построек, а не прошлое выбранной постройки
                // нужно исправить
                uint currentCount = 0;
                uint maxCount = 0;
                uint freeSlots = 0;
                int newCount = 0;
                uint prevCount = 0;

                if (building.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.PROVINCIAL)
                {
                    province.TryGetBuildingCount(building, out currentCount);
                    maxCount = building.maxLevel;
                    freeSlots = building.maxLevel - currentCount;
                    prevCount = currentCount;
                }
                else if (building.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.SHARED)
                {
                    foreach (var b in province.State.stateBuildings.Keys)
                    {
                        if (b.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.SHARED)
                            currentCount += province.State.stateBuildings[b];
                    }
                    maxCount = (uint)Math.Round(province.State.stateCategory.localBuildingsSlots * province.State.buildingsMaxLevelFactor);
                    freeSlots = maxCount - currentCount;
                    province.State.stateBuildings.TryGetValue(building, out prevCount);
                }
                else //NOT_SHARED 
                {
                    province.State.stateBuildings.TryGetValue(building, out currentCount);
                    maxCount = building.maxLevel;
                    freeSlots = building.maxLevel - currentCount;
                    prevCount = currentCount;
                }

                newCount = (int)prevCount + changeCount;

                if (newCount < 0) newCount = 0;
                else if (newCount > maxCount) newCount = (int)maxCount;

                if (prevCount == newCount) return;

                Action<uint> action;

                if (building.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.PROVINCIAL)
                {
                    action = (c) =>
                    {
                        province.State.SetProvinceBuilding(province, building, c);
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, parameter);
                    };
                }
                else
                {
                    action = (c) =>
                    {
                        province.State.SetStateBuilding(building, c);
                        MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, parameter);
                    };
                }

                MapManager.ActionsBatch.AddWithExecute(
                    () => action((uint)newCount),
                    () => action(prevCount)
                );
            }
            else return;
        }
    }
}
