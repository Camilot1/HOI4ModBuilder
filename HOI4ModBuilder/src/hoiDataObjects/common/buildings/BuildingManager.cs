using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    class BuildingManager : IParadoxRead
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

            foreach (var fileInfo in fileInfoPairs.Values)
            {
                _currentFile = fileInfo;
                using (var fs = new System.IO.FileStream(fileInfo.filePath, System.IO.FileMode.Open))
                    ParadoxParser.Parse(fs, Instance);
            }
        }

        public static Dictionary<string, Building>.KeyCollection GetBuildingNames()
        {
            return _allBuildings.Keys;
        }

        public static Dictionary<string, Building>.ValueCollection GetBuildings()
        {
            return _allBuildings.Values;
        }

        public static bool TryGetBuilding(string name, out Building building)
        {
            return _allBuildings.TryGetValue(name, out building);
        }

        public static bool HasBuilding(string name)
        {
            return _allBuildings.ContainsKey(name);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "buildings")
            {
                var buildings = new List<Building>();
                var list = new BuildingsList(buildings);
                parser.Parse(list);
                list.ExecuteAfterParse();
                _buildingsByFilesMap.Add(_currentFile, buildings);
            }
        }

        class BuildingsList : IParadoxRead
        {
            private List<Building> _buildings;
            public BuildingsList(List<Building> buildings)
            {
                _buildings = buildings;
            }

            public void TokenCallback(ParadoxParser parser, string token)
            {
                var building = new Building(token);
                parser.Parse(building);
                _buildings.Add(building);
                _allBuildings[token] = building;
            }

            public void ExecuteAfterParse()
            {
                foreach (var building in _buildings)
                    building.ExecuteAfterParse();
            }
        }
    }
}
