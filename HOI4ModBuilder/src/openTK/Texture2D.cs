using System;
using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using static HOI4ModBuilder.utils.Structs;
using System.Runtime.InteropServices;

namespace HOI4ModBuilder
{

    class Texture2D : IDisposable
    {
        public int ID { get; private set; }
        public Value2I size;
        private int BufferID { get; set; }
        private float[] Coordinates { get; set; }


        public Texture2D(Bitmap bitmap, TextureType textureType, bool linear)
        {
            size.x = bitmap.Width;
            size.y = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, size.x, size.y), ImageLockMode.ReadOnly, textureType.imagePixelFormat);

            Init(data, textureType, linear, null);
            bitmap.UnlockBits(data);
            TextureManager.AddTexture(this);
        }

        public Texture2D(Bitmap bitmap, BitmapData data, TextureType textureType, bool linear)
        {
            size.x = bitmap.Width;
            size.y = bitmap.Height;
            Init(data, textureType, linear, null);
            bitmap.UnlockBits(data);
            TextureManager.AddTexture(this);
        }

        public Texture2D(TextureType textureType, bool linear, float[] coordinates, int width, int height, byte[] data)
        {
            size.x = width;
            size.y = height;
            Init(null, textureType, linear, coordinates);
            Set(textureType, 0, 0, size.x, size.y, data);
            TextureManager.AddTexture(this);
        }

        private void Init(in BitmapData data, in TextureType textureType, bool linear, float[] coordinates)
        {
            ID = GL.GenTexture();

            if (ID == 0) throw new Exception("Can't generate ID for Texture2D");

            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, linear ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, linear ? (int)TextureMagFilter.Linear : (int)TextureMagFilter.Nearest);
            if (data != null) Update(data, textureType);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            Coordinates = coordinates != null ? coordinates : new float[]
            {
                0f, 1f, //LU
                1f, 1f, //RU
                1f, 0f, //RD
                0f, 0f //LD
            };

            BufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, Coordinates.Length * sizeof(float), Coordinates, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void Update(in BitmapData data, in TextureType textureType)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, textureType.pixelInternalFormat, size.x, size.y, 0, textureType.openGLPixelFormat, PixelType.UnsignedByte, data.Scan0);
        }

        public void Set(in TextureType textureType, int x, int y, int width, int height, byte[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, textureType.pixelInternalFormat, width, height, 0, textureType.openGLPixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, textureType.openGLPixelFormat, PixelType.UnsignedByte, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Update(in TextureType textureType, int x, int y, int width, int height, byte[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, textureType.openGLPixelFormat, PixelType.UnsignedByte, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Update(in TextureType textureType, int x, int y, int width, int height, int[] data)
        {
            byte bytesPerPixel = textureType.bytesPerPixel;

            byte[] values = new byte[data.Length * bytesPerPixel];
            int value;
            int index;

            if (textureType.openGLPixelFormat == OpenTK.Graphics.OpenGL.PixelFormat.Bgr)
            {
                if (bytesPerPixel == 3)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        value = data[i];
                        index = i * bytesPerPixel;
                        values[index] = (byte)value;
                        values[index + 1] = (byte)(value >> 8);
                        values[index + 2] = (byte)(value >> 16);
                    }
                }
                else throw new Exception($"Texture Update() method error code: {textureType.openGLPixelFormat} {bytesPerPixel}");
            }
            else if (textureType.openGLPixelFormat == OpenTK.Graphics.OpenGL.PixelFormat.Rgb)
            {
                if (bytesPerPixel == 3)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        value = data[i];
                        index = i * bytesPerPixel;
                        values[index + 2] = (byte)(value >> 16);
                        values[index + 1] = (byte)(value >> 8);
                        values[index] = (byte)value;
                    }
                }
                else throw new Exception($"Texture Update() method error code: {textureType.openGLPixelFormat} {bytesPerPixel}");
            }
            else throw new Exception($"Texture Update() method error code: {textureType.openGLPixelFormat} {bytesPerPixel}");

            Update(textureType, x, y, width, height, values);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferID);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(BufferID);
            GL.DeleteTexture(ID);
            TextureManager.RemoveTexture(this);
        }

        public Value2I GetSize()
        {
            return size;
        }
    }
}
