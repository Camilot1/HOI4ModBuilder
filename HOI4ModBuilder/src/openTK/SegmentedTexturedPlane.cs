using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;
using static System.Net.Mime.MediaTypeNames;

namespace HOI4ModBuilder.src.openTK
{
    class SegmentedTexturedPlane : TexturedPlane
    {
        [JsonIgnore]
        public List<Texture2D> textures = new List<Texture2D>(0);

        public SegmentedTexturedPlane() : base()
        {

        }

        public SegmentedTexturedPlane(Texture2D texture, float width, float height) : base(texture, width, height)
        {
        }

        public SegmentedTexturedPlane(List<Texture2D> textures, float width, float height) : base(null, width, height)
        {
            this.textures = textures;
        }

        public void SetTextures(List<Texture2D> textures)
        {
            this.textures = textures;
        }

        public new void Draw()
        {
            EnableStates();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            foreach (var texture in textures)
            {
                texture.Bind();
                GL.DrawArrays(PrimitiveType.Quads, 0, vertexData.Length);
            }
            if (textures.Count > 0)
            {
                textures[textures.Count - 1].Unbind();
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            DisableStates();
        }

        public new void Dispose()
        {
            if (textures != null)
            {
                foreach (var texture in textures) texture.Dispose();
            }
            base.Dispose();
        }
    }
}
