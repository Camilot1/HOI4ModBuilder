using HOI4ModBuilder.hoiDataObjects.history.countries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects
{
    class Faction
    {
        public String tag;
        public Country leader;
        public List<Country> members = new List<Country>(0);
    }
}
