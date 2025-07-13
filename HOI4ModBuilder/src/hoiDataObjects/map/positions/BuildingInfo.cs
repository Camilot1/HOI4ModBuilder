using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.map.positions
{
    public class BuildingInfo
    {
        public string name;
        public object origin;

        public bool isProvincial;
        public bool isOnlyCoastal;
        public bool shouldHaveAdjacentProvince;
        public ushort count;

        public Color3B color;

        public override string ToString()
        {
            return name + " " + count;
        }
    }
}
