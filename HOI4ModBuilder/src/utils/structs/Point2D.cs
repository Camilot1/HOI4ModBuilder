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

        public static Point2D operator *(Point2D a, Point2D b) => new Point2D { x = a.x * b.x, y = a.y * b.y };

        public bool InboundsPositiveBox(Value2I value2I, Point2D sizeFactor)
        {
            return InboundsPositiveBox(value2I.x, value2I.y, sizeFactor.x, sizeFactor.y);
        }

        public bool InboundsPositiveBox(double width, double height, double factorX, double factorY)
        {
            return x >= 0 && x <= width * factorX && y >= 0 && y <= height * factorY;
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
