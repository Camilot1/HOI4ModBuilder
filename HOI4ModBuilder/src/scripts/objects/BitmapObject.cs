using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class BitmapObject : IBitmapObject
    {
        private BitmapCustomData _data;
        public object GetValue() => _data;

        public BitmapObject()
        {
            _data = new BitmapCustomData();
        }

        public string GetKeyword() => BitmapDeclarator.GetKeyword();
        public IScriptObject GetEmptyCopy() => new BitmapObject();
        public IScriptObject GetCopy() => new BitmapObject { _data = new BitmapCustomData(_data) };

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is BitmapObject;

        public void Save(int lineIndex, string[] args, IScriptObject value)
        {
            throw new NotImplementedException();
        }

        public void Load(int lineIndex, string[] args, IScriptObject value)
        {
            throw new NotImplementedException();
        }
    }

    public class BitmapCustomData
    {
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public PixelFormat PixelFormat { get; set; }
        public ColorPalette Palette { get; set; }
        public byte[] Bytes { get; set; } = new byte[0];

        public BitmapCustomData() { }

        public BitmapCustomData(BitmapCustomData other)
        {
            if (other != null)
                CopyFrom(other);
        }

        public BitmapCustomData CopyFrom(BitmapCustomData other)
        {
            Path = other.Path;
            Width = other.Width;
            Height = other.Height;
            PixelFormat = other.PixelFormat;

            Bytes = new byte[other.Bytes.Length];
            unsafe
            {
                fixed (byte* pSrc = other.Bytes)
                fixed (byte* pDst = Bytes)
                {
                    Buffer.MemoryCopy(pSrc, pDst, Bytes.Length, other.Bytes.Length);
                }
            }

            return this;
        }

        public BitmapCustomData LoadFrom(Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            PixelFormat = bitmap.PixelFormat;
            Palette = bitmap.Palette;
            Bytes = Utils.BitmapToArray(bitmap, ImageLockMode.ReadOnly, PixelFormat);

            return this;
        }

    }
}
