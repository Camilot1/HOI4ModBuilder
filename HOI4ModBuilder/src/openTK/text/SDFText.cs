using HOI4ModBuilder.src.openTK.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.openTK
{
    class SDFText
    {
        private Value2F cursor;
        public Value2F position;
        private Value2F scale;
        public Font Font { get; }
        private ColoredChar[] chars;

        // 2 - pos, 3 - color, 2 - uv, 4 - vertexes
        public const int floatsPerChar = 7 * 4;

        public SDFText(string text, Color3B color, Value2F position, Value2F scale, Font font)
        {
            this.position = position;
            this.scale = scale;
            this.Font = font;
            SetText(text, color);
        }

        public SDFText(ColoredChar[] chars, Value2F position, Value2F scale, Font font)
        {
            this.position = position;
            this.scale = scale;
            this.Font = font;
            this.chars = chars;
        }

        public void SetText(string text, Color3B color)
        {
            ColoredChar[] chars = new ColoredChar[text.Length];
            char[] data = text.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = new ColoredChar(data[i], color);
            }

            this.chars = chars;
        }

        public int GetLength()
        {
            return chars.Length;
        }

        public float GetWidth()
        {
            foreach (ColoredChar ch in chars)
            {
                Glyph glyph = Font.GetGlyph(ch.ch);
                cursor.x += (glyph.kerning + glyph.advance) * scale.x;
            }

            float width = cursor.x;

            cursor.x = 0;
            cursor.y = 0;

            return width;
        }

        public void WriteFloats(float[] data, int startIndex, float posXDif)
        {
            int charCount = chars.Length;
            Value2F newPos = new Value2F(position);
            newPos.x += posXDif;

            for (int i = startIndex; i < startIndex + charCount * floatsPerChar; i += floatsPerChar)
            {
                ColoredChar ch = chars[i / floatsPerChar];
                Glyph glyph = Font.GetGlyph(ch.ch);
                glyph.WriteFloats(data, cursor, newPos, scale, i, ch.color);
                cursor.x += (glyph.kerning + glyph.advance) * scale.x;
            }

            cursor.x = 0;
            cursor.y = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is SDFText text &&
                   EqualityComparer<Value2F>.Default.Equals(cursor, text.cursor) &&
                   EqualityComparer<Value2F>.Default.Equals(position, text.position) &&
                   EqualityComparer<Value2F>.Default.Equals(scale, text.scale) &&
                   EqualityComparer<Font>.Default.Equals(Font, text.Font) &&
                   EqualityComparer<ColoredChar[]>.Default.Equals(chars, text.chars);
        }

        public override int GetHashCode()
        {
            int hashCode = -1916435100;
            hashCode = hashCode * -1521134295 + cursor.GetHashCode();
            hashCode = hashCode * -1521134295 + position.GetHashCode();
            hashCode = hashCode * -1521134295 + scale.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Font>.Default.GetHashCode(Font);
            hashCode = hashCode * -1521134295 + EqualityComparer<ColoredChar[]>.Default.GetHashCode(chars);
            return hashCode;
        }
    }

    public struct ColoredChar
    {
        public char ch;
        public Color3B color;

        public ColoredChar(char ch, Color3B color)
        {
            this.ch = ch;
            this.color = color;
        }

        public override bool Equals(object obj)
        {
            return obj is ColoredChar @char &&
                   ch == @char.ch &&
                   EqualityComparer<Color3B>.Default.Equals(color, @char.color);
        }

        public override int GetHashCode()
        {
            int hashCode = 2012466134;
            hashCode = hashCode * -1521134295 + ch.GetHashCode();
            hashCode = hashCode * -1521134295 + color.GetHashCode();
            return hashCode;
        }
    }
}
