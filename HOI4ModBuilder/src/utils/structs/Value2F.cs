using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Value2F
    {
        public float x, y;

        public Value2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Value2F(Value2F value2F)
        {
            x = value2F.x;
            y = value2F.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Value2F f &&
                   x == f.x &&
                   y == f.y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }
}
