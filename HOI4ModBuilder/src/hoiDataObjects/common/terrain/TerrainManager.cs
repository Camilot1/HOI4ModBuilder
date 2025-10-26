using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.hoiDataObjects.common.terrain
{
    class TerrainManager : IParadoxRead
    {
        public static TerrainManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "terrain" });
        private static src.FileInfo _currentFile;
        private static Dictionary<string, ProvincialTerrain> _provincialTerraings = new Dictionary<string, ProvincialTerrain>();
        public static Dictionary<string, ProvincialTerrain>.KeyCollection GetAllTerrainKeys() => _provincialTerraings.Keys;

        private static Action _guiReinitAction = null;

        public static int GetNavalTerrainsCount()
        {
            int counter = 0;
            foreach (var terrain in _provincialTerraings.Values)
                if (terrain.isNavalTerrain)
                    counter++;
            return counter;
        }
        public static int GetLandTerrainsCount()
        {
            int counter = 0;
            foreach (var terrain in _provincialTerraings.Values)
                if (!terrain.isNavalTerrain)
                    counter++;
            return counter;
        }

        public static void Load(BaseSettings settings)
        {
            Instance = new TerrainManager();
            _provincialTerraings = new Dictionary<string, ProvincialTerrain>();
            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            MainForm.Instance.InvokeAction(() => MainForm.Instance.ToolStripComboBox_Map_Province_Terrain.Items.Clear());

            foreach (var fileInfo in fileInfoPairs.Values)
            {
                _currentFile = fileInfo;
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, Instance);
            }

            if (_guiReinitAction == null)
            {
                _guiReinitAction = () =>
                {
                    foreach (var terrainName in GetAllTerrainKeys())
                        MainForm.Instance.ToolStripComboBox_Map_Province_Terrain.Items.Add(terrainName);
                };
                MainForm.SubscribeGuiReinitAction(_guiReinitAction);
            }

            MainForm.Instance.InvokeAction(() => _guiReinitAction());
        }

        public static bool TryGetProvincialTerrain(string tag, out ProvincialTerrain terrain)
            => _provincialTerraings.TryGetValue(tag, out terrain);

        public static bool HasProvincialTerrain(string tag)
            => _provincialTerraings.ContainsKey(tag);

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "categories":
                    var provincialTerrains = new ProvincialTerrains(_provincialTerraings);
                    parser.Parse(provincialTerrains);
                    break;
                case "terrain": break;
            }
        }
    }

    class ProvincialTerrains : IParadoxRead
    {
        private Dictionary<string, ProvincialTerrain> _map = new Dictionary<string, ProvincialTerrain>();

        public ProvincialTerrains(Dictionary<string, ProvincialTerrain> map)
        {
            _map = map;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            string name = token;
            var provincialTerrain = new ProvincialTerrain(name);
            parser.Parse(provincialTerrain);
            _map[name] = provincialTerrain;
        }
    }

    class GraphicalTerrains : IParadoxRead
    {
        public void TokenCallback(ParadoxParser parser, string token)
        {
            throw new NotImplementedException();
        }
    }
}
