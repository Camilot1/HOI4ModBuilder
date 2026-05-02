#version 430 core

#include "includes/map_renderer_province_data.comp.glsl"
#include "includes/map_renderer_color_utils.comp.glsl"

layout(local_size_x = 16, local_size_y = 16) in;

layout(rgba8, binding = 0) uniform writeonly image2D outputImage;

uniform int mapWidth;
uniform int mapHeight;

layout(std430, binding = 0) readonly buffer PixelsToProvinceIdsBuffer
{
    uint pixelsToProvinceIds[];
};

layout(std430, binding = 1) readonly buffer ProvinceDataByIdBuffer
{
    ProvinceData provinceDataById[];
};

uint getPackedColor(int provinceTypeFlags)
{
    bool isLand = (provinceTypeFlags & PROVINCE_TYPE_LAND_FLAG) != 0;
    bool isSea = (provinceTypeFlags & PROVINCE_TYPE_SEA_FLAG) != 0;
    bool isLake = (provinceTypeFlags & PROVINCE_TYPE_LAKE_FLAG) != 0;
    bool isCoastal = (provinceTypeFlags & PROVINCE_TYPE_COASTAL_FLAG) != 0;

    if (isLand)
        return isCoastal ? 0x007F7F00u : 0x00007F00u;
    if (isSea)
        return isCoastal ? 0x007F007Fu : 0x0000007Fu;
    if (isLake)
        return 0x007FFFFFu;
    return 0u;
}

void main()
{
    uvec2 coord = gl_GlobalInvocationID.xy;
    if (coord.x >= uint(mapWidth) || coord.y >= uint(mapHeight))
        return;

    uint pixelIndex = coord.y * uint(mapWidth) + coord.x;
    uint provinceId = pixelsToProvinceIds[pixelIndex];

    uint packedColor = 0u;
    if (provinceId < provinceDataById.length())
        packedColor = getPackedColor(provinceDataById[provinceId].type);

    imageStore(outputImage, ivec2(coord), unpackColor(packedColor));
}
