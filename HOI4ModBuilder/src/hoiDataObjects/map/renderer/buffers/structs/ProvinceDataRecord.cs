using HOI4ModBuilder.hoiDataObjects.map;
using System.Runtime.InteropServices;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ProvinceDataRecord
    {
        public const int ProvinceTypeLandFlag = 1;
        public const int ProvinceTypeSeaFlag = 2;
        public const int ProvinceTypeLakeFlag = 4;
        public const int ProvinceTypeCoastalFlag = 8;

        public const int Stride = 8;
        public int Color;
        public int Type;
        public int TerrainId;
        public int ContinentId;
        public int StateId;
        public int RegionId;
        public int VictoryPoints;
        public int PixelsCount;

        public static ProvinceDataRecord FromProvince(Province province)
        {
            if (province == null)
                return default;

            return new ProvinceDataRecord
            {
                Color = province.Color,
                Type = GetProvinceTypeFlags(province),
                TerrainId = province.Terrain?.id ?? 0,
                ContinentId = province.ContinentId,
                StateId = province.State?.Id.GetValue() ?? 0,
                RegionId = province.Region?.Id ?? 0,
                VictoryPoints = (int)province.victoryPoints,
                PixelsCount = province.pixelsCount,
            };
        }

        public int[] ToIntArray()
            => new int[]
            {
                Color,
                Type,
                TerrainId,
                ContinentId,
                StateId,
                RegionId,
                VictoryPoints,
                PixelsCount,
            };

        public void WriteTo(int[] target, int startIndex)
        {
            target[startIndex] = Color;
            target[startIndex + 1] = Type;
            target[startIndex + 2] = TerrainId;
            target[startIndex + 3] = ContinentId;
            target[startIndex + 4] = StateId;
            target[startIndex + 5] = RegionId;
            target[startIndex + 6] = VictoryPoints;
            target[startIndex + 7] = PixelsCount;
        }

        private static int GetProvinceTypeFlags(Province province)
        {
            int flags = 0;

            switch (province.Type)
            {
                case EnumProvinceType.LAND:
                    flags |= ProvinceTypeLandFlag;
                    break;
                case EnumProvinceType.SEA:
                    flags |= ProvinceTypeSeaFlag;
                    break;
                case EnumProvinceType.LAKE:
                    flags |= ProvinceTypeLakeFlag;
                    break;
            }

            if (province.IsCoastal)
                flags |= ProvinceTypeCoastalFlag;

            return flags;
        }
    }
}
