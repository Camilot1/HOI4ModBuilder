using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

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

        public readonly GameList<ScriptBlockParseObject> DlcAllowed = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetTrigger(token)));
        public readonly GameParameter<ushort> ShowOnMapCount = new GameParameter<ushort>();
        public readonly GameParameter<ushort> ShowOnMapMeshes = new GameParameter<ushort>();
        public readonly GameParameter<bool> IsAlwaysShown = new GameParameter<bool>();
        public readonly GameParameter<bool> HasDestroyedMesh = new GameParameter<bool>();
        public readonly GameParameter<bool> IsCentered = new GameParameter<bool>();
        public readonly GameParameter<GameKeyObject<SpawnPoint>> SpawnPoint = new GameParameter<GameKeyObject<SpawnPoint>>()
            .INIT_SetValueParseAdapter((o, value) => new GameKeyObject<SpawnPoint> { key = value })
            .INIT_SetValueSaveAdapter((o) => o.key is GameConstant ? o.key : (o.value != null ? o.value.name : o.key));
        public readonly GameParameter<ushort> IconFrame = new GameParameter<ushort>();
        public readonly GameParameter<GameString> SpecialIcon = new GameParameter<GameString>();
        public readonly GameParameter<bool> IsBuildable = new GameParameter<bool>();

        //Cost
        public readonly GameParameter<uint> BaseCost = new GameParameter<uint>();
        public readonly GameParameter<uint> BaseCostConvertion = new GameParameter<uint>();
        public readonly GameParameter<uint> ExtraCostPerLevel = new GameParameter<uint>();
        public readonly GameParameter<uint> ExtraCostPerControllerBuilding = new GameParameter<uint>();
        public readonly GameParameter<bool> HasInfrastructureConstructionEffect = new GameParameter<bool>();

        public readonly GameParameter<BuildingLevelCap> LevelCap = new GameParameter<BuildingLevelCap>();

        public readonly GameParameter<float> BaseHealth = new GameParameter<float>();
        public readonly GameParameter<float> DamageFactor = new GameParameter<float>();
        public readonly GameParameter<float> RepairSpeedFactor = new GameParameter<float>();
        public readonly GameList<ScriptBlockParseObject> ProvinceDamageModifiers = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));
        public readonly GameList<ScriptBlockParseObject> StateDamageModifiers = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));
        public readonly GameParameter<bool> IsAlliedBuild = new GameParameter<bool>();
        public readonly GameParameter<bool> IsOnlyCoastal = new GameParameter<bool>();
        public readonly GameParameter<bool> IsPort = new GameParameter<bool>();
        public readonly GameParameter<bool> IsDisabledInDMZ = new GameParameter<bool>();
        public readonly GameParameter<bool> IsShowModifier = new GameParameter<bool>();
        public readonly GameList<ScriptBlockParseObject> StateModifiers = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));
        public readonly GameParameter<BuildingCountryModifiers> CountryModifiers = new GameParameter<BuildingCountryModifiers>();
        public readonly GameParameter<bool> IsNeedSupply = new GameParameter<bool>();
        public readonly GameParameter<bool> IsHideIfMissingTech = new GameParameter<bool>();
        public readonly GameParameter<GameString> MissingTechLoc = new GameParameter<GameString>();
        public readonly GameParameter<bool> IsNeedDetection = new GameParameter<bool>();
        public readonly GameParameter<GameString> DetectingIntelType = new GameParameter<GameString>();
        public readonly GameParameter<bool> IsOnlyDisplayIfExists = new GameParameter<bool>();
        public readonly GameList<GameString> Specialization = new GameList<GameString>();
        public readonly GameList<GameString> Tags = new GameList<GameString>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "dlc_allowed", o => ((Building)o).DlcAllowed },
            { "show_on_map", o => ((Building)o).ShowOnMapCount },
            { "show_on_map_meshes", o => ((Building)o).ShowOnMapMeshes },
            { "always_shown", o => ((Building)o).IsAlwaysShown },
            { "has_destroyed_mesh", o => ((Building)o).HasDestroyedMesh },
            { "centered", o => ((Building)o).IsCentered },
            { "spawn_point", o => ((Building)o).SpawnPoint },
            { "icon_frame", o => ((Building)o).IconFrame },
            { "special_icon", o => ((Building)o).SpecialIcon },
            { "is_buildable", o => ((Building)o).IsBuildable },

            { "base_cost", o => ((Building)o).BaseCost },
            { "base_cost_conversion", o => ((Building)o).BaseCostConvertion },
            { "per_level_extra_cost", o => ((Building)o).ExtraCostPerLevel },
            { "per_controlled_building_extra_cost", o => ((Building)o).ExtraCostPerControllerBuilding },
            { "infrastructure_construction_effect", o => ((Building)o).HasInfrastructureConstructionEffect },
            { "value", o => ((Building)o).BaseHealth },
            { "damage_factor", o => ((Building)o).DamageFactor },
            { "repair_speed_factor", o => ((Building)o).RepairSpeedFactor },
            { "province_damage_modifiers", o => ((Building)o).ProvinceDamageModifiers },
            { "state_damage_modifier", o => ((Building)o).StateDamageModifiers },
            { "allied_build", o => ((Building)o).IsAlliedBuild },
            { "only_costal", o => ((Building)o).IsOnlyCoastal },
            { "is_port", o => ((Building)o).IsPort },
            { "disabled_in_dmz", o => ((Building)o).IsDisabledInDMZ },
            { "level_cap", o => ((Building)o).LevelCap },
            { "show_modifier", o => ((Building)o).IsShowModifier },
            { "state_modifiers", o => ((Building)o).StateModifiers },
            { "country_modifiers", o => ((Building)o).CountryModifiers },
            { "need_supply", o => ((Building)o).IsNeedSupply },
            { "hide_if_missing_tech", o => ((Building)o).IsHideIfMissingTech },
            { "need_detection", o => ((Building)o).IsNeedDetection },
            { "detecting_intel_type", o => ((Building)o).DetectingIntelType },
            { "only_display_if_exists", o => ((Building)o).IsOnlyDisplayIfExists },
            { "specialization", o => ((Building)o).Specialization },
            { "tags", o => ((Building)o).Tags },
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

        public override void Validate(LinkedLayer layer)
        {
            var spawnPoint = SpawnPoint.GetValue();
            if (spawnPoint.key != null)
            {
                if (!(spawnPoint.key is string spawnPointKeyString))
                    Logger.LogLayeredError(layer, EnumLocKey.ERROR_OBJECT_PARAMETER_INVALID_VALUE, new Dictionary<string, string>
                    {
                        { "{object}", _name },
                        { "{parameter}", "spawn_point" },
                        { "{value}", $"{spawnPoint.key}" },
                    });
                else if (!BuildingManager.PARSER_AllSpawnPoints.TryGetValue(spawnPointKeyString, out spawnPoint.value))
                    Logger.LogLayeredWarning(layer, EnumLocKey.WARNING_OBJECT_PARAMETER_VALUE_NOT_FOUND, new Dictionary<string, string>
                    {
                        { "{object}", _name },
                        { "{parameter}", "spawn_point" },
                        { "{value}", $"{spawnPoint.key}" },
                    });
                else
                    SpawnPoint.SetSilentValue(spawnPoint);
            }

            if (LevelCap.GetValue() == null)
                LevelCap.SetValue(new BuildingLevelCap());

            base.Validate(layer);
        }

    }


}
