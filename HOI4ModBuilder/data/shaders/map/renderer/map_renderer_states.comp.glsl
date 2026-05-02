#version 430 core

#include "includes/map_renderer_province_data.comp.glsl"
#include "includes/map_renderer_state_data.comp.glsl"
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

layout(std430, binding = 2) readonly buffer StateDataByIdBuffer
{
    StateData stateDataById[];
};

void main()
{
    uvec2 coord = gl_GlobalInvocationID.xy;
    if (coord.x >= uint(mapWidth) || coord.y >= uint(mapHeight))
        return;

    uint pixelIndex = coord.y * uint(mapWidth) + coord.x;
    uint provinceId = pixelsToProvinceIds[pixelIndex];

    uint stateId = 0u;
    if (provinceId < provinceDataById.length())
        stateId = uint(max(provinceDataById[provinceId].stateId, 0));

    uint packedColor = 0u;
    if (stateId < stateDataById.length())
        packedColor = uint(stateDataById[stateId].color);

    imageStore(outputImage, ivec2(coord), unpackColor(packedColor));
}
