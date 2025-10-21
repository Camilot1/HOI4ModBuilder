using HOI4ModBuilder.src.forms.actionForms;
using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.settings.exceptions;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.managers.settings
{
    public class MapCheckerInfo
    {
        public List<string> enabled = new List<string>();
        public List<string> known = new List<string>();
    }

    public class BaseSettings
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


        [JsonIgnore]
        public ModSettings currentModSettings;

        [JsonIgnore]
        public Dictionary<string, ModDescriptor> modDescriptors = new Dictionary<string, ModDescriptor>();

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

        public void LoadModDescriptors()
        {
            modDescriptors = new Dictionary<string, ModDescriptor>();

            LoadModDescriptorCollection(unchangableModDirectories);
            LoadModDescriptorCollection(changableModDirectories);
            LoadModDescriptor(modDirectory);
        }

        private void LoadModDescriptorCollection(IEnumerable<string> directories)
        {
            if (directories == null)
                return;

            foreach (var directory in directories)
                LoadModDescriptor(directory);
        }

        private void LoadModDescriptor(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;

            var descriptorPath = Directory.EnumerateFiles(path, "*.mod", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (descriptorPath == null)
                throw new ModDescriptorFileNotFoundException(path);

            modDescriptors[path] = new ModDescriptor().Load(descriptorPath);
        }

        public bool IsModDirectorySelected()
        {
            var settings = SettingsManager.Settings;
            var directory = settings != null ? settings.modDirectory : modDirectory;

            return !string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory);
        }

        public bool IsWipEnabled(EnumWips enumWip)
            => useModSettings ? currentModSettings.CheckWips(enumWip) : defaultModSettings.CheckWips(enumWip);

        public bool IsUseCustomSavePatterns()
            => useModSettings ? currentModSettings.useCustomSavePatterns : defaultModSettings.useCustomSavePatterns;

        public bool CheckAndRegisterNewCodes()
        {
            var warningsUpdated = CheckNewWarningCodes();
            var errorsUpdated = CheckNewErrorCodes();

            return warningsUpdated || errorsUpdated;
        }

        public bool CheckNewWarningCodes()
            => RegisterNewCodes<EnumMapWarningCode>(searchWarningsSettings, EnumLocKey.FOUNDED_NEW_WARNING_CODES, "{newWarningCodes}");

        public bool CheckNewErrorCodes()
            => RegisterNewCodes<EnumMapErrorCode>(searchErrorsSettings, EnumLocKey.FOUNDED_NEW_ERROR_CODES, "{newErrorCodes}");

        [JsonIgnore]
        public static readonly string ModSettingsDirectory = ".hoi4modbuilder";
        [JsonIgnore]
        public static readonly string ModSettingsFile = "settings.json";

        public string GetModSettingsDirectoryPath()
            => Path.Combine(modDirectory ?? string.Empty, ModSettingsDirectory);
        public string GetModSettingsFilePath()
            => Path.Combine(GetModSettingsDirectoryPath(), ModSettingsFile);

        public bool IsModSettingsDirectoryExists()
            => Directory.Exists(GetModSettingsDirectoryPath());
        public bool IsModSettingsFileExists()
            => File.Exists(GetModSettingsFilePath());

        public ModSettings GetModSettings()
            => useModSettings ? currentModSettings : defaultModSettings;


        private bool RegisterNewCodes<TEnum>(MapCheckerInfo checkerInfo, EnumLocKey messageKey, string placeholder)
            where TEnum : struct, Enum
        {
            if (checkerInfo == null)
                return false;

            if (checkerInfo.known == null)
                checkerInfo.known = new List<string>();

            var newCodes = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(code => code.ToString())
                .Where(code => !checkerInfo.known.Contains(code))
                .ToList();

            if (newCodes.Count == 0)
                return false;

            var message = GuiLocManager.GetLoc(
                messageKey, new Dictionary<string, string> { { placeholder, string.Join("\n", newCodes) } }
            );

            Logger.ShowMessageOnUiThread(
                message,
                GuiLocManager.GetLoc(EnumLocKey.INFORMATION_MESSAGE_TITLE),
                MessageBoxIcon.Information
            );
            Logger.Log(message);

            checkerInfo.known.AddRange(newCodes);

            return true;
        }
    }
}
