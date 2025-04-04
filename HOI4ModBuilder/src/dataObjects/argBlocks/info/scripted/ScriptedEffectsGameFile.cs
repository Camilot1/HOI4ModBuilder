using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{

    public class ScriptedEffectsGameFile : GameFile
    {
        public readonly GameList<ScriptBlockParseObject> DynamicEffects = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "dynamicEffects", new DynamicGameParameter {
                provider = o => ((ScriptedEffectsGameFile)o).DynamicEffects,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, new InfoArgsBlock(
                    key, EnumScope.EFFECT, new EnumScope[] { EnumScope.SCOPE }, true, true
                    ))
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;
        public override IParseObject GetEmptyCopy() => new ScriptedEffectsGameFile();

        public override SaveAdapter GetSaveAdapter() => null;
        public ScriptedEffectsGameFile() { }
        public ScriptedEffectsGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public ScriptedEffectsGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
