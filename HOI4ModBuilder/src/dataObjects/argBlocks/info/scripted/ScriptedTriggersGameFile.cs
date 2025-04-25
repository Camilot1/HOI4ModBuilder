using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{
    public class ScriptedTriggersGameFile : GameFile
    {
        public readonly GameList<ScriptBlockParseObject> DynamicTriggers = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "dynamicTriggers", new DynamicGameParameter {
                provider = o => ((ScriptedTriggersGameFile)o).DynamicTriggers,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, new InfoArgsBlock(
                    key, EnumScope.TRIGGER, new EnumScope[] { EnumScope.SCOPE }, true, true
                    ))
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;
        public override IParseObject GetEmptyCopy() => new ScriptedTriggersGameFile();

        public override SavePattern GetSavePattern() => null;
        public ScriptedTriggersGameFile() { }
        public ScriptedTriggersGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public ScriptedTriggersGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
