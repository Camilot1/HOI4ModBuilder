const int PROVINCE_TYPE_LAND_FLAG = 1;
const int PROVINCE_TYPE_SEA_FLAG = 2;
const int PROVINCE_TYPE_LAKE_FLAG = 4;
const int PROVINCE_TYPE_COASTAL_FLAG = 8;

struct ProvinceData
{
    int color;
    int type;
    int terrainId;
    int continentId;
    int stateId;
    int regionId;
    int victoryPoints;
    int pixelsCount;
};

