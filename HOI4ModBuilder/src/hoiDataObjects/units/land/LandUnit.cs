using HOI4ModBuilder.hoiDataObjects.gfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.hoiDataObjects.units.land
{
    public class LandUnit
    {
        public string tag;
        public Sprite sprite;
        //mapIconCategory
        public byte aiPriority;
        public byte priority;
        public bool isActive;
        //group
        //categories

        BaseModifiers baseModifiers;

        CostModifiers costModifiers;

        OffensiveModifiers offensiveModifiers;

        DefensiveModifiers defensiveModifiers;
    }

}
