using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.openTK.text;
using System.Drawing;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    public static class MapDomainEventsHandler
    {
        public static void OnProvinceChanged(Province province)
        {
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceCreate(Province province)
        {
            MapRendererEventsHandler.OnProvinceCreate(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceRemove(Province province)
        {
            MapRendererEventsHandler.OnProvinceRemove(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceIdChanged(Province province, ushort fromId, ushort toId)
        {
            MapRendererEventsHandler.OnProvinceIDChange(province, fromId, toId);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceTerrainChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceContinentChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceStateChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceStateIdChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceRegionChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceRegionIdChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvincePixelsCountChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnProvinceTypeFlagsChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.PROVINCES, province);
        }

        public static void OnStateColorChanged(State state)
        {
            MapRendererEventsHandler.UpdateStateDataRecord(state);
        }

        public static void OnStateChanged(State state)
        {
            MapRendererEventsHandler.UpdateStateDataRecord(state);
            TextRenderEventsHandler.InvalidateState(EnumMapRenderEvents.STATES, state);
        }

        public static void OnStateCategoryChanged(State state)
        {
            MapRendererEventsHandler.UpdateStateDataRecord(state);
            TextRenderEventsHandler.InvalidateState(EnumMapRenderEvents.STATES, state);
        }

        public static void OnRegionColorChanged(StrategicRegion region)
        {
            MapRendererEventsHandler.UpdateRegionDataRecord(region);
        }
        public static void OnRegionChanged(StrategicRegion region)
        {
            MapRendererEventsHandler.UpdateRegionDataRecord(region);
            TextRenderEventsHandler.InvalidateRegion(EnumMapRenderEvents.REGIONS, region);
        }

        public static void OnProvinceVictoryPointsChanged(Province province)
        {
            MapRendererEventsHandler.UpdateProvinceDataRecord(province);
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.VICTORY_POINTS, province);
        }

        public static void OnBuildingsChanged(Province province)
        {
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.BUILDINGS, province);
        }

        public static void OnBuildingsChanged(State state)
        {
            TextRenderEventsHandler.InvalidateState(EnumMapRenderEvents.BUILDINGS, state);
        }

        public static void OnResourcesChanged(State state)
        {
            TextRenderEventsHandler.InvalidateState(EnumMapRenderEvents.RESOURCES, state);
        }

        public static void OnStrategicLocationsChanged(Province province)
        {
            TextRenderEventsHandler.InvalidateProvince(EnumMapRenderEvents.STRATEGIC_LOCATIONS, province);
        }
    }
}
