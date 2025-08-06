using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
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

        public MapRendererResult Execute(ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (!BuildingManager.TryGetBuilding(parameter, out Building building))
            {
                MapManager.FontRenderController.TryStart(out var result)?
                    .ClearAll()
                    .End();

                if (!result)
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
                MapManager.FontRenderController.TryStart(out var result)?
                .SetScale(scaleProvince)
                .ClearAllMulti()
                .ForEachProvince(
                    (p) => p.GetBuildingCount(building) > 0,
                    (fontRegion, p, pos) => fontRegion.SetTextMulti(
                        p.Id, TextRenderManager.Instance.FontData64, scaleProvince,
                        p.GetBuildingCount(building) + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

                if (!result)
                    return MapRendererResult.ABORT;
            }
            else
            {
                MapManager.FontRenderController.TryStart(out var result)?
                .SetScale(scaleState)
                .ClearAllMulti()
                .ForEachState(
                    (s) => s.GetStateBuildingCount(building) > 0,
                    (fontRegion, s, pos) => fontRegion.SetTextMulti(
                        s.Id.GetValue(), TextRenderManager.Instance.FontData64, scaleState,
                        s.GetStateBuildingCount(building) + "", pos, QFontAlignment.Centre, color, true
                    ))
                .EndAssembleParallel();

                if (!result)
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
    }
}
