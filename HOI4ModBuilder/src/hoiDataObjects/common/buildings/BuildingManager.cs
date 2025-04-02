using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    class BuildingManager
    {
        public static BuildingManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "buildings" });
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, List<Building>> _buildingsByFilesMap = new Dictionary<FileInfo, List<Building>>();
        private static Dictionary<string, Building> _allBuildings = new Dictionary<string, Building>();

        public static void Load(Settings settings)
        {
            Instance = new BuildingManager();
            _buildingsByFilesMap = new Dictionary<FileInfo, List<Building>>();
            _allBuildings = new Dictionary<string, Building>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);
            var gameParser = new GameParser();

            foreach (var fileInfo in fileInfoPairs.Values)
            {
                _currentFile = fileInfo;

                var buildingFile = new BuildingsGameFile(fileInfo, true);
                gameParser.ParseFile(buildingFile);

                var a = 1;
            }
        }

        public static Dictionary<string, Building>.KeyCollection GetBuildingNames() => _allBuildings.Keys;
        public static Dictionary<string, Building>.ValueCollection GetBuildings() => _allBuildings.Values;
        public static bool TryGetBuilding(string name, out Building building) => _allBuildings.TryGetValue(name, out building);
        public static bool HasBuilding(string name) => _allBuildings.ContainsKey(name);

        public bool Validate(LinkedLayer prevLayer) => true;

    }
}
