using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.utils;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    class BuildingManager
    {
        public static BuildingManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "buildings" });
        private static List<BuildingsGameFile> _gameFiles = new List<BuildingsGameFile>();
        private static Dictionary<string, Building> _allBuildings = new Dictionary<string, Building>();
        public static Dictionary<string, Building> PARSER_AllBuildings => _allBuildings;
        private static Dictionary<string, SpawnPoint> _allSpawnPoints = new Dictionary<string, SpawnPoint>();
        public static Dictionary<string, SpawnPoint> PARSER_AllSpawnPoints => _allSpawnPoints;

        public static void Load(Settings settings)
        {
            Instance = new BuildingManager();
            _gameFiles = new List<BuildingsGameFile>();
            _allBuildings = new Dictionary<string, Building>();
            _allSpawnPoints = new Dictionary<string, SpawnPoint>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);
            var gameParser = new GameParser();

            foreach (var fileInfo in fileInfoPairs.Values)
            {
                Logger.TryOrCatch(() =>
                {
                    var buildingFile = new BuildingsGameFile(fileInfo, true);
                    gameParser.ParseFile(buildingFile);
                    _gameFiles.Add(buildingFile);
                },
                (ex) =>
                {
                    Logger.LogError(EnumLocKey.ERROR_COULD_NOT_LOAD_BUILDINGS_FILE, new Dictionary<string, string>
                    {
                        { "{filePath}", fileInfo.filePath },
                        { "{cause}", ex.Message },
                    });
                });
            }
        }

        public static Dictionary<string, Building>.KeyCollection GetBuildingNames() => _allBuildings.Keys;
        public static Dictionary<string, Building>.ValueCollection GetBuildings() => _allBuildings.Values;
        public static bool TryGetBuilding(string name, out Building building) => _allBuildings.TryGetValue(name, out building);
        public static bool HasBuilding(string name) => _allBuildings.ContainsKey(name);

        public bool Validate(LinkedLayer prevLayer) => true;

    }
}
