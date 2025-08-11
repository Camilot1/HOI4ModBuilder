using QuickFont;
using System;

namespace HOI4ModBuilder.src.openTK.text
{
    public class FontData : IDisposable
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public int Size { get; private set; }
        public QFont Font { get; private set; }
        public bool IsDisposed { get; private set; }

        public FontData(int size, QFont font)
        {
            Size = size;
            Font = font;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            Font?.Dispose();
            IsDisposed = true;
        }

        public override bool Equals(object obj)
        {
            return obj is FontData data &&
                   _hashCode == data._hashCode;
        }
    }
}
