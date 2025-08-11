using HOI4ModBuilder.hoiDataObjects.map;

namespace HOI4ModBuilder.src.hoiDataObjects.map.positions
{
    public class BuildingPosition
    {
        public State state;
        public float x, y, z;
        public float rotation;
        public Province adjacentProvince;

        public BuildingInfo buildingInfo;
        public Province province;

    }
}
