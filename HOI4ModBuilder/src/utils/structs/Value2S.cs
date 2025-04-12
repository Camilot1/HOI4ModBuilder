using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Value2S
    {
        public short x, y;
        public Value2S(short x, short y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Value2S uS &&
                   x == uS.x &&
                   y == uS.y;
        }

        public override int GetHashCode()
        {
            return x << 16 | y;
        }

        public override string ToString()
        {
            return $"Value2S(x = {x}; y = {y})";
        }
    }
}
