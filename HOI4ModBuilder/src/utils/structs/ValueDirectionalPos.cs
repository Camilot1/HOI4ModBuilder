using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct ValueDirectionalPos
    {
        public Value2S pos;
        public byte flags;
        public bool isUsed;

        public override bool Equals(object obj)
        {
            return obj is ValueDirectionalPos pos &&
                   EqualityComparer<Value2S>.Default.Equals(this.pos, pos.pos);
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode();
        }

        public override string ToString()
        {
            return $"ValueDirectionalPos({pos}; flags={flags}; isUsed={isUsed}";
        }
    }
}
