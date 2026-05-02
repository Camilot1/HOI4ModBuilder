using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers.structs
{
    public struct StateDataRecord
    {
        public const int Stride = 2;

        public int Color;
        public int StateCategoryId;

        public static StateDataRecord FromState(State state)
        {
            if (state == null)
                return default;

            return new StateDataRecord
            {
                Color = state.Color & 0x00FFFFFF,
                StateCategoryId = state.StateCategory.GetValue()?.id ?? 0,
            };
        }

        public int[] ToIntArray()
            => new int[]
            {
                Color,
                StateCategoryId,
            };

        public void WriteTo(int[] target, int startIndex)
        {
            target[startIndex] = Color;
            target[startIndex + 1] = StateCategoryId;
        }
    }
}
