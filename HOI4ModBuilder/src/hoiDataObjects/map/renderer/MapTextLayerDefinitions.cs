using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.openTK.text;
using QuickFont;
using System.Drawing;
using HOI4ModBuilder.src.openTK.text.layerDefinitions;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public static class MapTextLayerDefinitions
    {
        private static readonly Color DefaultColor = Color.Yellow;

        public static readonly NoTextLayerDefinition<TextLayerContext> None
            = new NoTextLayerDefinition<TextLayerContext>();

        public static readonly ProvinceTextLayerDefinition<TextLayerContext> VictoryPoints
            = new ProvinceTextLayerDefinition<TextLayerContext>(
                0.04f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromProvince(
                    EnumMapRenderEvents.VICTORY_POINTS,
                    EnumMapRenderEvents.PROVINCES
                ),
                (province, context) => province.victoryPoints > 0,
                (province, context) => province.victoryPoints == 0 ? null : province.victoryPoints + ""
            );

        public static readonly ProvinceTextLayerDefinition<TextLayerContext> ProvinceIds
            = new ProvinceTextLayerDefinition<TextLayerContext>(
                0.03f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromProvince(EnumMapRenderEvents.PROVINCES),
                (province, context) => true,
                (province, context) => province.Id + ""
            );

        public static readonly StateTextLayerDefinition<TextLayerContext> StateIds
            = new StateTextLayerDefinition<TextLayerContext>(
                0.125f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromState(EnumMapRenderEvents.STATES),
                (state, context) => true,
                (state, context) => state.Id.GetValue() + ""
            );

        public static readonly RegionTextLayerDefinition<TextLayerContext> RegionIds
            = new RegionTextLayerDefinition<TextLayerContext>(
                0.15f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromRegion(EnumMapRenderEvents.REGIONS),
                (region, context) => true,
                (region, context) => region.Id + ""
            );

        public static readonly StateTextLayerDefinition<TextLayerContext> Resources
            = new StateTextLayerDefinition<TextLayerContext>(
                0.125f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromState(
                    EnumMapRenderEvents.RESOURCES,
                    EnumMapRenderEvents.STATES
                ),
                (state, context) => state.GetResourceCount(context.Parameter) > 0,
                (state, context) =>
                {
                    Resource resource = ResourceManager.Get(context.Parameter);
                    uint resourceCount = state.GetResourceCount(resource);
                    return resourceCount == 0 ? null : resourceCount + "";
                }
            );

        public static readonly StateTextLayerDefinition<TextLayerContext> Manpower
            = new StateTextLayerDefinition<TextLayerContext>(
                0.125f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromState(
                    EnumMapRenderEvents.MANPOWER,
                    EnumMapRenderEvents.STATES
                ),
                (state, context) => true,
                (state, context) => state.Manpower.GetValue() + ""
            );

        public static readonly ProvinceTextLayerDefinition<TextLayerContext> StrategicLocations
            = new ProvinceTextLayerDefinition<TextLayerContext>(
                0.075f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.Combine(
                    TextLayerDependencies.FromProvince(
                        EnumMapRenderEvents.STRATEGIC_LOCATIONS,
                        EnumMapRenderEvents.PROVINCES
                    ),
                    TextLayerDependencies.FromState(EnumMapRenderEvents.STATES)
                ),
                (province, context) => province.State != null &&
                    (province.State.provinceStrategicLocations.ContainsKey(province) ||
                     province.State.stateStrategicLocations.ContainsKey(province)),
                (province, context) => AssembleStrategicLocationText(province, context.ParameterValue)
            );

        public static readonly ProvinceTextLayerDefinition<TextLayerContext> BuildingProvinces
            = new ProvinceTextLayerDefinition<TextLayerContext>(
                0.04f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.FromProvince(EnumMapRenderEvents.BUILDINGS),
                (province, context) =>
                {
                    if (!BuildingManager.TryGetBuilding(context.Parameter, out var building))
                        return false;

                    return province.GetBuildingCount(building) > 0;
                },
                (province, context) =>
                {
                    if (!BuildingManager.TryGetBuilding(context.Parameter, out var building))
                        return null;

                    uint count = province.GetBuildingCount(building);
                    return count == 0 ? null : count + "";
                }
            );

        public static readonly StateTextLayerDefinition<TextLayerContext> BuildingStates
            = new StateTextLayerDefinition<TextLayerContext>(
                0.125f,
                DefaultColor,
                QFontAlignment.Centre,
                TextLayerDependencies.Combine(
                    TextLayerDependencies.FromState(
                        EnumMapRenderEvents.BUILDINGS,
                        EnumMapRenderEvents.STATES
                    ),
                    TextLayerDependencies.FromProvince(EnumMapRenderEvents.PROVINCES)
                ),
                (state, context) =>
                {
                    if (!BuildingManager.TryGetBuilding(context.Parameter, out var building))
                        return false;

                    return state.GetStateBuildingCount(building) > 0;
                },
                (state, context) =>
                {
                    if (!BuildingManager.TryGetBuilding(context.Parameter, out var building))
                        return null;

                    uint count = state.GetStateBuildingCount(building);
                    return count == 0 ? null : count + "";
                }
            );

        private static string AssembleStrategicLocationText(Province province, string parameterValue)
        {
            if (province.State == null)
                return null;

            province.State.provinceStrategicLocations.TryGetValue(province, out var provinceLocations);
            province.State.stateStrategicLocations.TryGetValue(province, out var stateLocations);

            if (provinceLocations == null && stateLocations == null)
                return null;

            string firstName = null;
            int count = 0;

            if (parameterValue == "")
            {
                if (provinceLocations != null && provinceLocations.Count > 0)
                {
                    firstName = provinceLocations[0].Name;
                    count += provinceLocations.Count;
                }
                if (stateLocations != null && stateLocations.Count > 0)
                {
                    firstName = stateLocations[0].Name;
                    count += stateLocations.Count;
                }
            }
            else
            {
                if (provinceLocations != null)
                {
                    foreach (var strategicLocation in provinceLocations)
                    {
                        if (strategicLocation.Name == parameterValue)
                        {
                            firstName = parameterValue;
                            count += 1;
                            break;
                        }
                    }
                }
                if (stateLocations != null)
                {
                    foreach (var strategicLocation in stateLocations)
                    {
                        if (strategicLocation.Name == parameterValue)
                        {
                            firstName = parameterValue;
                            count += 1;
                            break;
                        }
                    }
                }
            }

            if (count == 1)
                return firstName;
            if (count > 1)
                return firstName + " [+" + (count - 1) + "]";
            return null;
        }
    }
}
