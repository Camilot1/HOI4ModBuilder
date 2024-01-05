
namespace HOI4ModBuilder.utils
{
    public class Enums
    {
        public enum EnumBlendMode
        {
            ADD,
            MULTIPLY,
            OVERLAY
        }

        public enum EnumAnimationType
        {
            SCROLLING,
            ROTATING,
            PULSING
        }

        public enum EnumDirection
        {
            NONE,
            LEFT,
            UP,
            RIGHT,
            DOWN
        }

        public enum EnumFileScale
        {
            HEIGHT,
            ZOOM
        }

        public enum EnumMainLayer
        {
            PROVINCES_MAP,
            STATES,
            STRATEGIC_REGIONS,
            COUNTRIES,
            PROVINCE_TYPES,
            TERRAIN,
            CONTINENTS,
            MANPOWER,
            VICTORY_POINTS,
            STATE_CATEGORIES,
            BUILDINGS,
            TERRAIN_MAP,
            TREES_MAP,
            CITIES_MAP,
            HEIGHT_MAP,
            NORMAL_MAP,
            NONE
        }

        public enum EnumTool
        {
            CURSOR,
            RECTANGLE,
            ELLIPSE,
            MAGIC_WAND,
            BRUSH,
            FILL,
            ERASER,
            PIPETTE,
            PROVINCE_TYPE,
            PROVINCE_COASTAL,
            PROVINCE_TERRAIN,
            PROVINCE_CONTINENT,
            PROVINCE_STATE,
            PROVINCE_REGION,
            STATE_CATEGORY,
            BUILDINGS
        }

        public enum EnumEditLayer
        {
            PROVINCES,
            STATES,
            STRATEGIC_REGIONS,
            BUILDINGS,
            RIVERS,
            TERRAIN_MAP,
            TREES_MAP,
            CITIES_MAP,
            HEIGHT_MAP,
            NONE
        }

        public enum EnumMouseState
        {
            NONE,
            DOWN,
            MOVE,
            UP
        }

        public enum EnumBordersType
        {
            PROVINCES_BLACK,
            PROVINCES_WHITE,
            STATES_BLACK,
            STATES_WHITE,
            STRATEGIC_REGIONS_BLACK,
            STRATEGIC_REGIONS_WHITE
        }
    }
}
