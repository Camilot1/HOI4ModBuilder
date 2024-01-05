using HOI4ModBuilder.hoiDataObjects.units.land;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.units
{
    public class DivisionTemplate
    {
        public string name;
        public IList<TemplateUnit> regiments = new List<TemplateUnit>(0);
        public IList<TemplateUnit> support = new List<TemplateUnit>(0);
        public byte priority;
    }

    public struct TemplateUnit
    {
        public LandUnit unit;
        public Value2B position;
    }
}
