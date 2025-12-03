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
using static HOI4ModBuilder.src.managers.settings.ModSettings;

namespace HOI4ModBuilder.src.forms
{
    public partial class SettingsForm : Form
    {

        public static SettingsForm Instance;
        public static bool isLoading = false;

        private ModSettingsControlSet modSettingsControls;

        public SettingsForm()
        {
            InitializeComponent();
            InitializeModControlSets();
            Instance?.Close();
            Instance = this;
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
            Instance = null;
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

            var modSettings = settings.GetModSettings();

            modSettingsControls.ApplyFrom(modSettings);

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
                        Logger.LogSingleErrorMessage(EnumLocKey.SINGLE_MESSAGE_NO_REQUIRED_FOLDER_IN_DIRECTORY_IN_DOCUMENTS, new Dictionary<string, string> { { "{directoryName}", "mod" } });
                        return;
                    }
                    Invoke(new Action(() => TextBox_GameTempDirectory.Text = path));
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public bool IsValidGameTempDirectory(string path)
            => Directory.Exists(path + "mod");

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

                modSettingsControls.SaveTo(settings.GetModSettings());

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
            modSettingsControls = CreateDefaultModControls();
        }

        private ModSettingsControlSet CreateDefaultModControls()
            => new ModSettingsControlSet(
                TextBox_MAP_SCALE_PIXEL_TO_KM,
                TextBox_WATER_HEIGHT,
                TextBox_WATER_HEIGHT_min_land_offset,
                TextBox_WATER_HEIGHT_max_water_offset,
                TextBox_NormalMapStrength,
                TextBox_NormalMapBlur,
                CheckedListBox_SaveSettings,
                CheckedListBox_Wips,
                CheckBox_UseCustomSavePatterns,
                ComboBox_SelectedColorGenerationPattern,
                DataGridView_ColorGenerationPatterns
            );

        private sealed class ModSettingsControlSet
        {

            public TextBox MapScale { get; }
            public TextBox WaterHeight { get; }
            public TextBox WaterHeightMinOffset { get; }
            public TextBox WaterHeightMaxOffset { get; }
            public TextBox NormalMapStrength { get; }
            public TextBox NormalMapBlur { get; }
            public CheckedListBox SaveSettings { get; }
            public CheckedListBox Wips { get; }
            public CheckBox UseCustomSavePatterns { get; }
            public ComboBox SelectedColorGenerationPattern { get; }
            public DataGridView ColorGenerationPatternParameters { get; }

            public ModSettingsControlSet(
                TextBox mapScale,
                TextBox waterHeight,
                TextBox waterHeightMinOffset,
                TextBox waterHeightMaxOffset,
                TextBox normalMapStrength,
                TextBox normalMapBlur,
                CheckedListBox saveSettings,
                CheckedListBox wips,
                CheckBox useCustomSavePatterns,
                ComboBox selectedColorGenerationPattern,
                DataGridView colorGenerationPatternParameters
                )
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
                SelectedColorGenerationPattern = selectedColorGenerationPattern;
                ColorGenerationPatternParameters = colorGenerationPatternParameters;
            }

            public void LoadFrom(ModSettings modSettings)
            {
                if (modSettings == null)
                {
                    ClearModSettingsControls();
                    return;
                }
                MapScale.Text = "" + modSettings.MAP_SCALE_PIXEL_TO_KM;
                WaterHeight.Text = "" + modSettings.WATER_HEIGHT;
                WaterHeightMinOffset.Text = "" + modSettings.WATER_HEIGHT_minLandOffset;
                WaterHeightMaxOffset.Text = "" + modSettings.WATER_HEIGHT_maxWaterOffset;
                NormalMapStrength.Text = "" + modSettings.normalMapStrength;
                NormalMapBlur.Text = "" + modSettings.normalMapBlur;
                UseCustomSavePatterns.Checked = modSettings.useCustomSavePatterns;

                SetSaveSettingsItem(SaveSettings, 0, modSettings.exportRiversMapWithWaterPixels);
                SetSaveSettingsItem(SaveSettings, 1, modSettings.generateNormalMap);
                ApplyWipChecklist(Wips, modSettings);

                RepopulateComboBoxColorGenerationPatterns();

                InitColorGenerationPatterns(modSettings);

            }

