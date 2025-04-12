using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Point2D
    {
        public double x, y;

        public void Scale(double factor)
        {
            x *= factor;
            y *= factor;
        }

        public bool InboundsPositiveBox(Value2I value2I)
        {
            return InboundsPositiveBox(value2I.x, value2I.y);
        }

        public bool InboundsPositiveBox(double width, double height)
        {
            return x >= 0 && x <= width && y >= 0 && y <= height;
        }

        public bool Inbounds(double minX, double minY, double maxX, double maxY)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }

        public bool IsOnLine(Point2F f, Point2F s, float maxDistance)
        {
            double rd = Math.Sqrt(Math.Pow(f.x - s.x, 2) + Math.Pow(f.y - s.y, 2));
            double d0 = Math.Sqrt(Math.Pow(x - s.x, 2) + Math.Pow(y - s.y, 2));
            double d1 = Math.Sqrt(Math.Pow(f.x - x, 2) + Math.Pow(f.y - y, 2));

            return d0 + d1 <= rd * maxDistance;
        }
    }
}
