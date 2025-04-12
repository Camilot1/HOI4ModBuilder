using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Color3F
    {
        public readonly float r, g, b;
        public Color3F(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}
