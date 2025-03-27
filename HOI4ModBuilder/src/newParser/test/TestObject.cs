using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.newParser.test
{
    public class TestObject : AbstractParseObject
    {
        public override IParseObject GetEmptyCopy() => new TestObject();

        public readonly GameParameter<int> IntValue = new GameParameter<int>();
        public readonly GameParameter<TestObject> InnerObject = new GameParameter<TestObject>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "intValue", o => ((TestObject)o).IntValue },
            { "innerObject", o => ((TestObject)o).InnerObject }
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;
    }
}
