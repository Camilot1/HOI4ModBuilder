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

        public readonly GameList<ScriptBlockParseObject> ModifiersStatic = new GameList<ScriptBlockParseObject>()
            .INIT_SetValueParseAdapter((o, token) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(token)));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "modifiers", o => ((TestObject)o).ModifiersStatic },
        };

        public readonly GameList<ScriptBlockParseObject> Modifiers = new GameList<ScriptBlockParseObject>();
        public readonly GameList<ScriptBlockParseObject> Effects = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "modifiers", new DynamicGameParameter {
                provider = o => ((TestObject)o).Modifiers,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(key))
            } },
            { "effects", new DynamicGameParameter {
                provider = o => ((TestObject)o).Effects,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetEffect(key))
            } },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;
        public override bool CustomParseCallback(GameParser parser) => false;

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter("test", "TestObject")
            .Add(STATIC_ADAPTER.Keys)
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;
        public override bool CustomSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter) => false;
        public AbstractParseObject GetThis() => this;
    }
}
