using HOI4ModBuilder.hoiDataObjects.units;
using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.common.ideologies
{
    class IdeologyGroup : IParadoxRead
    {
        public string name;
        public IList<Ideology> ideologies = new List<Ideology>(0);
        public IList<string> dynamicFactionNames = new List<string>(0);
        public Color3B color;
        public IdeologyGroupRules rules = new IdeologyGroupRules();
        public bool canBeBoosted = true;
        public bool canCollaborate = false;
        public bool canHostGovernmentInExile;
        public float warImpactOnWorldTension;
        public float factionImpactOnWorldTension;
        public EnumAiIdeology aiIdeology;
        public float aiIdeologyWantedUnitsFactor;
        public List<DataArgsBlock> modifiers = new List<DataArgsBlock>(0);
        public List<DataArgsBlock> factionModifiers = new List<DataArgsBlock>(0);

        public IdeologyGroup(string name)
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "types":
                    IdeologyTypes types = new IdeologyTypes(this, ideologies);
                    parser.Parse(types);
                    break;
                case "dynamic_faction_names":
                    dynamicFactionNames = parser.ReadStringList();
                    break;
                case "color":
                    IList<int> colorData = parser.ReadIntList();
                    if (colorData.Count == 3)
                    {
                        color.red = (byte)colorData[0];
                        color.green = (byte)colorData[1];
                        color.blue = (byte)colorData[2];
                    }
                    break;
                case "rules":
                    parser.Parse(rules);
                    break;
                case "can_be_boosted":
                    canBeBoosted = parser.ReadBool();
                    break;
                case "can_collaborate":
                    canCollaborate = parser.ReadBool();
                    break;
                case "can_host_government_in_exile":
                    canHostGovernmentInExile = parser.ReadBool();
                    break;
                case "war_impact_on_world_tension":
                    warImpactOnWorldTension = parser.ReadFloat();
                    break;
                case "faction_impact_on_world_tension":
                    factionImpactOnWorldTension = parser.ReadFloat();
                    break;
                case "ai_democratic":
                    aiIdeology = EnumAiIdeology.DEMOCRATIC;
                    break;
                case "ai_communism":
                    aiIdeology = EnumAiIdeology.COMMUNISM;
                    break;
                case "ai_fascism":
                    aiIdeology = EnumAiIdeology.FASCISM;
                    break;
                case "ai_neutral":
                    aiIdeology = EnumAiIdeology.NEUTRAL;
                    break;
                case "ai_ideology_wanted_units_factor":
                    aiIdeologyWantedUnitsFactor = parser.ReadFloat();
                    break;
                case "modifiers":
                    var modifiersList = new IdeologyGroupModifierList(name, modifiers);
                    parser.Parse(modifiersList);
                    break;
                case "faction_modifiers":
                    var factionModifiersList = new IdeologyGroupModifierList(name, factionModifiers);
                    parser.Parse(factionModifiersList);
                    break;
            }
        }
    }

    class IdeologyGroupModifierList : IParadoxRead
    {
        private string _ideologyGroupName;
        private List<DataArgsBlock> _modifiers;

        public IdeologyGroupModifierList(string ideologyGoupName, List<DataArgsBlock> modifiers)
        {
            _ideologyGroupName = ideologyGoupName;
            _modifiers = modifiers;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            try
            {
                DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, _modifiers);
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_WHILE_IDEOLOGY_GROUP_MODIFIERS_LOADING,
                    new Dictionary<string, string>
                    {
                        { "{ideologyGroupName}", _ideologyGroupName },
                        { "{token}", token }
                    }
                ), ex);
            }
        }
    }

    class IdeologyGroupRules : IParadoxRead
    {
        public bool canCreateCollaborationGovernment;
        public bool canDeclareWarOnSameIdeology;
        public bool canFarceGovernment;
        public bool canSendVolunteers;
        public bool canPuppet;
        public bool canLowerTension;
        public bool canOnlyJustifyWarOnThreatCountry;
        public bool canGuaranteeOtherIdeologies;
        public bool canCreateFaction;
        public bool canBoostOtherIdeologies;

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "can_create_collaboration_government":
                    canCreateCollaborationGovernment = parser.ReadBool();
                    break;
                case "can_declare_war_on_same_ideology":
                    canDeclareWarOnSameIdeology = parser.ReadBool();
                    break;
                case "can_force_government":
                    canFarceGovernment = parser.ReadBool();
                    break;
                case "can_send_volunteers":
                    canSendVolunteers = parser.ReadBool();
                    break;
                case "can_puppet":
                    canPuppet = parser.ReadBool();
                    break;
                case "can_lower_tension":
                    canLowerTension = parser.ReadBool();
                    break;
                case "can_only_justify_war_on_threat_country":
                    canOnlyJustifyWarOnThreatCountry = parser.ReadBool();
                    break;
                case "can_guarantee_other_ideologies":
                    canGuaranteeOtherIdeologies = parser.ReadBool();
                    break;
                case "can_create_faction":
                    canCreateFaction = parser.ReadBool();
                    break;
                case "can_boost_other_ideologies":
                    canBoostOtherIdeologies = parser.ReadBool();
                    break;
            }
        }

    }

    class IdeologyTypes : IParadoxRead
    {
        public IdeologyGroup ideologyGroup;
        public IList<Ideology> ideologies;

        public IdeologyTypes(in IdeologyGroup ideologyGroup, in IList<Ideology> ideologies)
        {
            this.ideologyGroup = ideologyGroup;
            this.ideologies = ideologies;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            Ideology ideology = new Ideology(token, ideologyGroup);
            parser.Parse(ideology);
            ideologies.Add(ideology);
        }
    }

    enum EnumAiIdeology
    {
        DEMOCRATIC,
        COMMUNISM,
        FASCISM,
        NEUTRAL
    }
}
