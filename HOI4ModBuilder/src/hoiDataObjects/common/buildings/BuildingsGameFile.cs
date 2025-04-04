using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

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
        public override SaveAdapter GetSaveAdapter() => new SaveAdapter(new[] { "common", "buildings" }, "BuildingsFile")
            .Add(STATIC_ADAPTER.Keys)
            .Load();

        public override void Validate(LinkedLayer layer)
        {
            CustomValidateSpawnPoints(layer);
            CustomValidateBuildings(layer);
            base.Validate(layer);
        }

        private void CustomValidateSpawnPoints(LinkedLayer layer)
        {
            var allSpawnPoints = BuildingManager.PARSER_AllSpawnPoints;
            foreach (var spawnPointEntry in SpawnPoints)
            {
                if (allSpawnPoints.ContainsKey(spawnPointEntry.Key))
                    Logger.LogLayeredWarning(layer, EnumLocKey.WARNING_OBJECT_PARAMETER_OVERRIDE, new Dictionary<string, string>
                    {
                        { "{object}", "spawn_points" },
                        { "{parameter}", "spawn_point name" },
                        { "{value}", $"{spawnPointEntry.Key}" },
                    });
                else
                    allSpawnPoints[spawnPointEntry.Key] = spawnPointEntry.Value;
            }
        }

        private void CustomValidateBuildings(LinkedLayer layer)
        {
            var allBuildings = BuildingManager.PARSER_AllBuildings;
            foreach (var buildingEntry in Buildings)
            {
                if (allBuildings.ContainsKey(buildingEntry.Key))
                    Logger.LogLayeredWarning(layer, EnumLocKey.WARNING_OBJECT_PARAMETER_OVERRIDE, new Dictionary<string, string>
                    {
                        { "{object}", "buildings" },
                        { "{parameter}", "building name" },
                        { "{value}", $"{buildingEntry.Key}" },
                    });
                else
                    allBuildings[buildingEntry.Key] = buildingEntry.Value;
            }
        }

        public BuildingsGameFile() { }
        public BuildingsGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public BuildingsGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
