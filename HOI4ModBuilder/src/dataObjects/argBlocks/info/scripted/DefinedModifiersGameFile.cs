using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{
    public class DefinedModifiersGameFile : GameFile
    {
        public readonly GameList<DefinedModifier> DynamicModifiers = new GameList<DefinedModifier>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "dynamicModifiers", new DynamicGameParameter {
                parseInnerBlock = true,
                provider = o => ((DefinedModifiersGameFile)o).DynamicModifiers,
                factory = (o, key) => new DefinedModifier(key)
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;
        public override IParseObject GetEmptyCopy() => new DefinedModifiersGameFile();

        public override SavePattern GetSavePattern() => null;
        public DefinedModifiersGameFile() { }
        public DefinedModifiersGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public DefinedModifiersGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }

    public class DefinedModifier : AbstractParseObject
    {
        public string name;
        public readonly GameParameter<GameString> ColorType = new GameParameter<GameString>();
        public readonly GameParameter<GameString> ValueType = new GameParameter<GameString>();
        public readonly GameParameter<byte> Precision = new GameParameter<byte>();
        public readonly GameParameter<GameString> Postfix = new GameParameter<GameString>();
        public readonly GameList<GameString> Categories = new GameList<GameString>(true, true);

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "color_type", o => ((DefinedModifier)o).ColorType },
            { "value_type", o => ((DefinedModifier)o).ValueType },
            { "precision", o => ((DefinedModifier)o).Precision },
            { "postfix", o => ((DefinedModifier)o).Postfix },
            { "category", o => ((DefinedModifier)o).Categories },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        public override IParseObject GetEmptyCopy() => new DefinedModifier();
        public override SavePattern GetSavePattern() => null;

        public DefinedModifier() { }
        public DefinedModifier(string name)
        {
            this.name = name;
        }
    }
}
