using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.warnings;
using System.Windows.Forms;
using HOI4ModBuilder.src.forms.actionForms;

namespace HOI4ModBuilder.src
{
    public class SettingsManager
    {
        public static readonly string CONFIGS_DIRECTORY = FileManager.AssembleFolderPath(new[] { "configs" });
        public static readonly string SETTINGS_FILENAME = "settings.json";
        public static readonly string SETTINGS_FILEPATH = CONFIGS_DIRECTORY + SETTINGS_FILENAME;

        public static readonly string[] SUPPORTED_LANGUAGES = new string[] { "ru", "en" };

        public static Settings Settings { get; private set; }
        public static void Init()
        {
            try
            {
                if (!Directory.Exists(CONFIGS_DIRECTORY))
                    Directory.CreateDirectory(CONFIGS_DIRECTORY);

                if (!File.Exists(SETTINGS_FILEPATH))
                {
                    Settings = new Settings { language = GuiLocManager.GetCurrentParentLanguageName };
                    SaveSettings();
                }
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILEPATH));
                Settings.PostInit();
                GuiLocManager.Init(Settings);

                var needToSave = Settings.CheckNewWarningCodes();
                needToSave |= Settings.CheckNewErrorCodes();

                if (needToSave)
                    SaveSettings();
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
        public static bool CheckDebugValue(EnumDebugValue value)
        {
            if (Settings == null)
                return false;
            return Settings.CheckDebugValue(value);
        }

        public static void SaveSettings()
        {
            Settings.PostInit();
            File.WriteAllText(SETTINGS_FILEPATH, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            if (Settings.currentModSettings != null)
                LocalModDataManager.SaveLocalSettings(Settings);
        }

        public static void ChangeLanguage(string language)
        {
            if (Settings != null) Settings.language = language;
            SaveSettings();
            Init();
        }
    }

    public enum EnumDebugValue
    {
        TEXT_RENDER_CHUNKS,
        TEXT_DISABLE_DISTANCE_CUTOFF,
        TEXT_DISABLE_VIEWPORT_CUTOFF
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
        public int actionHistorySize = 50;
        public byte textureOpacity = 180;
        public float MAP_VIEWPORT_HEIGHT = 1004;
        public int maxAdditionalTextureSize = 16384;

        public bool ignoreUpdateChecks;
        public string ignoreUpdateCheckVersion;

        public MapCheckerInfo searchWarningsSettings = new MapCheckerInfo();
        public MapCheckerInfo searchErrorsSettings = new MapCheckerInfo();
        public ModSettings defaultModSettings = new ModSettings();
        public DebugSettings debugSettings;

        public void PostInit()
        {
            if (maxAdditionalTextureSize < 8192)
                maxAdditionalTextureSize = 8192;
        }

        public Dictionary<EnumCreateObjectType, CreateObjectPatterns> createObjectPatterns = new Dictionary<EnumCreateObjectType, CreateObjectPatterns>()
        {
            { EnumCreateObjectType.STATE, new CreateObjectPatterns(EnumCreateObjectType.STATE) },
            { EnumCreateObjectType.REGION, new CreateObjectPatterns(EnumCreateObjectType.REGION) },
        };

        public CreateObjectPatterns GetCreateObjectPattern(EnumCreateObjectType type)
        {
            if (!createObjectPatterns.TryGetValue(type, out CreateObjectPatterns pattern))
                createObjectPatterns[type] = pattern = new CreateObjectPatterns(type);

            return pattern;
        }


        public bool CheckDebugValue(EnumDebugValue value)
        {
            if (debugSettings == null)
                return false;
            return debugSettings.CheckDebugValue(value);
        }

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

        public bool IsWipEnabled(EnumWips enumWip)
            => useModSettings ? currentModSettings.CheckWips(enumWip) : defaultModSettings.CheckWips(enumWip);

        public bool IsUseCustomSavePatterns()
            => useModSettings ? currentModSettings.useCustomSavePatterns : defaultModSettings.useCustomSavePatterns;

