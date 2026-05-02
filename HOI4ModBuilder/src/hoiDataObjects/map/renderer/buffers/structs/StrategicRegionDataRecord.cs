using HOI4ModBuilder.src.hoiDataObjects.map;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers.structs
{
    public struct StrategicRegionDataRecord
    {
        public const int Stride = 1;
        public int Color;

        public static StrategicRegionDataRecord FromStrategicRegion(StrategicRegion region)
        {
            if (region == null)
                return default;

            return new StrategicRegionDataRecord
            {
                Color = region.Color & 0x00FFFFFF,
            };
        }

        public int[] ToIntArray()
            => new int[]
            {
                Color,
            };

        public void WriteTo(int[] target, int startIndex)
        {
            target[startIndex] = Color;
        }
    }
}
