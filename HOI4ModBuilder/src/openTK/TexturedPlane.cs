using HOI4ModBuilder.managers;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using System;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder
{
    class TexturedPlane : IDisposable
    {
        [JsonIgnore]
        protected int VertexBufferID;
        [JsonIgnore]
        protected float[] vertexData;
        [JsonIgnore]
        public Texture2D Texture { get; set; }
        public Point2D pos;
        public Point2D size;

        public TexturedPlane()
        {

        }

        public TexturedPlane(Texture2D texture, float width, float height)
        {
            Texture = texture;
            size.x = width;
            size.y = height;
            VertexBufferID = GL.GenBuffer();

            if (VertexBufferID == 0) throw new Exception("Can't generate VertexBufferID for TexturedPlane.");

            Resize();
        }

        public void LoadTextureInfo(TextureInfo info)
        {
            pos = info.plane.pos;
            size = info.plane.size;
            Resize();
        }

        public void Scale(float factor)
        {
            size.Scale(factor);
            Resize();
        }

        public void Resize()
        {
            vertexData = new float[]
            {
                0f, 0f, 0f,
                (float)size.x, 0f, 0f,
                (float) size.x, (float) size.y, 0f,
                0f, (float) size.y, 0f
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Move(double dx, double dy)
        {
            pos.x += dx;
            pos.y += dy;
        }

        public void MoveTo(double x, double y)
        {
            pos.x = x;
            pos.y = y;
        }

        public void Draw()
        {
            EnableStates();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            Texture.Bind();
            GL.DrawArrays(PrimitiveType.Quads, 0, vertexData.Length);
            Texture.Unbind();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            DisableStates();
        }

        protected void EnableStates()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
        }

        protected void DisableStates()
        {
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
        }

        public void Dispose()
        {
            Texture?.Dispose();
            GL.DeleteBuffer(VertexBufferID);
        }

        public bool Inbounds(Point2D point)
        {
            return pos.x <= point.x && point.x <= pos.x + size.x && pos.y - size.y <= point.y && point.y <= pos.y;
        }
    }
}
