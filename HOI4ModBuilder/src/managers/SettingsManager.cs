using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src
{
    public class SettingsManager
    {
        public static readonly string CONFIGS_DIRECTORY = FileManager.AssembleFolderPath(new[] { "configs" });
        public static readonly string SETTINGS_FILENAME = "settings.json";
        public static readonly string SETTINGS_FILEPATH = CONFIGS_DIRECTORY + SETTINGS_FILENAME;

        public static Settings Settings { get; private set; }
        public static void Init()
        {
            try
            {
                if (!Directory.Exists(CONFIGS_DIRECTORY)) Directory.CreateDirectory(CONFIGS_DIRECTORY);
                if (!File.Exists(SETTINGS_FILEPATH))
                {
                    Settings = new Settings { language = GuiLocManager.GetCurrentParentLanguageName };
                    SaveSettings();
                }
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILEPATH));
                GuiLocManager.Init(Settings);
                Settings.LoadModDescriptors();

                var needToSave = Settings.CheckNewWarningCodes();
                needToSave |= Settings.CheckNewErrorCodes();

                if (needToSave) SaveSettings();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_WHILE_SETTINGS_FILE_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{settingsFilepath}", SETTINGS_FILEPATH },
                            { "{exceptionMessage}", ex.Message }
                        }
                    ),
                    ex
                 );
            }
        }

        public static void SaveSettings()
        {
            File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            if (Settings.currentModSettings != null) LocalModDataManager.SaveLocalSettings(Settings);
            Settings.LoadModDescriptors();
        }

        public static void ChangeLanguage(string language)
        {
            if (Settings != null) Settings.language = language;
            SaveSettings();
            Init();
        }
    }

    public class Settings
    {
        public string language;
        public string gameDirectory;
        public string gameTempDirectory;
        public string[] unchangableModDirectories = new string[0];
        public string[] changableModDirectories = new string[0];
        public string modDirectory;
        public bool useModSettings = true;
        public int actionHistorySize = 20;
        public byte textureOpacity = 180;
        public double MAP_VIEWPORT_HEIGHT = 1004;
        public int maxAdditionalTextureSize = 2048;

        public MapCheckerInfo searchWarningsSettings = new MapCheckerInfo();
        public MapCheckerInfo searchErrorsSettings = new MapCheckerInfo();
        public ModSettings defaultModSettings = new ModSettings();

        [JsonIgnore]
        public ModSettings currentModSettings;

        [JsonIgnore]
        public Dictionary<string, ModDescriptor> modDescriptors = new Dictionary<string, ModDescriptor>();

        public void LoadModDescriptors()
        {
            modDescriptors.Clear();
            modDescriptors = new Dictionary<string, ModDescriptor>();

            foreach (string path in unchangableModDirectories)
                LoadModDescriptor(path);
            foreach (string path in changableModDirectories)
                LoadModDescriptor(path);
            LoadModDescriptor(modDirectory);
        }

        private void LoadModDescriptor(string path)
        {
            if (path == null || path == "") return;
            string descriptorPath = null;
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".mod"))
                {
                    descriptorPath = file;
                    break;
                }
            }

            if (descriptorPath == null)
                throw new FileNotFoundException(
                    GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_MOD_DESCRIPTOR_FILE_NOT_FOUND,
                        new Dictionary<string, string> { { "{directoryPath}", path } }
                    )
                );

            if (File.Exists(descriptorPath)) modDescriptors[path] = new ModDescriptor().Load(descriptorPath);
        }

        public bool IsModDirectorySelected()
        {
            var settings = SettingsManager.Settings;
            if (settings.modDirectory == null || settings.modDirectory == "" || !Directory.Exists(settings.modDirectory)) return false;
            else return true;
        }

        public bool isWipEnabled(EnumWips enumWip)
            => useModSettings ? currentModSettings.CheckWips(enumWip) : defaultModSettings.CheckWips(enumWip);

        public bool CheckNewWarningCodes()
        {
            var newCodes = new List<string>();

            foreach (var errorCodes in Enum.GetValues(typeof(EnumMapWarningCode)))
            {
                var strValue = errorCodes.ToString();
                if (!searchWarningsSettings.known.Contains(strValue)) newCodes.Add(strValue);
            }

            if (newCodes.Count == 0) return false;

            Logger.LogSingleInfoMessage(
                EnumLocKey.FOUNDED_NEW_WARNING_CODES,
                new Dictionary<string, string>
                {
                    { "{newWarningCodes}", string.Join("\n", newCodes) }
                }
            );
            searchWarningsSettings.known.AddRange(newCodes);

            return true;
        }

        public bool CheckNewErrorCodes()
        {
            var newCodes = new List<string>();

            foreach (var errorCodes in Enum.GetValues(typeof(EnumMapErrorCode)))
            {
                var strValue = errorCodes.ToString();
                if (!searchErrorsSettings.known.Contains(strValue)) newCodes.Add(strValue);
            }

            if (newCodes.Count == 0) return false;

            Logger.LogSingleInfoMessage(
                EnumLocKey.FOUNDED_NEW_ERROR_CODES,
                new Dictionary<string, string>
                {
                    { "{newErrorCodes}", string.Join("\n", newCodes) }
                }
            );
            searchErrorsSettings.known.AddRange(newCodes);

            return true;
        }

        [JsonIgnore]
        public readonly string ModSettingsDirectory = ".hoi4modbuilder";
        [JsonIgnore]
        public readonly string ModSettingsFile = "settings.json";

        public string GetModSettingsDirectoryPath()
            => modDirectory + ModSettingsDirectory;
        public string GetModSettingsFilePath()
            => GetModSettingsDirectoryPath() + FileManager.PATH_SEPARATOR_STRING + ModSettingsFile;

        public bool IsModSettingsDirectoryExists() => Directory.Exists(GetModSettingsDirectoryPath());
        public bool IsModSettingsFileExists() => File.Exists(GetModSettingsFilePath());
        public double GetNormalMapStrength()
            => useModSettings ? currentModSettings.normalMapStrength : defaultModSettings.normalMapStrength;
        public bool GetExportRiversMapWithWaterPixelsFlag()
            => useModSettings ? currentModSettings.exportRiversMapWithWaterPixels : defaultModSettings.exportRiversMapWithWaterPixels;
        public bool GetGenerateNormalMapFlag()
            => useModSettings ? currentModSettings.generateNormalMap : defaultModSettings.generateNormalMap;
        public double GetMapScalePixelToKM()
            => useModSettings ? currentModSettings.MAP_SCALE_PIXEL_TO_KM : defaultModSettings.MAP_SCALE_PIXEL_TO_KM;

        public double GetWaterHeight() => useModSettings ? currentModSettings.WATER_HEIGHT : defaultModSettings.WATER_HEIGHT;
        public List<string> GetWarningsFilter() => searchWarningsSettings?.enabled;
        public List<string> GetErrorsFilter() => searchErrorsSettings?.enabled;
    }

    public class SaveDataSettings
    {
        public List<string> saveFlags = new List<string>(0);
        public List<string> knownSaveFlags = new List<string>(0);
    }

    public enum EnumSaveFlags
    {
        MAP_PROVINCES,
        MAP_RIVERS,
        MAP_TERRAIN,
        MAP_TREES,
        MAP_CITIES,
        MAP_HEIGHTS,
        MAP_NORMALS,

        PROVINCES_DEFINITION,
        ADJACENCIES,
        ADJACENCY_RULES,
        RAILWAYS,
        SUPPLY_HUBS,

        STATES,
        STRATEGIC_REGIONS,

        AI_AREAS,
        SUB_UNITS,

    }

    public class MapCheckerInfo
    {
        public List<string> enabled = new List<string>(0);
        public List<string> known = new List<string>(0);
    }

    public class ModSettings
    {
        public double normalMapStrength = 1.5d;
        public bool exportRiversMapWithWaterPixels = true;
        public bool generateNormalMap = false;

        public HashSet<string> wipsEnabled = new HashSet<string>(0);

        public double MAP_SCALE_PIXEL_TO_KM = 7.114;
        public double WATER_HEIGHT = 9.5f;

        public bool CheckWips(EnumWips enumWips) => wipsEnabled.Contains("" + enumWips);
        public void SetWips(EnumWips enumWips, bool value)
        {
            if (value)
                wipsEnabled.Add("" + enumWips);
            else
                wipsEnabled.Remove("" + enumWips);
        }
    }

    public enum EnumWips
    {
        SUB_UNITS,
        DIVISIONS_NAMES_GROUPS,
        OOBS,
        EQUIPMENTS
    }

    public class ModDescriptor : IParadoxRead
    {
        public string name;
        public HashSet<string> replacePathers = new HashSet<string>(0);
        public IList<string> tags = new List<string>(0);
        public string picture;
        public string supportedVersion;
        public string removeFileId;

        public ModDescriptor Load(string descriptorPath)
        {
            using (var fs = new FileStream(descriptorPath, FileMode.Open))
                ParadoxParser.Parse(fs, this);
            return this;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "name":
                    name = parser.ReadString();
                    break;
                case "replace_path":
                    replacePathers.Add(parser.ReadString().Replace('/', '\\') + '\\');
                    break;
                case "tags":
                    tags = parser.ReadStringList();
                    break;
                case "picture":
                    picture = parser.ReadString();
                    break;
                case "supported_version":
                    supportedVersion = parser.ReadString();
                    break;
                case "remote_file_id":
                    removeFileId = parser.ReadString();
                    break;
            }
        }
    }
}
