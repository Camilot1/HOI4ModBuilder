using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
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
        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "common", "buildings" }, "BuildingCountryModifiers")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override IParseObject GetEmptyCopy() => new BuildingCountryModifiers();
    }
}
