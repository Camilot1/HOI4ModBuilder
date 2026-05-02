using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using System;

namespace HOI4ModBuilder.src.openTK.text
{
    public enum TextRenderEntityKind : byte
    {
        Province = 1,
        State = 2,
        Region = 3
    }

    public struct TextRenderKey : IEquatable<TextRenderKey>
    {
        public readonly TextRenderEntityKind Kind;
        public readonly int Id;

        public TextRenderKey(TextRenderEntityKind kind, int id)
        {
            Kind = kind;
            Id = id;
        }

        public static TextRenderKey ForProvince(Province province)
            => new TextRenderKey(TextRenderEntityKind.Province, province?.Id ?? 0);

        public static TextRenderKey ForState(State state)
            => new TextRenderKey(TextRenderEntityKind.State, state == null ? 0 : state.Id.GetValue());

        public static TextRenderKey ForRegion(StrategicRegion region)
            => new TextRenderKey(TextRenderEntityKind.Region, region?.Id ?? 0);

        public bool Equals(TextRenderKey other) => Kind == other.Kind && Id == other.Id;

        public override bool Equals(object obj) => obj is TextRenderKey other && Equals(other);

        public override int GetHashCode() => ((int)Kind * 397) ^ Id;

        public override string ToString() => $"{Kind}:{Id}";
    }
}
