using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;

namespace HOI4ModBuilder.src.openTK.text
{
    public static class TextRenderEventsHandler
    {
        public static bool InvalidateProvince(EnumMapRenderEvents eventFlag, Province province)
            => MapManager.FontRenderController?.AddEventData(eventFlag, province) ?? false;

        public static bool InvalidateState(EnumMapRenderEvents eventFlag, State state)
            => MapManager.FontRenderController?.AddEventData(eventFlag, state) ?? false;

        public static bool InvalidateRegion(EnumMapRenderEvents eventFlag, StrategicRegion region)
            => MapManager.FontRenderController?.AddEventData(eventFlag, region) ?? false;
    }
}
