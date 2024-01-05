using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.openTK.text
{
    class Glyph
    {
        public char unicode;
        public float kerning;
        public float advance;
        public Bounds4F planeBounds;
        public Bounds4F atlasBounds;

        public Glyph(char unicode, float kerning, float advance, Bounds4F planeBounds, Bounds4F atlasBounds)
        {
            this.unicode = unicode;
            this.kerning = kerning;
            this.advance = advance;
            this.planeBounds = planeBounds;
            this.atlasBounds = atlasBounds;
        }

        public void WriteFloats(float[] data, Value2F cursor, Value2F pos, Value2F scale, int i, Color3B color)
        {
            float r = color.red / 255f;
            float g = color.green / 255f;
            float b = color.blue / 255f;

            //lu
            data[i] = pos.x + cursor.x + (kerning + planeBounds.left) * scale.x;
            data[i + 1] = pos.y + cursor.y + planeBounds.top * scale.y;
            data[i + 2] = r;
            data[i + 3] = g;
            data[i + 4] = b;
            data[i + 5] = atlasBounds.left;
            data[i + 6] = atlasBounds.top;

            //ru
            data[i + 7] = pos.x + cursor.x + (kerning + planeBounds.right) * scale.x;
            data[i + 8] = pos.y + cursor.y + planeBounds.top * scale.y;
            data[i + 9] = r;
            data[i + 10] = g;
            data[i + 11] = b;
            data[i + 12] = atlasBounds.right;
            data[i + 13] = atlasBounds.top;

            //rd
            data[i + 14] = pos.x + cursor.x + (kerning + planeBounds.right) * scale.x;
            data[i + 15] = pos.y + cursor.y + planeBounds.bottom;
            data[i + 16] = r;
            data[i + 17] = g;
            data[i + 18] = b;
            data[i + 19] = atlasBounds.right;
            data[i + 20] = atlasBounds.bottom;

            //ld
            data[i + 21] = pos.x + cursor.x + (kerning + planeBounds.left) * scale.x;
            data[i + 22] = pos.y + cursor.y + planeBounds.bottom;
            data[i + 23] = r;
            data[i + 24] = g;
            data[i + 25] = b;
            data[i + 26] = atlasBounds.left;
            data[i + 27] = atlasBounds.bottom;
        }

    }
}
