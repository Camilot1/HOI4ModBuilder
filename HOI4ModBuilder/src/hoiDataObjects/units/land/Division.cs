using HOI4ModBuilder.hoiDataObjects.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.hoiDataObjects.units
{
    class Division
    {
        public string name;
        public Province province; //location
        public DivisionTemplate template; //division_template
        public float experienceFactor; //start_experience_factor
        public float equipmentFactor; //start_equipment_factor

    }
}
