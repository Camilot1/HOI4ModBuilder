using HOI4ModBuilder.src.managers;
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

        public SettingsForm()
        {
            InitializeComponent();
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

            if (settings.defaultModSettings != null)
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Text = "" + settings.defaultModSettings.MAP_SCALE_PIXEL_TO_KM;
                TextBox_WATER_HEIGHT_Default.Text = "" + settings.defaultModSettings.WATER_HEIGHT;
                TextBox_WATER_HEIGHT_min_land_offset_Default.Text = "" + settings.defaultModSettings.WATER_HEIGHT_minLandOffset;
                TextBox_WATER_HEIGHT_max_water_offset_Default.Text = "" + settings.defaultModSettings.WATER_HEIGHT_maxWaterOffset;
                TextBox_NormalMapStrength_Default.Text = "" + settings.defaultModSettings.normalMapStrength;
                TextBox_NormalMapBlur_Default.Text = "" + settings.defaultModSettings.normalMapBlur;
                CheckBox_UseCustomSavePatterns_Default.Checked = settings.defaultModSettings.useCustomSavePatterns;

                CheckedListBox_SaveSettings_Default.SetItemChecked(0, settings.defaultModSettings.exportRiversMapWithWaterPixels);
                CheckedListBox_SaveSettings_Default.SetItemChecked(1, settings.defaultModSettings.generateNormalMap);

                foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
                {
                    int index = (int)enumObj;
                    if (index >= CheckedListBox_Wips_Default.Items.Count)
                        continue;

                    CheckedListBox_Wips_Default.SetItemChecked(index, settings.defaultModSettings.CheckWips(enumObj));
                }

                TextBox_WATER_HEIGHT_Default.Enabled = true;
                TextBox_WATER_HEIGHT_min_land_offset_Default.Enabled = true;
                TextBox_WATER_HEIGHT_max_water_offset_Default.Enabled = true;
                TextBox_NormalMapStrength_Default.Enabled = true;
                TextBox_NormalMapBlur_Default.Enabled = true;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Enabled = true;
                CheckedListBox_SaveSettings_Default.Enabled = true;
                CheckBox_UseCustomSavePatterns_Default.Enabled = true;
            }
            else
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Text = "";
                TextBox_WATER_HEIGHT_Default.Text = "";
                TextBox_WATER_HEIGHT_min_land_offset_Default.Text = "";
                TextBox_WATER_HEIGHT_max_water_offset_Default.Text = "";
                TextBox_NormalMapStrength_Default.Text = "";
                TextBox_NormalMapBlur_Default.Text = "";

                CheckedListBox_SaveSettings_Default.SetItemChecked(0, false);
                CheckedListBox_SaveSettings_Default.SetItemChecked(1, false);

                for (int i = 0; i < CheckedListBox_Wips_Default.Items.Count; i++)
                    CheckedListBox_Wips_Default.SetItemChecked(i, false);

                TextBox_WATER_HEIGHT_Default.Enabled = false;
                TextBox_WATER_HEIGHT_min_land_offset_Default.Enabled = false;
                TextBox_WATER_HEIGHT_max_water_offset_Default.Enabled = false;
                TextBox_NormalMapStrength_Default.Enabled = false;
                TextBox_NormalMapBlur_Default.Enabled = false;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Enabled = false;
                CheckedListBox_SaveSettings_Default.Enabled = false;
                CheckBox_UseCustomSavePatterns_Default.Enabled = false;
            }

            if (settings.currentModSettings != null)
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Text = "" + settings.currentModSettings.MAP_SCALE_PIXEL_TO_KM;
                TextBox_WATER_HEIGHT_Current.Text = "" + settings.currentModSettings.WATER_HEIGHT;
                TextBox_WATER_HEIGHT_min_land_offset_Current.Text = "" + settings.currentModSettings.WATER_HEIGHT_minLandOffset;
                TextBox_WATER_HEIGHT_max_water_offset_Current.Text = "" + settings.currentModSettings.WATER_HEIGHT_maxWaterOffset;
                TextBox_NormalMapStrength_Current.Text = "" + settings.currentModSettings.normalMapStrength;
                TextBox_NormalMapBlur_Current.Text = "" + settings.currentModSettings.normalMapBlur;
                CheckBox_UseCustomSavePatterns_Current.Checked = settings.currentModSettings.useCustomSavePatterns;

                CheckedListBox_SaveSettings_Current.SetItemChecked(0, settings.currentModSettings.exportRiversMapWithWaterPixels);
                CheckedListBox_SaveSettings_Current.SetItemChecked(1, settings.currentModSettings.generateNormalMap);

                foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
                {
                    int index = (int)enumObj;
                    if (index >= CheckedListBox_SaveSettings_Current.Items.Count)
                        continue;

                    CheckedListBox_Wips_Current.SetItemChecked(index, settings.defaultModSettings.CheckWips(enumObj));
                }

                TextBox_WATER_HEIGHT_Current.Enabled = true;
                TextBox_WATER_HEIGHT_min_land_offset_Current.Enabled = true;
                TextBox_WATER_HEIGHT_max_water_offset_Current.Enabled = true;
                TextBox_NormalMapStrength_Current.Enabled = true;
                TextBox_NormalMapBlur_Current.Enabled = true;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Enabled = true;
                CheckedListBox_SaveSettings_Current.Enabled = true;
                CheckBox_UseCustomSavePatterns_Current.Enabled = true;
            }
            else
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Text = "";
                TextBox_WATER_HEIGHT_Current.Text = "";
                TextBox_WATER_HEIGHT_min_land_offset_Current.Text = "";
                TextBox_WATER_HEIGHT_max_water_offset_Current.Text = "";
                TextBox_NormalMapStrength_Current.Text = "";
                TextBox_NormalMapBlur_Current.Text = "";

                CheckedListBox_SaveSettings_Current.SetItemChecked(0, false);
                CheckedListBox_SaveSettings_Current.SetItemChecked(1, false);

                for (int i = 0; i < CheckedListBox_Wips_Current.Items.Count; i++)
                    CheckedListBox_Wips_Current.SetItemChecked(i, false);

                TextBox_WATER_HEIGHT_Current.Enabled = false;
                TextBox_WATER_HEIGHT_min_land_offset_Current.Enabled = false;
                TextBox_WATER_HEIGHT_max_water_offset_Current.Enabled = false;
                TextBox_NormalMapStrength_Current.Enabled = false;
                TextBox_NormalMapBlur_Current.Enabled = false;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Enabled = false;
                CheckedListBox_SaveSettings_Current.Enabled = false;
                CheckBox_UseCustomSavePatterns_Current.Enabled = false;
            }

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

                settings.modDirectory = TextBox_ModDirectory.Text;
                if (PathCheck(settings.modDirectory))
                    settings.modDirectory += '\\';

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

                //Default settings
                var modSettings = settings.defaultModSettings;
                modSettings.MAP_SCALE_PIXEL_TO_KM = Utils.ParseFloat(TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Text);
                modSettings.WATER_HEIGHT = Utils.ParseFloat(TextBox_WATER_HEIGHT_Default.Text);
                modSettings.WATER_HEIGHT_minLandOffset = Utils.ParseFloat(TextBox_WATER_HEIGHT_min_land_offset_Default.Text);
                modSettings.WATER_HEIGHT_maxWaterOffset = Utils.ParseFloat(TextBox_WATER_HEIGHT_max_water_offset_Default.Text);
                modSettings.normalMapStrength = Utils.ParseFloat(TextBox_NormalMapStrength_Default.Text);
                modSettings.normalMapBlur = Utils.ParseFloat(TextBox_NormalMapBlur_Default.Text);

                modSettings.useCustomSavePatterns = CheckBox_UseCustomSavePatterns_Default.Checked;
                modSettings.exportRiversMapWithWaterPixels = CheckedListBox_SaveSettings_Default.GetItemChecked(0);
                modSettings.generateNormalMap = CheckedListBox_SaveSettings_Default.GetItemChecked(1);

                foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
                {
                    int index = (int)enumObj;
                    if (index >= CheckedListBox_Wips_Default.Items.Count)
                        continue;

                    modSettings.SetWips(enumObj, CheckedListBox_Wips_Default.GetItemChecked(index));
                }

                //Current mod settings
                modSettings = settings.currentModSettings;

                if (modSettings != null)
                {
                    modSettings.MAP_SCALE_PIXEL_TO_KM = Utils.ParseFloat(TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Text);
                    modSettings.WATER_HEIGHT = Utils.ParseFloat(TextBox_WATER_HEIGHT_Current.Text);
                    modSettings.WATER_HEIGHT_minLandOffset = Utils.ParseFloat(TextBox_WATER_HEIGHT_min_land_offset_Current.Text);
                    modSettings.WATER_HEIGHT_maxWaterOffset = Utils.ParseFloat(TextBox_WATER_HEIGHT_max_water_offset_Current.Text);
                    modSettings.normalMapStrength = Utils.ParseFloat(TextBox_NormalMapStrength_Current.Text);
                    modSettings.normalMapBlur = Utils.ParseFloat(TextBox_NormalMapBlur_Current.Text);

                    modSettings.useCustomSavePatterns = CheckBox_UseCustomSavePatterns_Current.Checked;
                    modSettings.exportRiversMapWithWaterPixels = CheckedListBox_SaveSettings_Current.GetItemChecked(0);
                    modSettings.generateNormalMap = CheckedListBox_SaveSettings_Current.GetItemChecked(1);

                    foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
                    {
                        int index = (int)enumObj;
                        if (index >= CheckedListBox_Wips_Current.Items.Count)
                            continue;

                        modSettings.SetWips(enumObj, CheckedListBox_Wips_Current.GetItemChecked(index));
                    }
                }

                SettingsManager.SaveSettings();
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

        private void ComboBox_UsingSettingsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (!isLoading)
                    SettingsManager.Settings.useModSettings = ComboBox_UsingSettingsType.SelectedIndex == 1;
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
