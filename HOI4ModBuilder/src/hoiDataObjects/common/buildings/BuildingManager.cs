using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    class BuildingManager : IParadoxRead
    {
        public static BuildingManager Instance { get; private set; }
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, List<Building>> _buildingsByFilesMap = new Dictionary<FileInfo, List<Building>>();
        private static Dictionary<string, Building> _allBuildings = new Dictionary<string, Building>();

        public static void Load(Settings settings)
        {
            Instance = new BuildingManager();
            _buildingsByFilesMap = new Dictionary<FileInfo, List<Building>>();
            _allBuildings = new Dictionary<string, Building>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\buildings\");

            foreach (var fileInfo in fileInfos.Values)
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
        }
    }
}
