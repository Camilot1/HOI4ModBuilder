using HOI4ModBuilder.src.hoiDataObjects.map.buildings;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text
{
    class SDFTextBundle
    {
        private readonly int id;
        private Dictionary<Font, Info> infosByFonts = new Dictionary<Font, Info>(0);

        public SDFTextBundle()
        {
            id = Utils.random.Next();
            TextureManager.AddSDFTextBundle(this);
        }

        public void AddText(SDFText text)
        {
            if (!infosByFonts.TryGetValue(text.Font, out Info info))
            {
                info = new Info();
                infosByFonts[text.Font] = info;
            }

            info.texts.Add(text);
            info.Assemble();
        }

        public void AddTexts(SDFText[] texts)
        {
            HashSet<Font> fonts = new HashSet<Font>();

            foreach (SDFText text in texts)
            {
                fonts.Add(text.Font);
                if (!infosByFonts.TryGetValue(text.Font, out Info info))
                {
                    infosByFonts[text.Font] = new Info();
                }

                info.texts.Add(text);
            }

            foreach (Font font in fonts)
            {
                infosByFonts[font].Assemble();
            }
        }

        public Info GetFontInfo(Font font)
        {
            return infosByFonts[font];
        }

        public void RemoveText(SDFText text)
        {
            if (infosByFonts.TryGetValue(text.Font, out Info info))
            {
                if (info.texts.Remove(text))
                {
                    info.Assemble();
                }

                if (info.texts.Count == 0) info.Dispose();
            }
        }

        public void Draw()
        {
            foreach (Font font in infosByFonts.Keys)
            {
                GL.BindTexture(TextureTarget.Texture2D, font.atlas.ID);
                infosByFonts[font].Draw();
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        public void Dispose()
        {
            Clear();
            TextureManager.RemoveSDFTextBundle(this);
        }

        public void Clear()
        {
            foreach (Info info in infosByFonts.Values)
                info.Dispose();
            infosByFonts = new Dictionary<Font, Info>(0);
        }

        public override bool Equals(object obj)
        {
            return obj is SDFTextBundle bundle &&
                   id == bundle.id &&
                   EqualityComparer<Dictionary<Font, Info>>.Default.Equals(infosByFonts, bundle.infosByFonts);
        }

        public override int GetHashCode()
        {
            int hashCode = 1187181539;
            hashCode = hashCode * -1521134295 + id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Font, Info>>.Default.GetHashCode(infosByFonts);
            return hashCode;
        }
    }

    class Info
    {
        public int vaoID, vboID;
        public int quadsCount;
        public List<SDFText> texts = new List<SDFText>(0);

        public void Assemble()
        {
            quadsCount = 0;
            foreach (SDFText text in texts) quadsCount += text.GetLength();

            float[] data = new float[quadsCount * 4 * 7];
            int startIndex = 0;

            foreach (SDFText text in texts)
            {
                text.WriteFloats(data, startIndex, -text.GetWidth() / 2f);
                startIndex += text.GetLength() * 4 * 7;
            }

            int elementSize = sizeof(float);

            if (vaoID == 0) GL.GenVertexArrays(1, out vaoID);
            if (vboID == 0) GL.GenBuffers(1, out vboID);

            GL.BindVertexArray(vaoID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * elementSize, data, BufferUsageHint.StaticDraw);

            int stride = 7 * elementSize;
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0); //x, y
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 2 * elementSize); //r, g, b
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 5 * elementSize); //u, v

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.BindVertexArray(vaoID);
            GL.DrawArrays(PrimitiveType.Quads, 0, quadsCount);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(vaoID);
            vaoID = 0;
            GL.DeleteBuffer(vboID);
            vboID = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Info info &&
                   vboID == info.vboID &&
                   vaoID == info.vaoID &&
                   quadsCount == info.quadsCount &&
                   EqualityComparer<List<SDFText>>.Default.Equals(texts, info.texts);
        }

        public override int GetHashCode()
        {
            int hashCode = 962044820;
            hashCode = hashCode * -1521134295 + vboID.GetHashCode();
            hashCode = hashCode * -1521134295 + vaoID.GetHashCode();
            hashCode = hashCode * -1521134295 + quadsCount.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<SDFText>>.Default.GetHashCode(texts);
            return hashCode;
        }
    }
}