        public bool CheckNewWarningCodes()
        {
            var newCodes = new List<string>();

            foreach (var errorCodes in Enum.GetValues(typeof(EnumMapWarningCode)))
            {
                var strValue = errorCodes.ToString();
                if (!searchWarningsSettings.known.Contains(strValue)) newCodes.Add(strValue);
            }

            if (newCodes.Count == 0) return false;

            var message = GuiLocManager.GetLoc(
                EnumLocKey.FOUNDED_NEW_WARNING_CODES,
                new Dictionary<string, string>
                {
                    { "{newWarningCodes}", string.Join("\n", newCodes) }
                }
            );
            Logger.ShowMessageOnUiThread(
                message,
                GuiLocManager.GetLoc(EnumLocKey.INFORMATION_MESSAGE_TITLE),
                MessageBoxIcon.Information
            );
            Logger.Log(message);
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

            var message = GuiLocManager.GetLoc(
                EnumLocKey.FOUNDED_NEW_ERROR_CODES,
                new Dictionary<string, string>
                {
                    { "{newErrorCodes}", string.Join("\n", newCodes) }
                }
            );
            Logger.ShowMessageOnUiThread(
                message,
                GuiLocManager.GetLoc(EnumLocKey.INFORMATION_MESSAGE_TITLE),
                MessageBoxIcon.Information
            );
            Logger.Log(message);
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
        public float GetNormalMapStrength()
            => useModSettings ? currentModSettings.normalMapStrength : defaultModSettings.normalMapStrength;
        public float GetNormalMapBlur()
            => useModSettings ? currentModSettings.normalMapBlur : defaultModSettings.normalMapBlur;
        public bool GetExportRiversMapWithWaterPixelsFlag()
            => useModSettings ? currentModSettings.exportRiversMapWithWaterPixels : defaultModSettings.exportRiversMapWithWaterPixels;
        public bool GetGenerateNormalMapFlag()
            => useModSettings ? currentModSettings.generateNormalMap : defaultModSettings.generateNormalMap;
        public float GetMapScalePixelToKM()
            => useModSettings ? currentModSettings.MAP_SCALE_PIXEL_TO_KM : defaultModSettings.MAP_SCALE_PIXEL_TO_KM;

        public float GetWaterHeight() => useModSettings ? currentModSettings.WATER_HEIGHT : defaultModSettings.WATER_HEIGHT;
        public float GetMinLandOffset() => useModSettings ? currentModSettings.WATER_HEIGHT_minLandOffset : defaultModSettings.WATER_HEIGHT_minLandOffset;
        public float GetMaxWaterOffset() => useModSettings ? currentModSettings.WATER_HEIGHT_maxWaterOffset : defaultModSettings.WATER_HEIGHT_maxWaterOffset;
        public List<string> GetWarningsFilter() => searchWarningsSettings?.enabled;
        public List<string> GetErrorsFilter() => searchErrorsSettings?.enabled;
    }

    public class CreateObjectPatterns
    {
        public string fileName;
        public string[] fileTextLines;

        public CreateObjectPatterns() { }

        public CreateObjectPatterns(EnumCreateObjectType type) : base()
        {
            InitDefault(type);
        }

        public void InitDefault(EnumCreateObjectType type)
        {
            switch (type)
            {
                case EnumCreateObjectType.STATE:
                    fileName = "{id}-State_{id}.txt";
                    fileTextLines = "state = { \n\tid = {id} \n\tname = \"STATE_{id}\" \n\tmanpower = 1 \n\tstate_category = wasteland \n\tprovinces = {} \n}".Split('\n');
                    break;
                case EnumCreateObjectType.REGION:
                    fileName = "{id}-Strategic_Region_{id}.txt";
                    fileTextLines = "strategic_region={ \n\tid={id} \n\tname=\"STRATEGICREGION_{id}\" \n\tprovinces={} \n}".Split('\n');
                    break;
            }
        }
    }

    public class SaveDataSettings
    {
        public List<string> saveFlags = new List<string>(0);
        public List<string> knownSaveFlags = new List<string>(0);
    }

    public class DebugSettings
    {
        public bool isEnabled;
        public DebugTextSettings text;

        public bool CheckDebugValue(EnumDebugValue value)
        {
            if (!isEnabled)
                return false;

            if (text != null && text.CheckDebugValue(value))
                return true;

            return false;
        }
    }

    public class DebugTextSettings
    {
        public bool renderChuncks;
        public bool disableDistanceCutoff;
        public bool disableViewportCutoff;

        public bool CheckDebugValue(EnumDebugValue value)
        {
            switch (value)
            {
                case EnumDebugValue.TEXT_RENDER_CHUNKS: return renderChuncks;
                case EnumDebugValue.TEXT_DISABLE_DISTANCE_CUTOFF: return disableDistanceCutoff;
                case EnumDebugValue.TEXT_DISABLE_VIEWPORT_CUTOFF: return disableViewportCutoff;
            }
            return false;
        }
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
        public bool useCustomSavePatterns = true;
        public bool exportRiversMapWithWaterPixels = true;
        public bool generateNormalMap = false;

        [JsonProperty("normalMapStrengthV2")]
        public float normalMapStrength = 25f; //recomended 20-25
        [JsonProperty("normalMapBlur")]
        public float normalMapBlur = 0.8f; //recommended 0.5-0.8

        public HashSet<string> wipsEnabled = new HashSet<string>(0);

        public float MAP_SCALE_PIXEL_TO_KM = 7.114f;
        public float WATER_HEIGHT = 9.5f;
        public float WATER_HEIGHT_minLandOffset = -0.1f;
        public float WATER_HEIGHT_maxWaterOffset = 0.1f;

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
