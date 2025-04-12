using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
