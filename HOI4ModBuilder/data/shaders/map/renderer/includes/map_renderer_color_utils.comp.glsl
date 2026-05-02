vec4 unpackColor(uint packedColor)
{
    float r = float((packedColor >> 16u) & 255u) / 255.0;
    float g = float((packedColor >> 8u) & 255u) / 255.0;
    float b = float(packedColor & 255u) / 255.0;
    return vec4(r, g, b, 1.0);
}
