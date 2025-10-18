using System;
using System.Drawing;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Color3B
    {
        private static readonly Random random = new Random();
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
            => ToArgb();

        public static bool operator ==(Color3B c1, Color3B c2)
            => c1.red == c2.red && c1.green == c2.green && c1.blue == c2.blue;

        public static bool operator !=(Color3B c1, Color3B c2)
            => c1.red != c2.red || c1.green != c2.green || c1.blue != c2.blue;

        public override string ToString()
            => $"({red}; {green}; {blue})";

        public Color ToColor()
            => Color.FromArgb(255, red, green, blue);
        public int ToArgb()
            => (int)((0xFF000000) | (red << 16) | (green << 8) | blue);

        public static Color3B GetRandowColor()
            => new Color3B((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
    }
}
