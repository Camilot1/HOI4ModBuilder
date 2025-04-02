using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class BuildingsGameFile : GameFile
    {
        public override IParseObject GetEmptyCopy() => new BuildingsGameFile();

        public readonly GameDictionary<string, Building> Buildings = new GameDictionary<string, Building>()
            .INIT_SetValueParseAdapter((key, value) => new Building((string)key));
        public readonly GameDictionary<string, SpawnPoint> SpawnPoints = new GameDictionary<string, SpawnPoint>()
            .INIT_SetValueParseAdapter((key, value) => new SpawnPoint((string)key));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "buildings", (o) => ((BuildingsGameFile)o).Buildings },
            { "spawn_points", (o) => ((BuildingsGameFile)o).SpawnPoints },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;
        public override SaveAdapter GetSaveAdapter() => new SaveAdapter(new[] { "common", "buildings" }, "BuildingsFile")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override bool CustomParseCallback(GameParser parser) => false;

        public override bool CustomSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter) => false;

        public BuildingsGameFile() { }
        public BuildingsGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public BuildingsGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
