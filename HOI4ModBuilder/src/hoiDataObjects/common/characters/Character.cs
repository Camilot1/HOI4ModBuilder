using HOI4ModBuilder.hoiDataObjects.history.countries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.characters
{
    class Character
    {
        public Country country;
        public string tag;
        public string name;
        public CharacterPortrait civilianPortrait;
        public CharacterPortrait armyPortrait;
        public CharacterPortrait navyPortrait;
    }
}
