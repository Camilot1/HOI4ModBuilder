using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.hoiDataObjects.units
{
    public struct BaseModifiers
    {
        public float maxOrganisation; //max_organisation
        public float reliability;
        public float weight;
        public float oddMaximumSpeed; //maximum_speed
        public float supplyConsumption; //supply_consumption
        public float defaultMorale; //default_morale
        public float combatWidth; //combat_width
    }

    public struct CostModifiers
    {
        public float lendLeaseCost; //lend_lease_cost
        public uint manpower;
        public uint trainingTimeInDays; //training_time
    }

    public struct OffensiveModifiers
    {
        public float attack;
        public float softAttack; //soft_attack
        public float hardAttack; //hard_attack
        public float airAttack; //air_attack
        public float piercing; //ap_attack
        public float breakthrough;
    }

    public struct DefensiveModifiers
    {
        public float defense;
        public float hp; //max_strength
        public float armor; //armor_value
        public float hardness;
        public float entrenchment;
    }

    public struct UniqueModifiers
    {
        public float speedFactor; //movement
        public float experienceLossFactor; //experience_loss_factor 

    }
}
