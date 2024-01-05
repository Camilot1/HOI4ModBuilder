using HOI4ModBuilder.src.hoiDataObjects.common.characters.traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.characters
{
    class RoleAdvisor
    {
        public EnumSlot slot;
        public EnumLedger ledger;
        //idea_token
        public bool canBeFired;
        public CountryLeaderTrait[] traits;


    }

    enum EnumSlot
    {
        POLITICAL_ADVISOR,
        THEORIST,
        ARMY_CHIEF,
        NAVE_CHIEF,
        AIR_CHIEF,
        HIGH_COMMAND
    }

    enum EnumLedger
    {
        AIR,
        NAVY,
        MILITARY,
        CIVILIAN,
        ALL,
        HIDDEN
    }
}
