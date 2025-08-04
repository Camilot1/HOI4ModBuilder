using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text
{
    public class TextObject : IDisposable
    {
        private readonly FontAtlas _atlas;
        private readonly int _vao;
        private readonly int _vbo;
        private int _vertexCount;

        private bool _isDisposed;

        public string Text;
        public Vector2 Pos;
        public Vector2 Scale;
        public Vector3 Color;

        public TextObject(int length)
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4 * length, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            const int stride = sizeof(float) * 4;
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, stride, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        }

        public unsafe void Assemble()
        {
            float x = 0;
            float y = 0;

            int charCount = Text.Length;
            _vertexCount = charCount * 6;
            int bufferSize = _vertexCount * 4 * sizeof(float);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            EnsureBufferCapacity(bufferSize);

            var ptr = (float*)GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);

            int v = 0;
            for (int i = 0; i < charCount; i++)
            {
                int ch = Text[i];
                if (!_atlas.TryGetGlyph(ch, out var g))
                    continue; // skip unsupported glyphs

                float xpos = x + g.Bearing.X * Scale.X;
                float ypos = y - (g.Size.Y - g.Bearing.Y) * Scale.Y;

                float w = g.Size.X * Scale.X;
                float h = g.Size.Y * Scale.Y;

                // Triangle 1
                ptr[v++] = xpos;
                ptr[v++] = ypos + h;
                ptr[v++] = g.UVMin.X;
                ptr[v++] = g.UVMax.Y;

                ptr[v++] = xpos;
                ptr[v++] = ypos;
                ptr[v++] = g.UVMin.X;
                ptr[v++] = g.UVMin.Y;

                ptr[v++] = xpos + w;
                ptr[v++] = ypos;
                ptr[v++] = g.UVMax.X;
                ptr[v++] = g.UVMin.Y;

                // Triangle 2
                ptr[v++] = xpos;
                ptr[v++] = ypos + h;
                ptr[v++] = g.UVMin.X;
                ptr[v++] = g.UVMax.Y;

                ptr[v++] = xpos + w;
                ptr[v++] = ypos;
                ptr[v++] = g.UVMax.X;
                ptr[v++] = g.UVMin.Y;

                ptr[v++] = xpos + w;
                ptr[v++] = ypos + h;
                ptr[v++] = g.UVMax.X;
                ptr[v++] = g.UVMax.Y;

                x += g.Advance * Scale.X;
            }

            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
        }

        private void EnsureBufferCapacity(int requiredBytes)
        {
            int currentSize;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out currentSize);
            if (requiredBytes <= currentSize)
                return;
            // Resize to next power‑of‑two for amortized O(1).
            int newSize = 1 << (int)Math.Ceiling(Math.Log(requiredBytes, 2));
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GL.DeleteVertexArray(_vao);
                GL.DeleteBuffer(_vbo);
                _isDisposed = true;
            }
        }
    }
}
