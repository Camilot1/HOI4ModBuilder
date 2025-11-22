using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations
{
    public class StrategicLocationManager
    {
        public static StrategicLocationManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "strategic_locations" });
        private static List<BuildingsGameFile> _gameFiles = new List<BuildingsGameFile>();
        private static Dictionary<string, StrategicLocation> _allStrategicLocations = new Dictionary<string, StrategicLocation>();

        public static void Load(BaseSettings settings)
        {
            Instance = new StrategicLocationManager();

            _gameFiles = new List<BuildingsGameFile>();
            _allStrategicLocations = new Dictionary<string, StrategicLocation>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            foreach (var fileInfo in fileInfoPairs.Values)
                LoadFile(new GameParser(), new StrategicLocationGameFile(fileInfo));

        }

        public static Dictionary<string, StrategicLocation>.KeyCollection GetNames() => _allStrategicLocations.Keys;
        public static Dictionary<string, StrategicLocation>.ValueCollection GetValues() => _allStrategicLocations.Values;
        public static bool TryGet(string name, out StrategicLocation strategicLocation)
            => _allStrategicLocations.TryGetValue(name, out strategicLocation);
        public static StrategicLocation Get(string name)
        {
            if (_allStrategicLocations.TryGetValue(name, out var strategicLocation))
                return strategicLocation;
            return null;
        }

        public static bool Has(string name) => _allStrategicLocations.ContainsKey(name);
        public static void AddSilent(StrategicLocation strategicLocation)
            => _allStrategicLocations[strategicLocation.Name] = strategicLocation;

        private static void LoadFile(GameParser parser, StrategicLocationGameFile file)
        {
            try
            {
                parser.ParseFile(file);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsError(
                    EnumLocKey.ERROR_WHILE_STRATEGIC_LOCATIONS_LOADING,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", file.FilePath }
                    },
                    ex
                );
            }
        }
    }
}
