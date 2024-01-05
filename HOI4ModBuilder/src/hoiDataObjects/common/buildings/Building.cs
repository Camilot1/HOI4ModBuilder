using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    class Building : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public uint iconFrame;
        public string name;
        public EnumBuildingSlotCategory enumBuildingSlotCategory = EnumBuildingSlotCategory.NON_SHARED;
        public int baseCost;
        public int baseCostConversion;
        public int perLevelExtraCost;
        public bool infrastructureConstructionEffect;
        public uint maxLevel = 15;
        public uint value = 1;
        public float damageFactor;
        public bool alliedBuild;
        public bool onlyCoastal;
        public bool disabledInDMZ;

        public uint showOnMapCount;
        public byte showOnMapMeshes = 1;
        public bool alwaysShown;
        public bool hasDestroyedMesh;
        public bool centered;

        public bool showModifier;
        public List<DataArgsBlock> modifiers = new List<DataArgsBlock>(0);

        public bool isPort;

        public Building(string name)
        {
            this.name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Building building &&
                   name == building.name;
        }


        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "base_cost":
                    baseCost = parser.ReadInt32();
                    break;
                case "base_cost_conversion":
                    baseCostConversion = parser.ReadInt32();
                    break;
                case "per_level_extra_cost":
                    perLevelExtraCost = parser.ReadInt32();
                    break;
                case "infrastructure_construction_effect":
                    infrastructureConstructionEffect = parser.ReadBool();
                    break;
                case "provincial":
                    if (parser.ReadBool()) enumBuildingSlotCategory = EnumBuildingSlotCategory.PROVINCIAL;
                    break;
                case "shares_slots":
                    if (parser.ReadBool()) enumBuildingSlotCategory = EnumBuildingSlotCategory.SHARED;
                    else enumBuildingSlotCategory = EnumBuildingSlotCategory.NON_SHARED;
                    break;
                case "max_level":
                    maxLevel = parser.ReadUInt32();
                    break;
                case "value":
                    value = parser.ReadUInt32();
                    break;
                case "damage_factor":
                    damageFactor = parser.ReadFloat();
                    break;
                case "allied_build":
                    alliedBuild = parser.ReadBool();
                    break;
                case "only_costal":
                    onlyCoastal = parser.ReadBool();
                    break;
                case "disabled_in_dmz":
                    disabledInDMZ = parser.ReadBool();
                    break;
                case "icon_frame":
                    iconFrame = parser.ReadUInt32();
                    break;
                case "show_on_map":
                    showOnMapCount = parser.ReadUInt32();
                    break;
                case "show_on_map_meshes":
                    showOnMapMeshes = parser.ReadByte();
                    break;
                case "always_shown":
                    alwaysShown = parser.ReadBool();
                    break;
                case "has_destroyed_mesh":
                    hasDestroyedMesh = parser.ReadBool();
                    break;
                case "centered":
                    centered = parser.ReadBool();
                    break;
                case "show_modifier":
                    showModifier = parser.ReadBool();
                    break;
                default:

                    try
                    {
                        DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, modifiers);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_BUILDING_LOADING,
                            new Dictionary<string, string>
                            {
                                { "{buildingName}", name },
                                { "{token}", token }
                            }
                        ), ex);
                    }
                    break;
            }
        }

        public enum EnumBuildingSlotCategory
        {
            SHARED,
            NON_SHARED,
            PROVINCIAL
        }
    }
}
