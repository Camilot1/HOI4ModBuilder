
namespace HOI4ModBuilder.src.utils.structs
{
    public struct Bounds4F
    {
        public float left;
        public float top;
        public float right;
        public float bottom;

        public Bounds4F(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public Bounds4F(Bounds4D bounds)
        {
            left = (float)bounds.left;
            top = (float)bounds.top;
            right = (float)bounds.right;
            bottom = (float)bounds.bottom;
        }

        public bool IsIntersectsWith(Bounds4F other)
        {
            bool overlapX = left <= other.right && right >= other.left;
            bool overlapY = top <= other.bottom && bottom >= other.top;

            return overlapX && overlapY;
        }

        public bool IsInbounds(float x, float y)
            => left <= x && x <= right && top <= y && y <= bottom;

        public bool IsInbounds(Point2D pos)
            => IsInbounds((float)pos.x, (float)pos.y);

        public bool HasSpace() => left != right && top != bottom;

        public void FixDimensions()
        {
            if (left > right)
                (right, left) = (left, right);
            if (top > bottom)
                (bottom, top) = (top, bottom);
        }

        public override string ToString()
            => "Bounds4F { left = " + left + ", top = " + top + ", right = " + right + ", bottom = " + bottom + "}";
    }
}
