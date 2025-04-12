using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.openTK.text
{
    public class FontData
    {
        public FontAtlasData atlas;
        public FontMetricsData metrics;
        public List<FontGlyphData> glyphs;
        public List<FontKerningData> kerning;
    }

    public class FontAtlasData
    {
        public string type;
        public int distanceRange;
        public int size;
        public int width;
        public int height;
        public string yOrigin;
    }

    public class FontMetricsData
    {
        public double emSize;
        public double lineHeight;
        public double ascender;
        public double descender;
        public double underlineY;
        public double underlineThickness;
    }

    public class FontGlyphData
    {
        public int unicode;
        public double advance;
        public Bounds4D planeBounds;
        public Bounds4D atlasBounds;
    }

    public class FontKerningData
    {
        public int unicode1;
        public int unicode2;
        public int unicode3;
        public double advance;
    }
}
