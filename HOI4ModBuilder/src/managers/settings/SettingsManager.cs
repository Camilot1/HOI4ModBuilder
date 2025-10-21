using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.forms.actionForms;
using HOI4ModBuilder.src.managers.settings;

namespace HOI4ModBuilder.src
{
    public class SettingsManager
    {
        private static readonly string ConfigsDirectory = FileManager.AssembleFolderPath(new[] { "configs" });
        private const string SettingsFileName = "settings.json";
        private static string SettingsFilePath => Path.Combine(ConfigsDirectory, SettingsFileName);

        public static readonly string[] SUPPORTED_LANGUAGES = new[] { "ru", "en" };

        public static BaseSettings Settings { get; private set; }
        public static void Init()
        {
            Logger.TryOrCatch(() =>
            {
                EnsureConfigsDirectory();
                EnsureSettingsFileExists();

                Settings = LoadSettingsFromFile();
                Settings.PostInit();
                GuiLocManager.Init(Settings);

                if (Settings.CheckAndRegisterNewCodes())
                    SaveSettings();

            }, (ex) =>
            {
                throw new Exception(
                    GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_WHILE_SETTINGS_FILE_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{settingsFilepath}", SettingsFilePath },
                            { "{exceptionMessage}", ex.Message }
                        }
                    ),
                    ex
                );
            });
        }
        public static bool CheckDebugValue(EnumDebugValue value)
        {
            if (Settings == null)
                return false;
            return Settings.CheckDebugValue(value);
        }

        public static void SaveSettings()
        {
            if (Settings == null)
                return;

            Settings.PostInit();
            File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));

            if (Settings.currentModSettings != null)
                LocalModDataManager.SaveLocalSettings(Settings);
        }

        public static void ChangeLanguage(string language)
        {
            if (Settings != null)
                Settings.language = language;

            SaveSettings();
            Init();
        }

        private static void EnsureConfigsDirectory()
        {
            if (!Directory.Exists(ConfigsDirectory))
                Directory.CreateDirectory(ConfigsDirectory);
        }

        private static void EnsureSettingsFileExists()
        {
            if (File.Exists(SettingsFilePath))
                return;

            Settings = new BaseSettings { language = GuiLocManager.GetCurrentParentLanguageName };

            SaveSettings();
        }

        private static BaseSettings LoadSettingsFromFile()
        {
            var serialized = File.ReadAllText(SettingsFilePath);
            var settings = JsonConvert.DeserializeObject<BaseSettings>(serialized);

            return settings ?? new BaseSettings();
        }
    }

    public enum EnumDebugValue
    {
        TEXT_RENDER_CHUNKS,
        TEXT_DISABLE_DISTANCE_CUTOFF,
        TEXT_DISABLE_VIEWPORT_CUTOFF
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
        public List<string> saveFlags = new List<string>();
        public List<string> knownSaveFlags = new List<string>();
    }

    public class DebugSettings
    {
        public bool isEnabled;
        public DebugTextSettings text;

        public bool CheckDebugValue(EnumDebugValue value)
            => isEnabled && text != null && text.CheckDebugValue(value);
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


    public enum EnumWips
    {
        SUB_UNITS,
        DIVISIONS_NAMES_GROUPS,
        OOBS,
        EQUIPMENTS
    }

}
