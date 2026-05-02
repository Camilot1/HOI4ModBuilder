using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers.structs
{
    public struct StateCategoryDataRecord
    {
        public const int Stride = 1;

        public int Color;

        public static StateCategoryDataRecord FromStateCategory(StateCategory stateCategory)
        {
            int color = stateCategory == null
                ? Utils.ArgbToInt(255, 255, 0, 0)
                : stateCategory.ColorInt;

            return new StateCategoryDataRecord
            {
                Color = color & 0x00FFFFFF,
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
