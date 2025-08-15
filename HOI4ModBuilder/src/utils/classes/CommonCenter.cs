using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;
using System.Windows;

namespace HOI4ModBuilder.src.utils.classes
{
    public class CommonCenter
    {
        private double _sumX, _sumY;
        private uint _pixelsCount;

        public CommonCenter Push(uint pixelsCount, float centerX, float centerY)
        {
            _sumX += centerX * pixelsCount;
            _sumY += centerY * pixelsCount;
            _pixelsCount += pixelsCount;
            return this;
        }

        public CommonCenter Push(uint pixelsCount, Point2F center)
            => Push(pixelsCount, center.x, center.y);

        public void Get(out uint pixelsCount, out Point2F center)
        {
            pixelsCount = _pixelsCount;
            center = default;
            if (_pixelsCount != 0)
            {
                center.x = (float)_sumX / _pixelsCount;
                center.y = (float)_sumY / _pixelsCount;
            }
        }
    }
}
