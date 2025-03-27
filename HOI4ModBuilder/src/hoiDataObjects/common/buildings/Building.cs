using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.dataObjects.parameters;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class Building : IParadoxObject, INeedToSave
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;


        public bool _needToSave;

        public bool NeedToSave() => _needToSave;
        public void SetNeedToSave(bool flag) => _needToSave = flag;

        public static readonly Dictionary<string, ParserFlags<Building>> FORMATTER = new Dictionary<string, ParserFlags<Building>>();

        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        public ParadoxParameter<List<DataArgsBlock>> DlcAllowed { get; }
        public static readonly ParserFlags<Building> F_DlcAllowed = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "dlc_allowed",
            PROVIDER = (o) => o.DlcAllowed,
            FABRIC = () => new List<DataArgsBlock>()
        });

        public ParadoxParameter<ushort> ShowOnMapCount { get; }
        public static readonly ParserFlags<Building> F_ShowOnMapCount = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "show_on_map",
            PROVIDER = (o) => o.ShowOnMapCount,
            DEFAULT = 0
        });

        public ParadoxParameter<ushort> ShowOnMapMeshes { get; }
        public static readonly ParserFlags<Building> F_ShowOnMapMeshes = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "show_on_map_meshes",
            PROVIDER = (o) => o.ShowOnMapMeshes,
            DEFAULT = 0
        });

        public ParadoxParameter<bool> ShowModifier { get; }
        public static readonly ParserFlags<Building> F_ShowModifier = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "show_modifier",
            PROVIDER = (o) => o.ShowModifier
        });

        public ParadoxParameter<ushort> IconFrame { get; }

        public static readonly ParserFlags<Building> F_IconFrame = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "icon_frame",
            PROVIDER = (o) => o.IconFrame
        });

        public ParadoxParameter<uint> BaseCost { get; }
        public static readonly ParserFlags<Building> F_BaseCost = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "base_cost",
            PROVIDER = (o) => o.BaseCost
        });

        public ParadoxParameter<uint> BaseCostConvertion { get; }

        public static readonly ParserFlags<Building> F_BaseCostConvertion = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "base_cost_conversion",
            PROVIDER = (o) => o.BaseCostConvertion
        });

        public ParadoxParameter<uint> ExtraCostPerLevel { get; }
        public static readonly ParserFlags<Building> F_ExtraCostPerLevel = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "per_level_extra_cost",
            PROVIDER = (o) => o.ExtraCostPerLevel
        });

        public ParadoxParameter<uint> ExtraCostPerControllerBuilding { get; }
        public static readonly ParserFlags<Building> F_ExtraCostPerControllerBuilding = ParadoxUtils.Register(FORMATTER, new ParserFlags<Building>
        {
            TOKEN = "per_controlled_building_extra_cost",
            PROVIDER = (o) => o.ExtraCostPerControllerBuilding
        });

        private static readonly string TOKEN_HAS_INFRASTRUCTUAL_CONTSRUCTION_EFFECT = "infrastructure_construction_effect";
        private ParadoxParameter<bool> _hasInfrastructureConstructionEffect;
        public ParadoxParameter<bool> HasInfrastructureConstructionEffect
        {
            get => _hasInfrastructureConstructionEffect;
            set => Utils.Setter(ref _hasInfrastructureConstructionEffect, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_BASE_HEALTH = "value";
        private ParadoxParameter<uint> _baseHealth;
        public ParadoxParameter<uint> BaseHealth
        {
            get => _baseHealth;
            set => Utils.Setter(ref _baseHealth, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_DAMAGE_FACTOR = "damage_factor";
        private ParadoxParameter<float> _damageFactor;
        public ParadoxParameter<float> DamageFactor
        {
            get => _damageFactor;
            set => Utils.Setter(ref _damageFactor, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_REPAIR_SPEED_FACTOR = "repair_speed_factor";
        private ParadoxParameter<float> _repairSpeedFactor;
        public ParadoxParameter<float> RepairSpeedFactor
        {
            get => _repairSpeedFactor;
            set => Utils.Setter(ref _repairSpeedFactor, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_IS_ALLIED_BUILD = "allied_build";
        private ParadoxParameter<bool> _isAlliedBuild;
        public ParadoxParameter<bool> IsAlliedBuild
        {
            get => _isAlliedBuild;
            set => Utils.Setter(ref _isAlliedBuild, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_IS_ONLY_COASTAL = "only_costal";
        private ParadoxParameter<bool> _isOnlyCoastal;
        public ParadoxParameter<bool> IsOnlyCoastal
        {
            get => _isOnlyCoastal;
            set => Utils.Setter(ref _isOnlyCoastal, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_IS_DISABLED_IN_DMZ = "disabled_in_dmz";
        private ParadoxParameter<bool> _isDisabledInDMZ;
        public ParadoxParameter<bool> IsDisabledInDMZ
        {
            get => _isDisabledInDMZ;
            set => Utils.Setter(ref _isDisabledInDMZ, ref value, ref _needToSave);
        }

        private ParadoxParameter<BuildingLevelCap> _buildingLevelCap;
        public ParadoxParameter<BuildingLevelCap> BuildingLevelCap
        {
            get => _buildingLevelCap;
        }

        private static readonly string TOKEN_STATE_MODIFIERS = "state_modifiers";
        private ParadoxParameter<List<DataArgsBlock>> _stateModifiers;


        public EnumBuildingSlotCategory enumBuildingSlotCategory = EnumBuildingSlotCategory.NON_SHARED;
        public uint maxLevel = 15;
        public bool onlyCoastal;
        public bool disabledInDMZ;

        public bool alwaysShown;
        public bool hasDestroyedMesh;
        public bool centered;

        private List<DataArgsBlock> _modifiers = new List<DataArgsBlock>();

        public bool isPort;


        public Building(string name)
        {
            _name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Building building &&
                   _name == building._name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
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
                case "always_shown":
                    alwaysShown = parser.ReadBool();
                    break;
                case "has_destroyed_mesh":
                    hasDestroyedMesh = parser.ReadBool();
                    break;
                case "centered":
                    centered = parser.ReadBool();
                    break;

            }
        }

        public void ExecuteAfterParse()
        {
            ExecuteBlockFunctions();
        }

        private void ExecuteBlockFunctions()
        {
            foreach (var dataBlock in _modifiers)
            {
                if (dataBlock.InfoArgsBlock == null) continue;

                var function = dataBlock.InfoArgsBlock.GetFunctionByScope(EnumScope.BUILDING);
                if (function == EnumArgsBlockFunctions.NONE) continue;

                try
                {
                    if (function == EnumArgsBlockFunctions.MODIFIER_BUILDING_SET_IS_PORT)
                        isPort = (bool)dataBlock.Value;
                }
                catch (Exception ex)
                {
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_CANT_EXECUTE_BLOCK_FUNCTIONS,
                        new Dictionary<string, string>
                        {
                            { "{functionName}", function.ToString() },
                            { "{blockName}", dataBlock.GetName() },
                            { "{blockValue}", $"{dataBlock.Value}" }
                        }
                    ), ex);
                }
            }
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            /*
            var newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, _name);

            foreach (var key in BuildingManager.BUILDING_FORMATTER)
            {
                if (!FORMATTER.TryGetValue(key, out var parameter))
                    throw new Exception("Unknown formatter parameter " + key);

                parameter.PROVIDER(this).Save(sb, outTab, tab);
            }

            DataArgsBlocksManager.SaveDataArgsBlocks(sb, newOutTab, tab, TOKEN_DLC_ALLOWED, _dlcAllowed);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_SHOW_ON_MAP_COUNT, _showOnMapCount, DEFAULT_SHOW_ON_MAP_COUNT);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_SHOW_ON_MAP_MESHES, _showOnMapMeshes, DEFAULT_SHOW_ON_MAP_MESHES);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_SHOW_MODIFIER, _showModifier);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_ICON_FRAME, _iconFrame);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_BASE_COST, _baseCost);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_BASE_COST_CONVERTION, _baseCostConvertion);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_EXTRA_COST_PER_LEVEL, _extraCostPerLevel);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_EXTRA_COST_PER_CONTROLLER_BUILDING, _extraCostPerControllerBuilding);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_HAS_INFRASTRUCTUAL_CONTSRUCTION_EFFECT, _hasInfrastructureConstructionEffect);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_BASE_HEALTH, _baseHealth);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_BASE_HEALTH, _damageFactor);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_BASE_HEALTH, _repairSpeedFactor);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_ALLIED_BUILD, _isAlliedBuild);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_ONLY_COASTAL, _isOnlyCoastal);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_DISABLED_IN_DMZ, _isDisabledInDMZ);

            _buildingLevelCap?.Save(sb, outTab, tab);
            DataArgsBlocksManager.SaveDataArgsBlocks(sb, newOutTab, tab, TOKEN_STATE_MODIFIERS, _stateModifiers);

            ParadoxUtils.EndBlock(sb, outTab);
            
            */
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            /*
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == TOKEN_DLC_ALLOWED)
                    parser.AdvancedParse(
                        prevLayer, (_parser, _prevLayer, _token) => Logger.WrapTokenCallbackExceptions(
                            _token, () => DataArgsBlocksManager.ParseModifiers(_parser, _token, _dlcAllowed)
                        )
                    );
                else if (token == TOKEN_SHOW_ON_MAP_COUNT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _showOnMapCount, parser.ReadUInt16());
                else if (token == TOKEN_SHOW_ON_MAP_MESHES)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _showOnMapMeshes, parser.ReadUInt16());
                else if (token == TOKEN_SHOW_MODIFIER)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _showModifier, parser.ReadBool());
                else if (token == TOKEN_ICON_FRAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _iconFrame, parser.ReadUInt16());
                else if (token == TOKEN_BASE_COST)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _baseCost, parser.ReadUInt32());
                else if (token == TOKEN_BASE_COST_CONVERTION)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _baseCostConvertion, parser.ReadUInt32());
                else if (token == TOKEN_EXTRA_COST_PER_LEVEL)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _extraCostPerLevel, parser.ReadUInt32());
                else if (token == TOKEN_EXTRA_COST_PER_CONTROLLER_BUILDING)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _extraCostPerControllerBuilding, parser.ReadUInt32());
                else if (token == TOKEN_HAS_INFRASTRUCTUAL_CONTSRUCTION_EFFECT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _hasInfrastructureConstructionEffect, parser.ReadBool());
                else if (token == TOKEN_BASE_HEALTH)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _baseHealth, parser.ReadUInt32());
                else if (token == TOKEN_DAMAGE_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _damageFactor, parser.ReadFloat());
                else if (token == TOKEN_REPAIR_SPEED_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _repairSpeedFactor, parser.ReadFloat());
                else if (token == TOKEN_IS_ALLIED_BUILD)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isAlliedBuild, parser.ReadBool());
                else if (token == TOKEN_IS_ONLY_COASTAL)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isOnlyCoastal, parser.ReadBool());
                else if (token == TOKEN_IS_DISABLED_IN_DMZ)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isDisabledInDMZ, parser.ReadBool());
                else if (token == BuildingLevelCap.TOKEN_NAME)
                    Logger.ParseLayeredValue(prevLayer, token, ref _buildingLevelCap, parser, _buildingLevelCap);
                else if (token == TOKEN_STATE_MODIFIERS)
                    parser.AdvancedParse(
                        prevLayer, (_parser, _prevLayer, _token) => Logger.WrapTokenCallbackExceptions(
                            _token, () => DataArgsBlocksManager.ParseModifiers(_parser, _token, _stateModifiers)
                        )
                    );
                else
                    try { DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, _modifiers); }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_BUILDING_LOADING,
                            new Dictionary<string, string>
                            {
                                    { "{buildingName}", _name },
                                    { "{token}", token }
                            }
                        ), ex);
                    }
            });
            */
        }

        public bool Validate(LinkedLayer prevLayer) => true;

        public enum EnumBuildingSlotCategory
        {
            SHARED,
            NON_SHARED,
            PROVINCIAL
        }
    }

    public class BuildingLevelCap : IParadoxObject
    {
        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave;
        }

        public static readonly string TOKEN_NAME = "level_cap";

        private static readonly string TOKEN_PROVINCE_MAX_LEVEL = "province_max";
        private ushort? _provinceMaxLevel;
        public ushort? ProvinceMaxLevel
        {
            get => _provinceMaxLevel;
            set => Utils.Setter(ref _provinceMaxLevel, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_STATE_MAX_LEVEL = "state_max";
        private ushort? _stateMaxLevel;
        public ushort? StateMaxLevel
        {
            get => _stateMaxLevel;
            set => Utils.Setter(ref _stateMaxLevel, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_SHARED_SLOTS = "shared_slots";
        private static readonly bool DEFAULT_SHARED_SLOTS = false;
        private bool _sharedSlots;
        public bool SharedSlots
        {
            get => _sharedSlots;
            set => Utils.Setter(ref _sharedSlots, ref value, ref _needToSave);
        }

        private static readonly string TOKEN_GROUP_BY = "group_by";
        private string _groupBy;
        public string GroupBy
        {
            get => _groupBy;
            set => Utils.Setter(ref _groupBy, ref value, ref _needToSave);
        }


        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            var newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, TOKEN_NAME);

            ParadoxUtils.Save(sb, newOutTab, TOKEN_PROVINCE_MAX_LEVEL, _provinceMaxLevel);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_SHARED_SLOTS, _sharedSlots, DEFAULT_SHARED_SLOTS);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_STATE_MAX_LEVEL, _stateMaxLevel);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_GROUP_BY, _groupBy);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == TOKEN_PROVINCE_MAX_LEVEL)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _provinceMaxLevel, parser.ReadUInt16());
                else if (token == TOKEN_SHARED_SLOTS)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _sharedSlots, parser.ReadBool());
                else if (token == TOKEN_STATE_MAX_LEVEL)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _stateMaxLevel, parser.ReadUInt16());
                else if (token == TOKEN_GROUP_BY)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _groupBy, parser.ReadString());
                //else
                //    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    public class BuildingCountryModifiers : IParadoxObject
    {
        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave;
        }

        public static readonly string TOKEN_NAME = "country_modifiers";

        private static readonly string TOKEN_ENABLE_FOR_CONTROLLERS = "enable_for_controllers";
        private List<string> _enableForControllers;
        public List<string> EnableForControllers
        {
            get => _enableForControllers;
        }

        private static readonly string TOKEN_MODIFIERS = "modifiers";
        private List<DataArgsBlock> _modifiers = new List<DataArgsBlock>();

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            var newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, TOKEN_NAME);

            ParadoxUtils.Save(sb, newOutTab, TOKEN_ENABLE_FOR_CONTROLLERS, _enableForControllers);
            DataArgsBlocksManager.SaveDataArgsBlocks(sb, newOutTab, tab, TOKEN_MODIFIERS, _modifiers);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            /*
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == TOKEN_ENABLE_FOR_CONTROLLERS)
                {
                    if (_modifiers == nu)
                }
                else if (token == TOKEN_SHARED_SLOTS)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _sharedSlots, parser.ReadBool());
                else if (token == TOKEN_STATE_MAX_LEVEL)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _stateMaxLevel, parser.ReadUInt16());
                else if (token == TOKEN_GROUP_BY)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _groupBy, parser.ReadString());
                //else
                //    throw new UnknownTokenException(token);
            });
            */
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
