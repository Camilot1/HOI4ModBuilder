using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public class ViewportInfo
    {
        public int x, y, width, height, max;

        public override string ToString()
        {
            return $"ViewportInfo(x={x}; y={y}; width={width}; height={height}; max={max}";
        }
    }
}
