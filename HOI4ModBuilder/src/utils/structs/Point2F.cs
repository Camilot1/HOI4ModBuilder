using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Point2F
    {
        public float x, y;

        public Point2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Scale(float factor)
        {
            x *= factor;
            y *= factor;
        }

        public bool InboundsPositiveBox(float width, float height)
        {
            return x >= 0 && x <= width && y >= 0 && y <= height;
        }

        public bool Inbounds(float minX, float minY, float maxX, float maxY)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }

        public bool IsOnLine(Point2F f, Point2F s, float maxDistance)
        {
            // Рассчитываем расстояние от точки до прямой
            double dist = Math.Abs((y - f.y) * (s.x - f.x) - (x - f.x) * (s.y - f.y)) /
                Math.Sqrt(Math.Pow(s.x - f.x, 2) + Math.Pow(s.y - f.y, 2));

            // Проверяем, лежит ли точка на прямой или находится на расстоянии, меньшем допустимого
            if (dist <= maxDistance)
            {
                // Проверяем, лежит ли точка между точками (x0, y0) и (x1, y1)
                if ((f.x <= x && x <= s.x || s.x <= x && x <= f.x) && (f.x <= y && y <= s.y || s.y <= y && y <= f.y))
                {
                    return true;
                }
            }

            return false;
        }

        public double GetDistanceTo(Point2F point)
        {
            return Math.Sqrt((x - point.x) * (x - point.x) + (y - point.y) * (y - point.y));
        }

        public double GetDistanceTo(Point2D point)
        {
            return Math.Sqrt((x - point.x) * (x - point.x) + (y - point.y) * (y - point.y));
        }
    }
}
