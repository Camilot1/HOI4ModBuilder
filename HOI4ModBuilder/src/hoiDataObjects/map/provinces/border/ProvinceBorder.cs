using HOI4ModBuilder.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    class ProvinceBorder
    {
        public int index;
        public Province provinceA, provinceB;
        public Value2US[] pixels;
        public Point2F center;

        public ProvinceBorder(int index, List<Value2US> pixels, Province provinceA, Province provinceB)
        {
            this.index = index;
            int count = pixels.Count;
            this.pixels = new Value2US[count];

            for (int i = 0; i < count; i++)
            {
                this.pixels[i] = pixels[i];
                center.x += this.pixels[i].x;
                center.y += this.pixels[i].y;
            }

            center.x /= count;
            center.y /= count;

            this.provinceA = provinceA;
            this.provinceB = provinceB;

            provinceA.AddBorder(this);
            provinceB.AddBorder(this);
        }

        public override bool Equals(object obj)
        {
            return obj is ProvinceBorder border &&
                   index == border.index &&
                   EqualityComparer<Province>.Default.Equals(provinceA, border.provinceA) &&
                   EqualityComparer<Province>.Default.Equals(provinceB, border.provinceB);
        }

        public override int GetHashCode()
        {
            int hashCode = 915895052;
            hashCode = hashCode * -1521134295 + index.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Province>.Default.GetHashCode(provinceA);
            hashCode = hashCode * -1521134295 + EqualityComparer<Province>.Default.GetHashCode(provinceB);
            return hashCode;
        }
    }
}