            private const int HSV_RANGE_DIF_PARAM_INDEX = 0;
            private const int HSV_RANGE_H_PARAM_INDEX = 1;
            private const int HSV_RANGE_S_PARAM_INDEX = 2;
            private const int HSV_RANGE_V_PARAM_INDEX = 3;

            private void RepopulateComboBoxColorGenerationPatterns()
            {
                var tempIndex = SelectedColorGenerationPattern.SelectedIndex;
                if (tempIndex < 0)
                    tempIndex = 0;

                SelectedColorGenerationPattern.Items.Clear();
                foreach (var obj in Enum.GetValues(typeof(EnumColorGenerationPattern)))
                    SelectedColorGenerationPattern.Items.Add(GuiLocManager.GetLoc("" + obj));

                if (SelectedColorGenerationPattern.Items.Count > 0)
                {
                    tempIndex = Math.Min(tempIndex, SelectedColorGenerationPattern.Items.Count - 1);
                    SelectedColorGenerationPattern.SelectedIndex = tempIndex;
                }
                else
                {
                    SelectedColorGenerationPattern.SelectedIndex = -1;
                }
                SelectedColorGenerationPattern.Refresh();
            }

            public void InitColorGenerationPatterns(ModSettings modSettings)
            {
                if (modSettings == null)
                    return;

                var selectedIndex = SelectedColorGenerationPattern.SelectedIndex;
                if (selectedIndex < 0)
                {
                    ColorGenerationPatternParameters.Rows.Clear();
                    return;
                }

                var pattern = (EnumColorGenerationPattern)selectedIndex;
                var hsvRanges = modSettings.GetHSVRanges(pattern);

                ColorGenerationPatternParameters.Rows.Clear();

                AssembleRow(HSV_RANGE_DIF_PARAM_INDEX, EnumLocKey.COLOR_GENERATION_PATTERN_HSV_RANGES_DIFFERENCE, hsvRanges.minDif, hsvRanges.maxDif);
                AssembleRow(HSV_RANGE_H_PARAM_INDEX, EnumLocKey.COLOR_GENERATION_PATTERN_HSV_RANGES_HUE, hsvRanges.minH, hsvRanges.maxH);
                AssembleRow(HSV_RANGE_S_PARAM_INDEX, EnumLocKey.COLOR_GENERATION_PATTERN_HSV_RANGES_SATURATION, hsvRanges.minS, hsvRanges.maxS);
                AssembleRow(HSV_RANGE_V_PARAM_INDEX, EnumLocKey.COLOR_GENERATION_PATTERN_HSV_RANGES_VALUE, hsvRanges.minV, hsvRanges.maxV);

                void AssembleRow(int parameterIndex, EnumLocKey loc, double min, double max)
                {
                    var row = new DataGridViewRow();
                    row.Tag = parameterIndex;
                    var minString = double.IsInfinity(min) ? "Infinity" : "" + min;
                    var maxString = double.IsInfinity(max) ? "Infinity" : "" + max;
                    row.CreateCells(ColorGenerationPatternParameters, new object[] { GuiLocManager.GetLoc(loc), minString, maxString });
                    ColorGenerationPatternParameters.Rows.Add(row);
                }
            }

            public void ApplyFrom(ModSettings modSettings)
            {
                if (modSettings == null)
                {
                    ClearModSettingsControls();
                    return;
                }
                LoadFrom(modSettings);
            }

