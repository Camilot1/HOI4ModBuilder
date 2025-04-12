using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Color3B
    {
        public readonly byte red, green, blue;
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
}
