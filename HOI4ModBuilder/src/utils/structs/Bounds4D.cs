using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct Bounds4D
    {
        public double left;
        public double top;
        public double right;
        public double bottom;

        public Bounds4D(double left, double top, double right, double bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
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
