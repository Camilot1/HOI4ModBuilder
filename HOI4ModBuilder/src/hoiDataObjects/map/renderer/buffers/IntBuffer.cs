namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers
{
    public sealed class IntBuffer
    {
        public int BufferId { get; private set; }
        public int[] Data { get; private set; }
        public int Length => Data?.Length ?? 0;
        public int LastAccessFrame { get; private set; }

        internal IntBuffer(int bufferId, int[] data, int currentFrame)
        {
            BufferId = bufferId;
            Data = data;
            LastAccessFrame = currentFrame;
        }

        internal void Replace(int[] data)
        {
            Data = data;
        }

        internal void Touch(int currentFrame)
        {
            LastAccessFrame = currentFrame;
        }
    }
}
