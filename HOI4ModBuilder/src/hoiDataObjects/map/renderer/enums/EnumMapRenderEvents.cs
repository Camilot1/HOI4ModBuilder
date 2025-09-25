
namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums
{
    public enum EnumMapRenderEvents
    {
        NONE = 0b0000_0000,
        PROVINCES_IDS = 0b0000_0001,
        STATES_IDS = 0b0000_0010,
        REGIONS_IDS = 0b0000_0100,
        BUILDINGS = 0b0000_1000,
        VICTORY_POINTS = 0b0001_0000,
        MANPOWER = 0b0010_0000,
        RESOURCES = 0b0100_0000
    }
}
