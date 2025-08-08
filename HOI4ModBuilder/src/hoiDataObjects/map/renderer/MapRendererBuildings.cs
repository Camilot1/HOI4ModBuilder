using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.openTK;
using QuickFont;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class MapRendererBuildings : IMapRenderer
    {
        private static readonly float scaleProvince = 0.04f;
        private static readonly float scaleState = 0.125f;
        private static readonly Color color = Color.Yellow;

        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (!BuildingManager.TryGetBuilding(parameter, out Building building))
            {
                if (recalculateAllText)
                    if (!TextRenderRecalculate())
                        return MapRendererResult.ABORT;

                func = (p) =>
                {
                    var type = p.Type;
                    if (type == EnumProvinceType.SEA)
                        return Utils.ArgbToInt(255, 0, 0, 127);
                    else if (type == EnumProvinceType.LAKE)
                        return Utils.ArgbToInt(255, 127, 255, 255);
                    else
                        return Utils.ArgbToInt(255, 0, 0, 0);
                };
                return MapRendererResult.CONTINUE;
            }

            var buildingLevelCap = building.LevelCap.GetValue();
            var buildingSlotCategory = buildingLevelCap.GetSlotCategory();

            if (buildingSlotCategory == EnumBuildingSlotCategory.PROVINCIAL)
            {
                if (recalculateAllText)
                    if (!TextRenderRecalculateProvinces(building))
                        return MapRendererResult.ABORT;
            }
            else
            {
                if (recalculateAllText)
                    if (!TextRenderRecalculateStates(building))
                        return MapRendererResult.ABORT;
            }

            if (buildingSlotCategory == EnumBuildingSlotCategory.PROVINCIAL)
            {
                float maxLevel = buildingLevelCap.GetProvinceMaxCount();

                func = (p) =>
                {
                    var type = p.Type;
                    if (type == EnumProvinceType.SEA)
                        return Utils.ArgbToInt(255, 0, 0, 127);
                    else if (type == EnumProvinceType.LAKE)
                        return Utils.ArgbToInt(255, 127, 255, 255);

                    if (!p.TryGetBuildingCount(building, out uint count))
                        return Utils.ArgbToInt(255, 0, 0, 0);

                    float factor = count / maxLevel;
                    if (factor > 1)
                        return Utils.ArgbToInt(255, 255, 0, 0);
                    else if (factor == 1)
                        return Utils.ArgbToInt(255, 0, 196, 255);
                    else
                    {
                        byte value = (byte)(255 * factor);
                        return Utils.ArgbToInt(255, 0, value, 0);
                    }
                };
            }
            else if (buildingSlotCategory == EnumBuildingSlotCategory.SHARED)
            {
                uint maxCount = 0;
                foreach (State state in StateManager.GetStates())
                {
                    if (state.stateBuildings.TryGetValue(building, out uint max))
                        if (max > maxCount) maxCount = max;
                }

                func = (p) =>
                {
                    var type = p.Type;
                    if (type == EnumProvinceType.SEA)
                        return Utils.ArgbToInt(255, 0, 0, 127);
                    else if (type == EnumProvinceType.LAKE)
                        return Utils.ArgbToInt(255, 127, 255, 255);
                    else if (p.State == null)
                        return Utils.ArgbToInt(255, 0, 0, 0);
                    else
                    {
                        p.State.stateBuildings.TryGetValue(building, out uint count);

                        float factor = count / (float)maxCount;
                        if (factor > 1)
                            return Utils.ArgbToInt(255, 255, 0, 0);
                        else if (factor == 1)
                            return Utils.ArgbToInt(255, 0, 196, 255);
                        else
                        {
                            byte value = (byte)(255 * factor);
                            return Utils.ArgbToInt(255, 0, value, 0);
                        }
                    }
                };
            }
            else //NON_SHARED
            {
                float maxCount = buildingLevelCap.GetStateMaxCount();
                func = (p) =>
                {
                    var type = p.Type;
                    if (type == EnumProvinceType.SEA)
                        return Utils.ArgbToInt(255, 0, 0, 127);
                    else if (type == EnumProvinceType.LAKE)
                        return Utils.ArgbToInt(255, 127, 255, 255);
                    else if (p.State == null)
                        return Utils.ArgbToInt(255, 0, 0, 0);
                    else
                    {
                        p.State.stateBuildings.TryGetValue(building, out uint count);

                        float factor = count / maxCount;
                        if (factor > 1)
                            return Utils.ArgbToInt(255, 255, 0, 0);
                        else if (factor == 1)
                            return Utils.ArgbToInt(255, 0, 196, 255);
                        else
                        {
                            byte value = (byte)(255 * factor);
                            return Utils.ArgbToInt(255, 0, value, 0);
                        }
                    }
                };
            }

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .SetEventsHandler((int)EnumMapRenderEvents.NONE, (flags, obj) => { })
                .ClearAll()
                .End();

            return result;
        }

        public bool TextRenderRecalculateProvinces(Building building)
        {
            var scale = scaleProvince;
            var controller = MapManager.FontRenderController;
            controller.TryStart(out var result)?
                .SetEventsHandler((int)EnumMapRenderEvents.BUILDINGS, (flags, objs) =>
                {
                    controller.TryStart(controller.EventsFlags, out var eventResult)?
                    .ForEachProvince(objs, p => true, (fontRegion, p, pos) =>
                    {
                        var count = p.GetBuildingCount(building);
                        if (count == 0)
                            controller.PushAction(pos, r => r.RemoveTextMulti(p.Id));
                        else
                            controller.PushAction(pos, r => r.SetTextMulti(
                                p.Id, TextRenderManager.Instance.FontData64, scale,
                                count + "", pos, QFontAlignment.Centre, color, true
                            ));
                    })
                    .EndAssembleParallelWithWait();
                })
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachProvince(
                    (p) => p.GetBuildingCount(building) > 0,
                    (fontRegion, p, pos) => fontRegion.SetTextMulti(
                        p.Id, TextRenderManager.Instance.FontData64, scale,
                        p.GetBuildingCount(building) + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }

        public bool TextRenderRecalculateStates(Building building)
        {
            var scale = scaleState;
            var controller = MapManager.FontRenderController;
            controller.TryStart(out var result)?
                .SetEventsHandler((int)EnumMapRenderEvents.BUILDINGS, (flags, objs) =>
                {
                    controller.TryStart(controller.EventsFlags, out var eventResult)?
                    .ForEachState(objs, s => true, (fontRegion, s, pos) =>
                    {
                        var count = s.GetStateBuildingCount(building);
                        if (count == 0)
                            controller.PushAction(pos, r => r.RemoveTextMulti(s.Id.GetValue()));
                        else
                            controller.PushAction(pos, r => r.SetTextMulti(
                                s.Id.GetValue(), TextRenderManager.Instance.FontData64, scale,
                                count + "", pos, QFontAlignment.Centre, color, true
                            ));
                    })
                    .EndAssembleParallelWithWait();
                })
                .SetScale(scale)
                .ClearAllMulti()
                .ForEachState(
                    (s) => s.GetStateBuildingCount(building) > 0,
                    (fontRegion, s, pos) => fontRegion.SetTextMulti(
                        s.Id.GetValue(), TextRenderManager.Instance.FontData64, scale,
                        s.GetStateBuildingCount(building) + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

            return result;
        }
    }
}
