using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.newParser.test
{
    public class TestObject : AbstractParseObject
    {
        public override IParseObject GetEmptyCopy() => new TestObject();

        public readonly GameParameter<int> IntValue = new GameParameter<int>();
        public readonly GameList<ScriptBlockParseObject> ModifiersStatic = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));
        public readonly GameList<ScriptBlockParseObject> EffectsStatic = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetEffect(token)));

        public readonly GameDictionary<GameString, int> MapA = new GameDictionary<GameString, int>();
        public readonly GameDictionary<GameString, TestObject> MapB = new GameDictionary<GameString, TestObject>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "intValue", o => ((TestObject)o).IntValue },
            { "mapA", o => ((TestObject)o).MapA },
            { "mapB", o => ((TestObject)o).MapB },
            { "modifiers", o => ((TestObject)o).ModifiersStatic },
            { "effects", o => ((TestObject)o).EffectsStatic },
        };

        public readonly GameList<ScriptBlockParseObject> Modifiers = new GameList<ScriptBlockParseObject>();
        public readonly GameList<ScriptBlockParseObject> Effects = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "dynamicModifiers", new DynamicGameParameter {
                provider = o => ((TestObject)o).Modifiers,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(key))
            } },
            { "dynamicEffects", new DynamicGameParameter {
                provider = o => ((TestObject)o).Effects,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetEffect(key))
            } },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern("test", "TestObject")
            .Add(STATIC_ADAPTER.Keys)
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;
    }
}
