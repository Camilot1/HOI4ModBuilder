using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;

namespace HOI4ModBuilder.src.openTK.text
{
    public class TextRenderInvalidationQueue
    {
        private readonly TextRenderInvalidationBatch _pending = new TextRenderInvalidationBatch();

        public bool Enqueue(EnumMapRenderEvents eventFlag, Province province) => _pending.Add(eventFlag, province);

        public bool Enqueue(EnumMapRenderEvents eventFlag, State state) => _pending.Add(eventFlag, state);

        public bool Enqueue(EnumMapRenderEvents eventFlag, StrategicRegion region) => _pending.Add(eventFlag, region);

        public bool HasPendingEvents(TextLayerDependencies dependencies)
            => !_pending.IsEmpty && _pending.HasMatchingDependencies(dependencies);

        public TextRenderInvalidationBatch Drain()
        {
            if (_pending.IsEmpty)
                return null;

            var batch = new TextRenderInvalidationBatch(_pending);
            _pending.Clear();
            return batch;
        }

        public void Clear() => _pending.Clear();
    }
}
