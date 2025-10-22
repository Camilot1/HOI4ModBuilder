using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms
{
    public partial class SettingsForm : Form
    {

        public static SettingsForm instance;
        public static bool isLoading = false;

        private ModSettingsControlSet defaultModControls;
        private ModSettingsControlSet currentModControls;

        public SettingsForm()
        {
            InitializeComponent();
            InitializeModControlSets();
            instance?.Close();
            instance = this;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadData();
            GuiLocManager.formsReinitEvents.Add(this, () =>
            {
                Controls.Clear();
                InitializeComponent();
                InitializeModControlSets();
                LoadData();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
        {
            isLoading = true;

            var settings = SettingsManager.Settings;

            switch (settings.language)
            {
                case "ru": ComboBox_Language.SelectedIndex = 0; break;
                case "en": ComboBox_Language.SelectedIndex = 1; break;
                default: ComboBox_Language.SelectedIndex = 1; break;
            }

            TextBox_GameDirectory.Text = settings.gameDirectory;
            TextBox_GameTempDirectory.Text = settings.gameTempDirectory;
            TextBox_ModDirectory.Text = settings.modDirectory;

            ComboBox_UsingSettingsType.SelectedIndex = settings.useModSettings ? 1 : 0;

            Button_CreateModSettings.Enabled = settings.currentModSettings == null && !settings.IsModSettingsFileExists();

            TextBox_ActionHistorySize.Text = "" + settings.actionHistorySize;
            TextBox_Textures_Opacity.Text = "" + Math.Round(settings.textureOpacity / 255d * 100, 2);
            TextBox_MAP_VIEWPORT_HEIGHT.Text = "" + settings.MAP_VIEWPORT_HEIGHT;
            ComboBox_MaxAdditionalTextureSize.Text = "" + settings.maxAdditionalTextureSize;

            TextBox_IgnoreUpdateVersion.Text = settings.ignoreUpdateCheckVersion;
            CheckBox_IgnoreUpdateVersion.Checked = settings.ignoreUpdateChecks;

            ApplyModSettingsToControls(settings.defaultModSettings, defaultModControls);
            ApplyModSettingsToControls(settings.currentModSettings, currentModControls);

            SavePattern.LoadAll();

            isLoading = false;
        }

        private void Button_GameDirectory_Click(object sender, EventArgs e)
        {
            var thread = new Thread(() =>
            {
                var DirDialog = new FolderBrowserDialog
                {
                    Description = GuiLocManager.GetLoc(EnumLocKey.FOLDER_BROWSER_DIALOG_CHOOSE_GAME_DIRECTORY_TITLE),
                    SelectedPath = TextBox_GameDirectory.Text
                };

                if (DirDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = FileManager.AssembleFolderPath(new[] { DirDialog.SelectedPath });

                    if (!IsValidGameDirectory(path))
                    {
                        Logger.LogSingleErrorMessage(EnumLocKey.SINGLE_MESSAGE_NO_HOI4EXE_FILE_IN_GAME_DIRECTORY, new Dictionary<string, string> { { "{fileName}", "hoi4.exe" } });
                        return;
                    }
                    Invoke(new Action(() => TextBox_GameDirectory.Text = path));
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        public bool IsValidGameDirectory(string path)
            => File.Exists(path + "hoi4.exe");

        private void Button_GameTempDirectory_Click(object sender, EventArgs e)
        {
            var thread = new Thread(() =>
            {
                var DirDialog = new FolderBrowserDialog
                {
                    Description = GuiLocManager.GetLoc(EnumLocKey.FOLDER_BROWSER_DIALOG_CHOOSE_DIRECTORY_IN_DOCUMENTS_TITLE),
                    SelectedPath = TextBox_GameTempDirectory.Text
                };

                if (DirDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = FileManager.AssembleFolderPath(new[] { DirDialog.SelectedPath });

                    if (!IsValidGameTempDirectory(path))
                    {
                        Logger.LogSingleErrorMessage(EnumLocKey.SINGLE_MESSAGE_NO_SAVEGAMES_FOULDER_IN_DIRECTORY_IN_DOCUMENTS, new Dictionary<string, string> { { "{directoryName}", "save games" } });
                        return;
                    }
                    Invoke(new Action(() => TextBox_GameTempDirectory.Text = path));
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public bool IsValidGameTempDirectory(string path)
            => Directory.Exists(path + "save games");

        private void Button_ModDirectory_Click(object sender, EventArgs e)
        {
            var thread = new Thread(() =>
            {
                var dialogPath = !string.IsNullOrEmpty(TextBox_ModDirectory.Text) ?
                        TextBox_ModDirectory.Text :
                        FileManager.AssembleFolderPath(new[] { FileManager.GetDocumentsFolderPath(), "Paradox Interactive", "Hearts of Iron IV", "mod" });
                var DirDialog = new FolderBrowserDialog
                {
                    Description = GuiLocManager.GetLoc(EnumLocKey.FOLDER_BROWSER_DIALOG_CHOOSE_DIRECTORY_OF_MOD_TITLE),
                    SelectedPath = dialogPath
                };

                if (DirDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = FileManager.AssembleFolderPath(new[] { DirDialog.SelectedPath });

                    string descriptorPath = null;
                    int descriptorCount = 0;
                    foreach (string file in Directory.GetFiles(path))
                    {
                        string[] parts = file.Split('.');
                        if (parts[parts.Length - 1] == "mod")
                        {
                            if (descriptorCount > 0)
                            {
                                Logger.LogSingleErrorMessage(EnumLocKey.SINGLE_MESSAGE_MULTIPLE_MOD_DESCRIPTOR_IN_DIRECTORY_OF_MOD);
                                return;
                            }
                            descriptorPath = file;
                            descriptorCount++;
                        }
                    }

                    if (descriptorPath == null)
                    {
                        Logger.LogSingleErrorMessage(EnumLocKey.SINGLE_MESSAGE_NO_MOD_DESCRIPTOR_IN_DIRECTORY_OF_MOD);
                        return;
                    }

                    Invoke(new Action(() => TextBox_ModDirectory.Text = path));
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var settings = SettingsManager.Settings;
                var prevLang = settings.language;

                switch (ComboBox_Language.SelectedIndex)
                {
                    case 0: settings.language = "ru"; break;
                    case 1: settings.language = "en"; break;
                }

                settings.gameDirectory = TextBox_GameDirectory.Text;
                if (PathCheck(settings.gameDirectory))
                    settings.gameDirectory += '\\';

                settings.gameTempDirectory = TextBox_GameTempDirectory.Text;
                if (PathCheck(settings.gameTempDirectory))
                    settings.gameTempDirectory += '\\';

                var tempModDirectory = TextBox_ModDirectory.Text;
                if (PathCheck(tempModDirectory))
                    tempModDirectory += '\\';

                settings.actionHistorySize = int.Parse(TextBox_ActionHistorySize.Text);

                double textureOpacity = Utils.ParseFloat(TextBox_Textures_Opacity.Text) / 100d;
                if (textureOpacity < 0)
                    settings.textureOpacity = 0;
                else if (textureOpacity > 1)
                    settings.textureOpacity = 255;
                else
                    settings.textureOpacity = (byte)Math.Round(textureOpacity * 255);

                settings.ignoreUpdateCheckVersion = TextBox_IgnoreUpdateVersion.Text;
                settings.ignoreUpdateChecks = CheckBox_IgnoreUpdateVersion.Checked;

                settings.MAP_VIEWPORT_HEIGHT = Utils.ParseFloat(TextBox_MAP_VIEWPORT_HEIGHT.Text);
                settings.maxAdditionalTextureSize = int.Parse(ComboBox_MaxAdditionalTextureSize.Text);

                UpdateModSettingsFromControls(settings.defaultModSettings, defaultModControls);
                UpdateModSettingsFromControls(settings.currentModSettings, currentModControls);

                SettingsManager.SaveSettings();

                if (settings.modDirectory != tempModDirectory)
                {
                    settings.modDirectory = tempModDirectory;
                    LocalModDataManager.Load(settings);
                    SettingsManager.SaveSettings();
                }

                if (prevLang != settings.language)
                    GuiLocManager.SetCurrentUICulture(settings.language);
                else
                    LoadData();

            });

            bool PathCheck(string path)
            {
                if (path == null || path.Length == 0)
                    return false;

                return path[path.Length - 1] != '\\';
            }
        }

        private void InitializeModControlSets()
        {
            defaultModControls = CreateDefaultModControls();
            currentModControls = CreateCurrentModControls();
        }

        private ModSettingsControlSet CreateDefaultModControls()
            => new ModSettingsControlSet(
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default,
                TextBox_WATER_HEIGHT_Default,
                TextBox_WATER_HEIGHT_min_land_offset_Default,
                TextBox_WATER_HEIGHT_max_water_offset_Default,
                TextBox_NormalMapStrength_Default,
                TextBox_NormalMapBlur_Default,
                CheckedListBox_SaveSettings_Default,
                CheckedListBox_Wips_Default,
                CheckBox_UseCustomSavePatterns_Default
            );

        private ModSettingsControlSet CreateCurrentModControls()
            => new ModSettingsControlSet(
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current,
                TextBox_WATER_HEIGHT_Current,
                TextBox_WATER_HEIGHT_min_land_offset_Current,
                TextBox_WATER_HEIGHT_max_water_offset_Current,
                TextBox_NormalMapStrength_Current,
                TextBox_NormalMapBlur_Current,
                CheckedListBox_SaveSettings_Current,
                CheckedListBox_Wips_Current,
                CheckBox_UseCustomSavePatterns_Current
            );

        private void ApplyModSettingsToControls(ModSettings modSettings, ModSettingsControlSet controls)
        {
            if (modSettings == null)
            {
                ClearModSettingsControls(controls);
                SetModSettingsControlsEnabled(controls, false);
                return;
            }

            controls.MapScale.Text = "" + modSettings.MAP_SCALE_PIXEL_TO_KM;
            controls.WaterHeight.Text = "" + modSettings.WATER_HEIGHT;
            controls.WaterHeightMinOffset.Text = "" + modSettings.WATER_HEIGHT_minLandOffset;
            controls.WaterHeightMaxOffset.Text = "" + modSettings.WATER_HEIGHT_maxWaterOffset;
            controls.NormalMapStrength.Text = "" + modSettings.normalMapStrength;
            controls.NormalMapBlur.Text = "" + modSettings.normalMapBlur;
            controls.UseCustomSavePatterns.Checked = modSettings.useCustomSavePatterns;

            SetSaveSettingsItem(controls.SaveSettings, 0, modSettings.exportRiversMapWithWaterPixels);
            SetSaveSettingsItem(controls.SaveSettings, 1, modSettings.generateNormalMap);
            ApplyWipChecklist(controls.Wips, modSettings);

            SetModSettingsControlsEnabled(controls, true);
        }

        private void UpdateModSettingsFromControls(ModSettings target, ModSettingsControlSet controls)
        {
            if (target == null)
                return;

            target.MAP_SCALE_PIXEL_TO_KM = Utils.ParseFloat(controls.MapScale.Text);
            target.WATER_HEIGHT = Utils.ParseFloat(controls.WaterHeight.Text);
            target.WATER_HEIGHT_minLandOffset = Utils.ParseFloat(controls.WaterHeightMinOffset.Text);
            target.WATER_HEIGHT_maxWaterOffset = Utils.ParseFloat(controls.WaterHeightMaxOffset.Text);
            target.normalMapStrength = Utils.ParseFloat(controls.NormalMapStrength.Text);
            target.normalMapBlur = Utils.ParseFloat(controls.NormalMapBlur.Text);

            target.useCustomSavePatterns = controls.UseCustomSavePatterns.Checked;
            if (controls.SaveSettings.Items.Count > 0)
                target.exportRiversMapWithWaterPixels = controls.SaveSettings.GetItemChecked(0);
            if (controls.SaveSettings.Items.Count > 1)
                target.generateNormalMap = controls.SaveSettings.GetItemChecked(1);

            foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
            {
                int index = (int)enumObj;
                if (index >= controls.Wips.Items.Count)
                    continue;

                target.SetWips(enumObj, controls.Wips.GetItemChecked(index));
            }
        }

        private static void ClearModSettingsControls(ModSettingsControlSet controls)
        {
            controls.MapScale.Text = "";
            controls.WaterHeight.Text = "";
            controls.WaterHeightMinOffset.Text = "";
            controls.WaterHeightMaxOffset.Text = "";
            controls.NormalMapStrength.Text = "";
            controls.NormalMapBlur.Text = "";
            controls.UseCustomSavePatterns.Checked = false;

            SetSaveSettingsItem(controls.SaveSettings, 0, false);
            SetSaveSettingsItem(controls.SaveSettings, 1, false);
            ClearWipChecklist(controls.Wips);
        }

        private static void SetModSettingsControlsEnabled(ModSettingsControlSet controls, bool enabled)
        {
            controls.MapScale.Enabled = enabled;
            controls.WaterHeight.Enabled = enabled;
            controls.WaterHeightMinOffset.Enabled = enabled;
            controls.WaterHeightMaxOffset.Enabled = enabled;
            controls.NormalMapStrength.Enabled = enabled;
            controls.NormalMapBlur.Enabled = enabled;
            controls.SaveSettings.Enabled = enabled;
            controls.UseCustomSavePatterns.Enabled = enabled;
        }

        private static void ApplyWipChecklist(CheckedListBox listBox, ModSettings settings)
        {
            if (listBox == null || settings == null)
                return;

            foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
            {
                int index = (int)enumObj;
                if (index >= listBox.Items.Count)
                    continue;

                listBox.SetItemChecked(index, settings.CheckWips(enumObj));
            }
        }

        private static void ClearWipChecklist(CheckedListBox listBox)
        {
            if (listBox == null)
                return;

            for (int i = 0; i < listBox.Items.Count; i++)
                listBox.SetItemChecked(i, false);
        }

        private static void SetSaveSettingsItem(CheckedListBox listBox, int index, bool value)
        {
            if (listBox == null || index >= listBox.Items.Count)
                return;

            listBox.SetItemChecked(index, value);
        }

        private sealed class ModSettingsControlSet
        {
            public ModSettingsControlSet(
                TextBox mapScale,
                TextBox waterHeight,
                TextBox waterHeightMinOffset,
                TextBox waterHeightMaxOffset,
                TextBox normalMapStrength,
                TextBox normalMapBlur,
                CheckedListBox saveSettings,
                CheckedListBox wips,
                CheckBox useCustomSavePatterns)
            {
                MapScale = mapScale;
                WaterHeight = waterHeight;
                WaterHeightMinOffset = waterHeightMinOffset;
                WaterHeightMaxOffset = waterHeightMaxOffset;
                NormalMapStrength = normalMapStrength;
                NormalMapBlur = normalMapBlur;
                SaveSettings = saveSettings;
                Wips = wips;
                UseCustomSavePatterns = useCustomSavePatterns;
            }

            public TextBox MapScale { get; }
            public TextBox WaterHeight { get; }
            public TextBox WaterHeightMinOffset { get; }
            public TextBox WaterHeightMaxOffset { get; }
            public TextBox NormalMapStrength { get; }
            public TextBox NormalMapBlur { get; }
            public CheckedListBox SaveSettings { get; }
            public CheckedListBox Wips { get; }
            public CheckBox UseCustomSavePatterns { get; }
        }

        private void ComboBox_UsingSettingsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (!isLoading)
                    SettingsManager.Settings.useModSettings = ComboBox_UsingSettingsType.SelectedIndex == 1;
                tabControl1.SelectedIndex = SettingsManager.Settings.useModSettings ? 1 : 0;
            });
        }

        private void Button_CreateModSettings_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var settings = SettingsManager.Settings;

                if (!settings.IsModDirectorySelected())
                {
                    Button_CreateModSettings.Enabled = false;
                    return;

                }

                if (!settings.IsModSettingsFileExists())
                {
                    settings.currentModSettings = new ModSettings();

                    if (!Directory.Exists(settings.GetModSettingsDirectoryPath()))
                        Directory.CreateDirectory(settings.GetModSettingsDirectoryPath());

                    File.WriteAllText(
                        settings.GetModSettingsFilePath(),
                        JsonConvert.SerializeObject(settings.currentModSettings, Formatting.Indented)
                    );
                    LoadData();
                }

                Button_CreateModSettings.Enabled = false;

            });
        }

        private void Button_Directory_AutoDetect_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                var documentsFolder = FileManager.GetDocumentsFolderPath();
                var gameTempPath = FileManager.AssembleFolderPath(new[] { documentsFolder, "Paradox Interactive", "Hearts of Iron IV" });
                if (!IsValidGameTempDirectory(gameTempPath))
                    gameTempPath = null;

                var steamPathes = FileManager.GetSteamAppsFolders();
                string gameDirectoryPath = null;

                foreach (var steamGamePath in steamPathes)
                {
                    var gamePath = FileManager.AssembleFolderPath(new[] { steamGamePath, "common", "Hearts of Iron IV" });
                    if (IsValidGameDirectory(gamePath))
                    {
                        gameDirectoryPath = gamePath;
                        break;
                    }
                }

                if (gameTempPath != null)
                    SettingsManager.Settings.gameTempDirectory = gameTempPath;
                if (gameDirectoryPath != null)
                    SettingsManager.Settings.gameDirectory = gameDirectoryPath;

                SettingsManager.Settings.modDirectory = TextBox_ModDirectory.Text;

                LoadData();
            });
        }
    }
}
