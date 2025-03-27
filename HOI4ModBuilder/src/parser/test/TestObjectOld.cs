using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser.objects
{
    public class TestObjectOld : AbstractParseObjectOld
    {
        public override IParseObjectOld GetEmptyCopy() => new TestObjectOld();

        public readonly GameParameterOld<int> IntValue = new GameParameterOld<int>();
        public readonly GameParameterOld<InnerTestObjectOld> InnerObject = new GameParameterOld<InnerTestObjectOld>();
        public readonly GameListOld<InnerTestObjectOld> ListedObjects = new GameListOld<InnerTestObjectOld>();
        public readonly GameListOld<DataArgsBlock> DynamicListedModifiers = new GameListOld<DataArgsBlock>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "intValue", o => ((TestObjectOld)o).IntValue },
            { "innerObject", o => ((TestObjectOld)o).InnerObject },
            { "listedObjects", o => ((TestObjectOld)o).ListedObjects },
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        private static readonly Dictionary<string, DynamicGameParameterOld> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameterOld>
        { /*
            { "modifiers", new DynamicGameParameter(){
                parameterProvider =(o) => ((TestObject)o).DynamicListedModifiers,
                payloadFactory = (o, key) => InfoArgsBlocksManager.GetModifier(key)?.GetNewDataArgsBlockInstance()
            } } */
        };
        public override Dictionary<string, DynamicGameParameterOld> GetDynamicAdapter() => DYNAMIC_ADAPTER;

    }
}
