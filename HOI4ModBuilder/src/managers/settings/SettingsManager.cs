using System.IO;
using Newtonsoft.Json;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.managers.settings.exceptions;
using HOI4ModBuilder.hoiDataObjects.map;

namespace HOI4ModBuilder.src
{
    public class SettingsManager
    {
        private static readonly string ConfigsDirectory = FileManager.AssembleFolderPath(new[] { "configs" });
        private const string SettingsFileName = "settings.json";
        private static string SettingsFilePath => Path.Combine(ConfigsDirectory, SettingsFileName);

        public static readonly string[] SUPPORTED_LANGUAGES = new[] { "ru", "en" };

        public static BaseSettings Settings { get; private set; }

        public static HSVRanges GetHSVRanges(EnumProvinceType type)
        {
            var hsvRanges = Settings?.GetModSettings()?.GetProvincesHSVRanges(type);
            return hsvRanges != null ? hsvRanges : new HSVRanges();
        }
        public static void Init()
        {
            Logger.TryOrCatch(
                () => LoadSettings(),
                (ex) => throw new SettingsFileLoadingException(SettingsFilePath, ex)
            );
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

        public static void LoadSettings()
        {
            EnsureConfigsDirectory();
            EnsureSettingsFileExists();

            Settings = LoadSettingsFromFile();
            Settings.PostInit();
            GuiLocManager.Init(Settings);

            if (Settings.CheckAndRegisterNewCodes())
                SaveSettings();
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
}
