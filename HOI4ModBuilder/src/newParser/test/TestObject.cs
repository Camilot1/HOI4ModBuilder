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
        //public readonly GameParameter<GameList<int>> IntList = new GameParameter<GameList<int>>();
        public readonly GameList<int> IntList = new GameList<int>();
        public readonly GameParameter<TestObject> InnerObject = new GameParameter<TestObject>();


        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "intValue", o => ((TestObject)o).IntValue },
            { "intList", o => ((TestObject)o).IntList },
            { "innerObject", o => ((TestObject)o).InnerObject }
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

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(typeof(TestObject)).Add(STATIC_ADAPTER.Keys);
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;
        public override bool CustomSave(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key) => false;
    }
}
