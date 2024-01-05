using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.characters.traits
{
    class UnitLeaderTrait
    {
        public EnumType[] types;
        public EnumTraitType traitType;
        public bool showInCombat;
        //TODO allowed
        //TODO ai_will_do
        //TODO newCommanderWeight
        //TODO slot
        
    }

    enum EnumType
    {
        ALL,
        LAND,
        NAVY,
        OPERATIVE,
        FIELD_MARSHAL,
        CORPS_COMMANDER
    }

    enum EnumTraitType
    {
        BASIC,
        PERSONALITY,
        ASSIGNABLE,
        BASIC_TERRAIN,
        ASSIGNABLE_TERRAIN,
        STATUS,
        EXILE
    }
}
