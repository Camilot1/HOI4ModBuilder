using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class SpawnPoint : AbstractParseObject
    {
        public string name;

        public readonly GameParameter<GameString> Type = new GameParameter<GameString>();
        public readonly GameParameter<uint> Max = new GameParameter<uint>();
        public readonly GameParameter<bool> OnlyCoastal = new GameParameter<bool>();
        public readonly GameParameter<bool> DisableAutoNudging = new GameParameter<bool>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "type", o => ((SpawnPoint)o).Type },
            { "max", o => ((SpawnPoint)o).Max },
            { "only_costal", o => ((SpawnPoint)o).OnlyCoastal },
            { "disable_auto_nudging", o => ((SpawnPoint)o).DisableAutoNudging },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "building" }, "SpawnPoint")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new SpawnPoint();

        public SpawnPoint() { }

        public SpawnPoint(string buildingName)
        {
            this.name = buildingName;
        }
    }
}
