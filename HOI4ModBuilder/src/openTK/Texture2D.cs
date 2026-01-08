using System;
using System.Drawing.Imaging;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.managers.texture;
using System.Diagnostics;

namespace HOI4ModBuilder
{

    public class Texture2D : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public int TextureId { get; private set; }
        private Value2I _size;
        public Value2I Size => _size;
        private int BufferID { get; set; }
        private float[] Coordinates { get; set; }
        public TextureType TextureType { get; private set; }
        private static readonly float[] DefaultCoordinates = new float[]
        {
            0f, 1f, //LU
            1f, 1f, //RU
            1f, 0f, //RD
            0f, 0f //LD
        };


        public Texture2D(Bitmap bitmap, TextureType textureType, bool linear)
        {
            _size.Set(bitmap.Width, bitmap.Height);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, _size.x, _size.y), ImageLockMode.ReadOnly, textureType.imagePixelFormat);

            Init(data, textureType, linear, null);
            bitmap.UnlockBits(data);
            TextureManager.AddTexture(this);
        }

        public Texture2D(Bitmap bitmap, BitmapData data, TextureType textureType, bool linear)
        {
            _size.Set(bitmap.Width, bitmap.Height);
            Init(data, textureType, linear, null);
            bitmap.UnlockBits(data);
            TextureManager.AddTexture(this);
        }

        public Texture2D(TextureType textureType, bool linear, float[] coordinates, int width, int height, byte[] data)
        {
            _size.Set(width, height);
            Init(null, textureType, linear, coordinates);
            Set(textureType, 0, 0, _size.x, _size.y, data);
            TextureManager.AddTexture(this);
        }

        private void Init(in BitmapData data, in TextureType textureType, bool linear, float[] coordinates)
        {
            TextureType = textureType;

            TextureId = GL.GenTexture();

            if (TextureId == 0)
                throw new Exception("Can't generate ID for Texture2D");

            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, linear ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, linear ? (int)TextureMagFilter.Linear : (int)TextureMagFilter.Nearest);
            if (data != null)
                Update(data, textureType);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            Coordinates = coordinates ?? DefaultCoordinates;

            BufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, Coordinates.Length * sizeof(float), Coordinates, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void Update(in BitmapData data, in TextureType textureType)
        {
            ValidateBitmapDataSize(data, textureType, _size.x, _size.y);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexImage2D(TextureTarget.Texture2D, 0, textureType.pixelInternalFormat, _size.x, _size.y, 0, textureType.openGLPixelFormat, PixelType.UnsignedByte, data.Scan0);
        }

        public void Set(in TextureType textureType, int x, int y, int width, int height, byte[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            if (data == null)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, textureType.pixelInternalFormat, width, height, 0, textureType.openGLPixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            }
            else if (x == 0 && y == 0)
            {
                ValidateByteDataSize(data, textureType, width, height);
                GL.TexImage2D(TextureTarget.Texture2D, 0, textureType.pixelInternalFormat, width, height, 0, textureType.openGLPixelFormat, PixelType.UnsignedByte, data);
            }
            else
            {
                ValidateByteDataSize(data, textureType, width, height);
                GL.TexImage2D(TextureTarget.Texture2D, 0, textureType.pixelInternalFormat, width, height, 0, textureType.openGLPixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, textureType.openGLPixelFormat, PixelType.UnsignedByte, data);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Update(in TextureType textureType, int x, int y, int width, int height, byte[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            ValidateByteDataSize(data, textureType, width, height);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, textureType.openGLPixelFormat, PixelType.UnsignedByte, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private static void ValidateByteDataSize(byte[] data, in TextureType textureType, int width, int height)
        {
            if (data == null)
                return;

            int expected = width * height * textureType.bytesPerPixel;
            Debug.Assert(data.Length == expected, $"Texture data size mismatch. Expected {expected}, got {data.Length}.");
            if (data.Length != expected)
                throw new ArgumentException($"Texture data size mismatch. Expected {expected}, got {data.Length}.");
        }

        private static void ValidateBitmapDataSize(in BitmapData data, in TextureType textureType, int width, int height)
        {
            if (data == null)
                return;

            int required = width * height * textureType.bytesPerPixel;
            int available = Math.Abs(data.Stride) * height;
            Debug.Assert(available >= required, $"BitmapData size mismatch. Required {required}, available {available}.");
            if (available < required)
                throw new ArgumentException($"BitmapData size mismatch. Required {required}, available {available}.");
        }

        /** Deprecated **/
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
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferID);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Save(string fileName)
        {
            Bitmap bitmap = new Bitmap(_size.x, _size.y, TextureType.imagePixelFormat);

            if (TextureType.pixelInternalFormat == PixelInternalFormat.Luminance)
            {
                var pallete = bitmap.Palette;
                var entries = pallete.Entries;
                for (int i = 0; i < entries.Length; i++)
                    entries[i] = Color.FromArgb(255, i, i, i);
                bitmap.Palette = pallete;
            }

            var bytes = new byte[_size.x * _size.y * TextureType.bytesPerPixel];
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.GetTexImage(TextureTarget.Texture2D, 0, TextureType.openGLPixelFormat, PixelType.UnsignedByte, bytes);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            Utils.ArrayToBitmap(bytes, bitmap, ImageLockMode.WriteOnly, _size.x, _size.y, TextureType);
            bitmap.Save(fileName);
            bitmap.Dispose();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                GL.DeleteBuffer(BufferID);
                GL.DeleteTexture(TextureId);
                IsDisposed = true;
            }
            TextureManager.RemoveTexture(this);
        }

    }
}
