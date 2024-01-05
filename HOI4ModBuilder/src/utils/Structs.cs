using System;
using System.Collections.Generic;
using System.Drawing;

namespace HOI4ModBuilder.utils
{
    public class Structs
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

        public struct Value3F
        {
            public float x, y, z;

            public Value3F(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public override bool Equals(object obj)
            {
                return obj is Value3F f &&
                       x == f.x &&
                       y == f.y &&
                       z == f.z;
            }

            public override int GetHashCode()
            {
                int hashCode = 1502939027;
                hashCode = hashCode * -1521134295 + x.GetHashCode();
                hashCode = hashCode * -1521134295 + y.GetHashCode();
                hashCode = hashCode * -1521134295 + z.GetHashCode();
                return hashCode;
            }
        }

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

        public struct Value2I
        {
            public int x, y;

            public override bool Equals(object obj)
            {
                return obj is Value2I i &&
                       x == i.x &&
                       y == i.y;
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

        public struct Color3B
        {
            public byte red, green, blue;
            public Color3B(byte red, byte green, byte blue)
            {
                this.red = red;
                this.green = green;
                this.blue = blue;
            }

            public Color3B(int color)
            {
                red = (byte)(color >> 16);
                green = (byte)(color >> 8);
                blue = (byte)color;
            }

            public Color3B(Color color)
            {
                red = color.R;
                green = color.G;
                blue = color.B;
            }

            public override bool Equals(object obj)
            {
                return obj is Color3B b &&
                       red == b.red &&
                       green == b.green &&
                       blue == b.blue;
            }

            public void SetColor(int argb)
            {
                red = (byte)(argb >> 16);
                green = (byte)(argb >> 8);
                blue = (byte)argb;
            }

            public override int GetHashCode()
            {
                return (int)((0xFF000000) | (red << 16) | (green << 8) | blue);
            }

            public static bool operator ==(Color3B c1, Color3B c2)
            {
                return c1.red == c2.red && c1.green == c2.green && c1.blue == c2.blue;
            }

            public static bool operator !=(Color3B c1, Color3B c2)
            {
                return !(c1.red == c2.red && c1.green == c2.green && c1.blue == c2.blue);
            }

            public override string ToString()
            {
                return $"({red}; {green}; {blue})";
            }

            public Color ToColor()
            {
                return Color.FromArgb(255, red, green, blue);
            }

            public static Color3B GetRandowColor()
            {
                var random = new Random();
                return new Color3B((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            }
        }

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

        public struct Bounds4D
        {
            public double left;
            public double top;
            public double right;
            public double bottom;

            public Bounds4D(double left, double top, double right, double bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public bool Inbounds(Point2D pos)
            {
                return left <= pos.x && pos.x <= right && top <= pos.y && pos.y <= bottom;
            }

            public bool HasSpace()
            {
                return left != right && top != bottom;
            }

            public void FixDimensions()
            {
                if (left > right) right = left;
                if (top > bottom) bottom = top;
            }
        }

        public struct Bounds4F
        {
            public float left;
            public float top;
            public float right;
            public float bottom;

            public Bounds4F(float left, float top, float right, float bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public Bounds4F(Bounds4D bounds)
            {
                left = (float)bounds.left;
                top = (float)bounds.top;
                right = (float)bounds.right;
                bottom = (float)bounds.bottom;
            }

            public bool Inbounds(Point2D pos)
            {
                return left <= pos.x && pos.x <= right && top <= pos.y && pos.y <= bottom;
            }

            public bool HasSpace()
            {
                return left != right && top != bottom;
            }

            public void FixDimensions()
            {
                if (left > right) right = left;
                if (top > bottom) bottom = top;
            }
        }

        public struct Bounds4US
        {
            public ushort left;
            public ushort top;
            public ushort right;
            public ushort bottom;

            public Bounds4US(ushort left, ushort top, ushort right, ushort bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public Bounds4US(Bounds4US bounds)
            {
                left = bounds.left;
                top = bounds.top;
                right = bounds.right;
                bottom = bounds.bottom;
            }

            public bool Inbounds(Point2D pos)
            {
                return left <= pos.x && pos.x <= right && top <= pos.y && pos.y <= bottom;
            }

            public bool HasSpace()
            {
                return left != right && top != bottom;
            }

            public void FixDimensions()
            {
                if (left > right) right = left;
                if (top > bottom) bottom = top;
            }

            public HashSet<Value2US> ToPositions(ushort maxWidth, ushort maxHeight)
            {
                FixDimensions();
                var poses = new HashSet<Value2US>((right - left) * (bottom - top));

                ushort maxX = right > maxWidth ? maxWidth : right;
                ushort maxY = bottom > maxHeight ? maxHeight : bottom;

                for (ushort x = left; x < maxX; x++)
                {
                    for (ushort y = top; y < maxY; y++)
                    {
                        poses.Add(new Value2US(x, y));
                    }
                }
                return poses;
            }

            internal void Set(ushort left, ushort top, ushort right, ushort bottom)
            {
                this.left = left;
                this.top = top;
                this.left = left;
                this.bottom = bottom;
            }
        }

        public struct ViewportInfo
        {
            public int x, y, width, height, max;
        }

    }
}
