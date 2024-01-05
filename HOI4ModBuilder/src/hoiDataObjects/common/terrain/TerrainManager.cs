using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HOI4ModBuilder.hoiDataObjects.common.terrain
{
    class TerrainManager : IParadoxRead
    {
        public static TerrainManager Instance { get; private set; }
        private static src.FileInfo _currentFile;
        private static Dictionary<string, ProvincialTerrain> _provincialTerraings = new Dictionary<string, ProvincialTerrain>();

        public static void Load(Settings settings)
        {
            Instance = new TerrainManager();
            _provincialTerraings = new Dictionary<string, ProvincialTerrain>();
            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\terrain\");

            MainForm.Instance.InvokeAction(() => MainForm.Instance.ToolStripComboBox_Map_Province_Terrain.Items.Clear());

            foreach (var fileInfo in fileInfos.Values)
            {
                _currentFile = fileInfo;
                var fs = new FileStream(fileInfo.filePath, FileMode.Open);
                ParadoxParser.Parse(fs, Instance);
            }

            MainForm.Instance.InvokeAction(() =>
            {
                foreach (string terrain in _provincialTerraings.Keys)
                    MainForm.Instance.ToolStripComboBox_Map_Province_Terrain.Items.Add(terrain);
            });
        }

        public static bool TryGetProvincialTerrain(string tag, out ProvincialTerrain terrain)
        {
            return _provincialTerraings.TryGetValue(tag, out terrain);
        }

        public static bool HasProvincialTerrain(string tag)
        {
            return _provincialTerraings.ContainsKey(tag);
        }

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
