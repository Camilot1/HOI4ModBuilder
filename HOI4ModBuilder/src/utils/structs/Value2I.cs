using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Value2I
    {
        public int x, y;

        public override bool Equals(object obj)
        {
            return obj is Value2I i &&
                   x == i.x &&
                   y == i.y;
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public string ToPositionString()
        {
            return $"{x}; {y}";
        }
    }
}
