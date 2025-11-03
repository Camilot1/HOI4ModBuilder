using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Bounds4S
    {
        public short left;
        public short top;
        public short right;
        public short bottom;

        public Bounds4S(short left, short top, short right, short bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public Bounds4S(Bounds4S bounds)
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

        public HashSet<Value2S> ToPositions(short maxWidth, short maxHeight)
        {
            FixDimensions();
            var poses = new HashSet<Value2S>((right - left) * (bottom - top));

            short maxX = right > maxWidth ? maxWidth : right;
            short maxY = bottom > maxHeight ? maxHeight : bottom;

            for (short x = left; x < maxX; x++)
            {
                for (short y = top; y < maxY; y++)
                {
                    poses.Add(new Value2S(x, y));
                }
            }
            return poses;
        }

        public void Set(short left, short top, short right, short bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        public void Set(Bounds4S other)
        {
            left = other.left;
            top = other.top;
            right = other.right;
            bottom = other.bottom;
        }
        public void Set(Value2S value2S)
        {
            left = right = value2S.x;
            top = bottom = value2S.y;
        }
        public void Set(short x, short y)
        {
            left = right = x;
            top = bottom = y;
        }
        public void ExpandIfNeeded(Value2S value2S)
        {
            ExpandIfNeeded(value2S.x, value2S.y);
        }
        public void ExpandIfNeeded(Bounds4S bounds4S)
        {
            ExpandIfNeeded(bounds4S.left, bounds4S.top);
            ExpandIfNeeded(bounds4S.right, bounds4S.bottom);
        }

        public void ExpandIfNeeded(short x, short y)
        {
            if (x < left)
                left = x;
            else if (x > right)
                right = x;

            if (y < top)
                top = y;
            else if (y > bottom)
                bottom = y;
        }
        public void SetZero()
        {
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }
        public bool IsZero()
            => left == top && top == right && right == bottom;
    }
}
