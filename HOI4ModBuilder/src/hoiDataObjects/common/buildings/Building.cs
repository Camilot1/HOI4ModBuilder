using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class Building : AbstractParseObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        //TODO
        public EnumBuildingSlotCategory enumBuildingSlotCategory => EnumBuildingSlotCategory.SHARED;
        public uint maxLevel = 999;

        public readonly GameList<ScriptBlockParseObject> DlcAllowed = new GameList<ScriptBlockParseObject>();
        public readonly GameParameter<ushort> ShowOnMapCount = new GameParameter<ushort>();
        public readonly GameParameter<ushort> ShowOnMapMeshes = new GameParameter<ushort>();
        public readonly GameParameter<bool> ShowModifier = new GameParameter<bool>();
        public readonly GameParameter<ushort> IconFrame = new GameParameter<ushort>();
        public readonly GameParameter<uint> BaseCost = new GameParameter<uint>();
        public readonly GameParameter<uint> BaseCostConvertion = new GameParameter<uint>();
        public readonly GameParameter<uint> ExtraCostPerLevel = new GameParameter<uint>();
        public readonly GameParameter<uint> ExtraCostPerControllerBuilding = new GameParameter<uint>();
        public readonly GameParameter<bool> HasInfrastructureConstructionEffect = new GameParameter<bool>();
        public readonly GameParameter<uint> BaseHealth = new GameParameter<uint>();
        public readonly GameParameter<float> DamageFactor = new GameParameter<float>();
        public readonly GameParameter<float> RepairSpeedFactor = new GameParameter<float>();
        public readonly GameParameter<bool> IsAlliedBuild = new GameParameter<bool>();
        public readonly GameParameter<bool> IsOnlyCoastal = new GameParameter<bool>();
        public readonly GameParameter<bool> IsPort = new GameParameter<bool>();
        public readonly GameParameter<bool> IsDisabledInDMZ = new GameParameter<bool>();
        public readonly GameParameter<BuildingLevelCap> BuildingLevelCap = new GameParameter<BuildingLevelCap>();
        public readonly GameList<ScriptBlockParseObject> StateModifiers = new GameList<ScriptBlockParseObject>();
        public readonly GameParameter<BuildingCountryModifiers> CountryModifiers = new GameParameter<BuildingCountryModifiers>();


        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "dlc_allowed", o => ((Building)o).DlcAllowed },
            { "show_on_map", o => ((Building)o).ShowOnMapCount },
            { "show_on_map_meshes", o => ((Building)o).ShowOnMapMeshes },
            { "show_modifier", o => ((Building)o).ShowModifier },
            { "icon_frame", o => ((Building)o).IconFrame },
            { "base_cost", o => ((Building)o).BaseCost },
            { "base_cost_conversion", o => ((Building)o).BaseCostConvertion },
            { "per_level_extra_cost", o => ((Building)o).ExtraCostPerLevel },
            { "per_controlled_building_extra_cost", o => ((Building)o).ExtraCostPerControllerBuilding },
            { "infrastructure_construction_effect", o => ((Building)o).HasInfrastructureConstructionEffect },
            { "value", o => ((Building)o).BaseHealth },
            { "damage_factor", o => ((Building)o).DamageFactor },
            { "repair_speed_factor", o => ((Building)o).RepairSpeedFactor },
            { "allied_build", o => ((Building)o).IsAlliedBuild },
            { "only_costal", o => ((Building)o).IsOnlyCoastal },
            { "is_port", o => ((Building)o).IsPort },
            { "disabled_in_dmz", o => ((Building)o).IsDisabledInDMZ },
            { "level_cap", o => ((Building)o).BuildingLevelCap },
            { "state_modifiers", o => ((Building)o).StateModifiers },
            { "country_modifiers", o => ((Building)o).CountryModifiers },
        };

        public readonly GameList<ScriptBlockParseObject> Modifiers = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "modifiers", new DynamicGameParameter {
                provider = o => ((Building)o).Modifiers,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(key))
            } },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        public override bool CustomParseCallback(GameParser parser) => false;

        public override bool CustomSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter) => false;

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "common", "buildings" }, "Building")
            .Add(STATIC_ADAPTER.Keys)
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override IParseObject GetEmptyCopy() => new Building();


        public Building() { }
        public Building(string name)
        {
            _name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Building building &&
                   _name == building._name;
        }

        public enum EnumBuildingSlotCategory
        {
            SHARED,
            NON_SHARED,
            PROVINCIAL
        }
    }

    public class BuildingLevelCap : AbstractParseObject
    {
        public readonly GameParameter<bool> SharedSlots = new GameParameter<bool>();
        public readonly GameParameter<uint> StateMax = new GameParameter<uint>();
        public readonly GameParameter<uint> ProvinceMax = new GameParameter<uint>();
        public readonly GameParameter<GameString> GroupBy = new GameParameter<GameString>();
        public readonly GameParameter<GameString> ExclusiveWith = new GameParameter<GameString>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "shares_slots", o => ((BuildingLevelCap)o).SharedSlots },
            { "state_max", o => ((BuildingLevelCap)o).StateMax },
            { "province_max", o => ((BuildingLevelCap)o).ProvinceMax },
            { "group_by", o => ((BuildingLevelCap)o).GroupBy },
            { "exclusive_with", o => ((BuildingLevelCap)o).ExclusiveWith },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;


        public override bool CustomParseCallback(GameParser parser) => false;

        public override bool CustomSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter) => false;

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "common", "building" }, "BuildingLevelCap")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override IParseObject GetEmptyCopy() => new BuildingLevelCap();
    }

    public class BuildingCountryModifiers : AbstractParseObject
    {
        public readonly GameList<Country> EnableForControllers = new GameList<Country>()
            .INIT_SetValueParseAdapter((o, token) => CountryManager.GetCountry(token))
            .INIT_SetValueSaveAdapter((country) => country.Tag);
        public readonly GameList<ScriptBlockParseObject> Modifiers = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "enable_for_controllers", o => ((BuildingCountryModifiers)o).EnableForControllers },
            { "modifiers", o => ((BuildingCountryModifiers)o).Modifiers },
        };


        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;

        public override bool CustomParseCallback(GameParser parser) => false;

        public override bool CustomSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter) => false;

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "common", "buildings" }, "BuildingCountryModifiers")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override IParseObject GetEmptyCopy() => new BuildingCountryModifiers();

    }
}
