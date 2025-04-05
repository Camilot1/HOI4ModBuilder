using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info.scripted
{
    public class ScriptedModifiersGameFile : GameFile
    {
        public readonly GameList<ScriptBlockParseObject> DynamicModifiers = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "dynamicModifiers", new DynamicGameParameter {
                provider = o => ((ScriptedModifiersGameFile)o).DynamicModifiers,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, new InfoArgsBlock(
                    key, EnumScope.MODIFIER, new EnumScope[] { EnumScope.SCOPE }, true, true
                    ))
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;
        public override IParseObject GetEmptyCopy() => new ScriptedModifiersGameFile();

        public override SaveAdapter GetSaveAdapter() => null;
        public ScriptedModifiersGameFile() { }
        public ScriptedModifiersGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public ScriptedModifiersGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
