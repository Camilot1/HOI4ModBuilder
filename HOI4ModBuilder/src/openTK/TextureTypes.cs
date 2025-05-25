using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder
{
    enum EnumTextureType
    {
        _8bppAlpha,
        _8bppGrayScale,
        _8bppIndexed,
        _24bppRgb,
        _32bppArgb
    }

    struct TextureType
    {
        public System.Drawing.Imaging.PixelFormat imagePixelFormat;
        public PixelInternalFormat pixelInternalFormat;
        public PixelFormat openGLPixelFormat;
        public byte bytesPerPixel;

        public TextureType(System.Drawing.Imaging.PixelFormat imagePixelFormat, PixelInternalFormat pixelInternalFormat, PixelFormat openGLPixelFormat, byte bytesPerPixel)
        {
            this.imagePixelFormat = imagePixelFormat;
            this.pixelInternalFormat = pixelInternalFormat;
            this.openGLPixelFormat = openGLPixelFormat;
            this.bytesPerPixel = bytesPerPixel;
        }

        public static bool operator ==(TextureType current, TextureType other)
        {
            return current.Equals(other);
        }
        public static bool operator !=(TextureType current, TextureType other)
        {
            return !current.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is TextureType type &&
                   imagePixelFormat == type.imagePixelFormat &&
                   pixelInternalFormat == type.pixelInternalFormat &&
                   openGLPixelFormat == type.openGLPixelFormat &&
                   bytesPerPixel == type.bytesPerPixel;
        }

        public override int GetHashCode()
        {
            int hashCode = 1531596680;
            hashCode = hashCode * -1521134295 + imagePixelFormat.GetHashCode();
            hashCode = hashCode * -1521134295 + pixelInternalFormat.GetHashCode();
            hashCode = hashCode * -1521134295 + openGLPixelFormat.GetHashCode();
            hashCode = hashCode * -1521134295 + bytesPerPixel.GetHashCode();
            return hashCode;
        }
    }

    class TextureTypes
    {
        private static Dictionary<EnumTextureType, TextureType> _textureTypes = InitTextureTypes();

        private static Dictionary<EnumTextureType, TextureType> InitTextureTypes()
        {
            var textureTypes = new Dictionary<EnumTextureType, TextureType>()
            {
                { EnumTextureType._8bppAlpha, new TextureType(System.Drawing.Imaging.PixelFormat.Format8bppIndexed, PixelInternalFormat.Alpha, PixelFormat.Alpha, 1) },
                { EnumTextureType._8bppGrayScale, new TextureType(System.Drawing.Imaging.PixelFormat.Format8bppIndexed, PixelInternalFormat.Luminance, PixelFormat.Luminance, 1) },
                { EnumTextureType._8bppIndexed, new TextureType(System.Drawing.Imaging.PixelFormat.Format8bppIndexed, PixelInternalFormat.Rgb, PixelFormat.Bgr, 3) },
                { EnumTextureType._24bppRgb, new TextureType(System.Drawing.Imaging.PixelFormat.Format24bppRgb, PixelInternalFormat.Rgb, PixelFormat.Bgr, 3) },
                { EnumTextureType._32bppArgb, new TextureType(System.Drawing.Imaging.PixelFormat.Format32bppArgb, PixelInternalFormat.Rgba, PixelFormat.Bgra, 4) }
            };
            return textureTypes;
        }

        public static TextureType Get(EnumTextureType textureType) => _textureTypes[textureType];
    }
}
