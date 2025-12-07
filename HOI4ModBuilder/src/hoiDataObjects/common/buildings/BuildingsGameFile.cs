using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class BuildingsGameFile : GameFile
    {
        public override IParseObject GetEmptyCopy() => new BuildingsGameFile();

        public readonly GameDictionary<string, Building> Buildings = new GameDictionary<string, Building>()
            .INIT_SetCheckForPreload(true)
            .INIT_SetValueParseAdapter((o, key, value) =>
            {
                if (((BuildingsGameFile)o.GetParent()).IsPostload)
                    return o[(string)key];
                else
                    return new Building((string)key);
            });
        public readonly GameDictionary<string, SpawnPoint> SpawnPoints = new GameDictionary<string, SpawnPoint>()
            .INIT_SetCheckForPreload(true)
            .INIT_SetValueParseAdapter((o, key, value) =>
            {
                if (((BuildingsGameFile)o.GetParent()).IsPostload)
                    return o[(string)key];
                else
                    return new SpawnPoint((string)key);
            });

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "buildings", (o) => ((BuildingsGameFile)o).Buildings },
            { "spawn_points", (o) => ((BuildingsGameFile)o).SpawnPoints },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "buildings" }, "BuildingsFile")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

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
                {
                    if (!IsPreload && !IsPostload)
                        Logger.LogLayeredWarning(layer, EnumLocKey.WARNING_OBJECT_PARAMETER_OVERRIDE, new Dictionary<string, string>
                        {
                            { "{object}", "spawn_points" },
                            { "{parameter}", "spawn_point name" },
                            { "{value}", $"{spawnPointEntry.Key}" },
                        });
                }
                else
                    allSpawnPoints[spawnPointEntry.Key] = spawnPointEntry.Value;
            }
        }

        private void CustomValidateBuildings(LinkedLayer layer)
        {
            foreach (var buildingEntry in Buildings)
            {
                if (BuildingManager.HasBuilding(buildingEntry.Key))
                {
                    if (!IsPreload && !IsPostload)
                        Logger.LogLayeredWarning(layer, EnumLocKey.WARNING_OBJECT_PARAMETER_OVERRIDE, new Dictionary<string, string>
                        {
                            { "{object}", "buildings" },
                            { "{parameter}", "building name" },
                            { "{value}", $"{buildingEntry.Key}" },
                        });
                }
                else
                    BuildingManager.AddBuildingSilent(buildingEntry.Value);
            }
        }

        public BuildingsGameFile() { }
        public BuildingsGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public BuildingsGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants)
        {}
    }
}
