using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Value2US
    {
        public ushort x, y;
        public Value2US(ushort x, ushort y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Value2US uS &&
                   x == uS.x &&
                   y == uS.y;
        }

        public override int GetHashCode()
        {
            return x << 16 | y;
        }
    }
}