            public void SaveTo(ModSettings target)
            {
                if (target == null)
                    return;

                // basic numeric settings
                target.MAP_SCALE_PIXEL_TO_KM = Utils.ParseFloat(MapScale.Text);
                target.WATER_HEIGHT = Utils.ParseFloat(WaterHeight.Text);
                target.WATER_HEIGHT_minLandOffset = Utils.ParseFloat(WaterHeightMinOffset.Text);
                target.WATER_HEIGHT_maxWaterOffset = Utils.ParseFloat(WaterHeightMaxOffset.Text);
                target.normalMapStrength = Utils.ParseFloat(NormalMapStrength.Text);
                target.normalMapBlur = Utils.ParseFloat(NormalMapBlur.Text);

                // save pattern toggles
                target.useCustomSavePatterns = UseCustomSavePatterns.Checked;
                if (SaveSettings.Items.Count > 0)
                    target.exportRiversMapWithWaterPixels = SaveSettings.GetItemChecked(0);
                if (SaveSettings.Items.Count > 1)
                    target.generateNormalMap = SaveSettings.GetItemChecked(1);

                foreach (EnumWips enumObj in Enum.GetValues(typeof(EnumWips)))
                {
                    int index = (int)enumObj;
                    if (index >= Wips.Items.Count)
                        continue;

                    target.SetWips(enumObj, Wips.GetItemChecked(index));
                }

                SaveColorGenerationPatterns(target);
            }

            private void SaveColorGenerationPatterns(ModSettings target)
            {
                if (target == null)
                    return;

                var selectedPatternIndex = SelectedColorGenerationPattern.SelectedIndex;
                if (selectedPatternIndex < 0)
                    return;

                var pattern = (EnumColorGenerationPattern)selectedPatternIndex;
                var hsvRanges = target.GetHSVRanges(pattern);
                if (hsvRanges == null)
                    return;

                foreach (DataGridViewRow row in ColorGenerationPatternParameters.Rows)
                {
                    if (row == null || row.IsNewRow || !(row.Tag is int paramIndex))
                        continue;

                    var minValue = Utils.ParseDouble(row.Cells[1].Value + "");
                    var maxValue = Utils.ParseDouble(row.Cells[2].Value + "");

                    switch (paramIndex)
                    {
                        case HSV_RANGE_DIF_PARAM_INDEX:
                            hsvRanges.minDif = minValue;
                            hsvRanges.maxDif = maxValue;
                            break;
                        case HSV_RANGE_H_PARAM_INDEX:
                            hsvRanges.minH = minValue;
                            hsvRanges.maxH = maxValue;
                            break;
                        case HSV_RANGE_S_PARAM_INDEX:
                            hsvRanges.minS = minValue;
                            hsvRanges.maxS = maxValue;
                            break;
                        case HSV_RANGE_V_PARAM_INDEX:
                            hsvRanges.minV = minValue;
                            hsvRanges.maxV = maxValue;
                            break;
                    }
                }
            }

            private void ClearModSettingsControls()
            {
                MapScale.Text = "";
                WaterHeight.Text = "";
                WaterHeightMinOffset.Text = "";
                WaterHeightMaxOffset.Text = "";
                NormalMapStrength.Text = "";
                NormalMapBlur.Text = "";
                UseCustomSavePatterns.Checked = false;

                SetSaveSettingsItem(SaveSettings, 0, false);
                SetSaveSettingsItem(SaveSettings, 1, false);
                ClearWipChecklist(Wips);

                SelectedColorGenerationPattern.Items.Clear();
                ColorGenerationPatternParameters.Rows.Clear();

                RepopulateComboBoxColorGenerationPatterns();
            }


            private void ClearWipChecklist(CheckedListBox listBox)
            {
                if (listBox == null)
                    return;

                for (int i = 0; i < listBox.Items.Count; i++)
                    listBox.SetItemChecked(i, false);
            }

            private void SetSaveSettingsItem(CheckedListBox listBox, int index, bool value)
            {
                if (listBox == null || index >= listBox.Items.Count)
                    return;

                listBox.SetItemChecked(index, value);
            }
            private void ApplyWipChecklist(CheckedListBox listBox, ModSettings settings)
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
        }

        private void ComboBox_UsingSettingsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (!isLoading)
                    SettingsManager.Settings.useModSettings = ComboBox_UsingSettingsType.SelectedIndex == 1;
                modSettingsControls.LoadFrom(SettingsManager.Settings.GetModSettings());
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
                    settings.currentModSettings.PostInit();

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

        private void Button_ModSettings_ApplyChanges_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => modSettingsControls.SaveTo(SettingsManager.Settings.GetModSettings()));

        private void ComboBox_SelectedColorGenerationPattern_SelectedIndexChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => modSettingsControls.InitColorGenerationPatterns(
                SettingsManager.Settings.GetModSettings()
            ));
    }
}
