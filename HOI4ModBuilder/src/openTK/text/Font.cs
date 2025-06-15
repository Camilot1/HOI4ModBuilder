using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.openTK.text
{
    class Font
    {
        public string name;
        public Texture2D atlas;
        private Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>(0);

        public Font(string name, FontData data, Texture2D atlas)
        {
            this.name = name;
            this.atlas = atlas;
            LoadData(data);
        }

        public Glyph GetGlyph(char ch)
        {
            if (glyphs.TryGetValue(ch, out Glyph glyph)) return glyph;
            else if (glyphs.TryGetValue('�', out glyph)) return glyph;
            else throw new Exception($"При попытке получить символ '{ch}' из шрифта \"{name}\" произошла ошибка! Не найден сам символ и символ неизвестного символа '�'");
        }

        private void LoadData(FontData data)
        {
            if (data.glyphs == null || data.glyphs.Count == 0) throw new Exception($"Шрифт не содержит глифов!");

            foreach (FontGlyphData glyphData in data.glyphs)
            {
                Value2I size = atlas.Size;

                Bounds4F textureBounds = new Bounds4F(glyphData.atlasBounds);
                textureBounds.left /= (float)size.x;
                textureBounds.top /= (float)size.y;
                textureBounds.right /= (float)size.x;
                textureBounds.bottom /= (float)size.y;


                glyphs[(char)glyphData.unicode] = new Glyph(
                    (char)glyphData.unicode,
                    0,
                    (float)glyphData.advance,
                    new Bounds4F(glyphData.planeBounds),
                    textureBounds
                );
            }

            if (data.kerning == null || data.kerning.Count == 0) return;

            foreach (FontKerningData kerningData in data.kerning)
            {
                Glyph glyph;
                if (glyphs.TryGetValue((char)kerningData.unicode1, out glyph)) glyph.kerning = (float)kerningData.advance;
                if (glyphs.TryGetValue((char)kerningData.unicode2, out glyph)) glyph.kerning = (float)kerningData.advance;
                if (glyphs.TryGetValue((char)kerningData.unicode3, out glyph) && glyph.kerning != 0) throw new Exception($"Переполнение kerningData (unicode3 != 0) для шрифта {name}");
            }
        }

        public void Dispose()
        {
            if (atlas != null) atlas.Dispose();
        }

        public override bool Equals(object obj)
        {
            return obj is Font font &&
                   name == font.name;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(name);
        }
    }
}
