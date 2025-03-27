using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.newParser.test
{
    public class TestGameFile : GameFile
    {
        public override IParseObject GetEmptyCopy() => new TestGameFile();

        public readonly GameParameter<TestObject> InnerObject = new GameParameter<TestObject>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "innerObject", o => ((TestGameFile)o).InnerObject }
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;

        public TestGameFile() { }
        public TestGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public TestGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
