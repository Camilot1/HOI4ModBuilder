using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Bounds4US
    {
        public ushort left;
        public ushort top;
        public ushort right;
        public ushort bottom;

        public Bounds4US(ushort left, ushort top, ushort right, ushort bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public Bounds4US(Bounds4US bounds)
        {
            left = bounds.left;
            top = bounds.top;
            right = bounds.right;
            bottom = bounds.bottom;
        }

        public bool Inbounds(Point2D pos)
        {
            return left <= pos.x && pos.x <= right && top <= pos.y && pos.y <= bottom;
        }

        public bool HasSpace()
        {
            return left != right && top != bottom;
        }

        public void FixDimensions()
        {
            if (left > right) right = left;
            if (top > bottom) bottom = top;
        }

        public HashSet<Value2US> ToPositions(ushort maxWidth, ushort maxHeight)
        {
            FixDimensions();
            var poses = new HashSet<Value2US>((right - left) * (bottom - top));

            ushort maxX = right > maxWidth ? maxWidth : right;
            ushort maxY = bottom > maxHeight ? maxHeight : bottom;

            for (ushort x = left; x < maxX; x++)
            {
                for (ushort y = top; y < maxY; y++)
                {
                    poses.Add(new Value2US(x, y));
                }
            }
            return poses;
        }

        internal void Set(ushort left, ushort top, ushort right, ushort bottom)
        {
            this.left = left;
            this.top = top;
            this.left = left;
            this.bottom = bottom;
        }
    }
}
