using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.common.terrain
{
    class ProvincialTerrain : IParadoxRead
    {
        public string name;
        public int color;
        public bool isWater;
        public bool isNavalTerrain;


        public ProvincialTerrain(string name)
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "color":
                    IList<int> colors = parser.ReadIntList();
                    color = Utils.ArgbToInt(255, (byte)colors[0], (byte)colors[1], (byte)colors[2]);
                    break;
                case "is_water":
                    isWater = parser.ReadBool();
                    break;
                case "naval_terrain":
                    isNavalTerrain = parser.ReadBool();
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is ProvincialTerrain terrain &&
                   name == terrain.name &&
                   color == terrain.color;
        }

        public override int GetHashCode()
        {
            int hashCode = 1395206702;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + color.GetHashCode();
            return hashCode;
        }
    }
}
