using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.ideologies
{
    class Ideology : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string name;
        public IdeologyGroup ideologyGroup;
        public bool canBeRandomlySelected = true;

        public Ideology(string name, IdeologyGroup ideologyGroup)
        {
            this.name = name;
            this.ideologyGroup = ideologyGroup;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "can_be_randomly_selected":
                    canBeRandomlySelected = parser.ReadBool();
                    break;
            }
        }
    }
}
