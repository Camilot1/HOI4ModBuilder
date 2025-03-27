using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    class BuildingManager : IParadoxObject
    {
        public static BuildingManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "buildings" });
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, List<Building>> _buildingsByFilesMap = new Dictionary<FileInfo, List<Building>>();
        private static Dictionary<string, Building> _allBuildings = new Dictionary<string, Building>();

        public static readonly List<string> BUILDING_FORMATTER = new List<string>(Building.FORMATTER.Keys);

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
                    ParadoxParser.AdvancedParse(fs, new LinkedLayer(null, fileInfo.filePath, true), Instance, out bool validationResult);
            }
        }

        public static Dictionary<string, Building>.KeyCollection GetBuildingNames() => _allBuildings.Keys;
        public static Dictionary<string, Building>.ValueCollection GetBuildings() => _allBuildings.Values;
        public static bool TryGetBuilding(string name, out Building building) => _allBuildings.TryGetValue(name, out building);
        public static bool HasBuilding(string name) => _allBuildings.ContainsKey(name);

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new System.NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            if (token == "buildings")
            {
                var buildings = new List<Building>();
                var list = new BuildingsList(buildings);
                parser.AdvancedParse(new LinkedLayer(prevLayer, token), list, out bool _);
                list.ExecuteAfterParse();
                _buildingsByFilesMap.Add(_currentFile, buildings);
            }
        }

        public bool Validate(LinkedLayer prevLayer) => true;

        class BuildingsList : IParadoxObject
        {
            private List<Building> _buildings;
            public BuildingsList(List<Building> buildings)
            {
                _buildings = buildings;
            }

            public void ExecuteAfterParse()
            {
                foreach (var building in _buildings)
                    building.ExecuteAfterParse();
            }

            public bool Save(StringBuilder sb, string outTab, string tab)
            {
                throw new System.NotImplementedException();
            }

            public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
            {
                var building = new Building(token);
                parser.AdvancedParse(new LinkedLayer(prevLayer, token), building, out bool _);
                _buildings.Add(building);
                _allBuildings[token] = building;
            }

            public bool Validate(LinkedLayer prevLayer) => true;
        }
    }
}
