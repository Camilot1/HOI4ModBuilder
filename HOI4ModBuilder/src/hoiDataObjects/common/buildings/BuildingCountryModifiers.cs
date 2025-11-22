using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class BuildingCountryModifiers : AbstractParseObject
    {
        public readonly GameList<Country> EnableForControllers = new GameList<Country>()
            .INIT_ForceValueInline(true);
        public readonly GameList<ScriptBlockParseObject> Modifiers = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "enable_for_controllers", o => ((BuildingCountryModifiers)o).EnableForControllers },
            { "modifiers", o => ((BuildingCountryModifiers)o).Modifiers },
        };


        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "buildings" }, "BuildingCountryModifiers")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new BuildingCountryModifiers();
    }
}
