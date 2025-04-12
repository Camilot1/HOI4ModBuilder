using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Value2B
    {
        public byte x, y;

        public override bool Equals(object obj)
        {
            return obj is Value2B b &&
                   x == b.x &&
                   y == b.y;
        }

        public override int GetHashCode()
        {
            return x << 8 | y;
        }
    }
}
