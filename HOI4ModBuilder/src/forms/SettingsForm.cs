using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            if (settings.defaultModSettings != null)
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Text = "" + settings.defaultModSettings.MAP_SCALE_PIXEL_TO_KM;
                TextBox_WATER_HEIGHT_Default.Text = "" + settings.defaultModSettings.WATER_HEIGHT;
                TextBox_NormalMapStrength_Default.Text = "" + settings.defaultModSettings.normalMapStrength;

                CheckedListBox_SaveSettings_Default.SetItemChecked(0, settings.defaultModSettings.exportRiversMapWithWaterPixels);
                CheckedListBox_SaveSettings_Default.SetItemChecked(1, settings.defaultModSettings.generateNormalMap);

                TextBox_WATER_HEIGHT_Default.Enabled = true;
                TextBox_NormalMapStrength_Default.Enabled = true;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Enabled = true;
                CheckedListBox_SaveSettings_Default.Enabled = true;
            }
            else
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Text = "";
                TextBox_WATER_HEIGHT_Default.Text = "";
                TextBox_NormalMapStrength_Default.Text = "";

                CheckedListBox_SaveSettings_Default.SetItemChecked(0, false);
                CheckedListBox_SaveSettings_Default.SetItemChecked(1, false);

                TextBox_WATER_HEIGHT_Default.Enabled = false;
                TextBox_NormalMapStrength_Default.Enabled = false;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Enabled = false;
                CheckedListBox_SaveSettings_Default.Enabled = false;
            }

            if (settings.currentModSettings != null)
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Text = "" + settings.currentModSettings.MAP_SCALE_PIXEL_TO_KM;
                TextBox_WATER_HEIGHT_Current.Text = "" + settings.currentModSettings.WATER_HEIGHT;
                TextBox_NormalMapStrength_Current.Text = "" + settings.currentModSettings.normalMapStrength;

                CheckedListBox_SaveSettings_Current.SetItemChecked(0, settings.currentModSettings.exportRiversMapWithWaterPixels);
                CheckedListBox_SaveSettings_Current.SetItemChecked(1, settings.currentModSettings.generateNormalMap);

                TextBox_WATER_HEIGHT_Current.Enabled = true;
                TextBox_NormalMapStrength_Current.Enabled = true;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Enabled = true;
                CheckedListBox_SaveSettings_Current.Enabled = true;
            }
            else
            {
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Text = "";
                TextBox_WATER_HEIGHT_Current.Text = "";
                TextBox_NormalMapStrength_Current.Text = "";

                CheckedListBox_SaveSettings_Current.SetItemChecked(0, false);
                CheckedListBox_SaveSettings_Current.SetItemChecked(1, false);

                TextBox_WATER_HEIGHT_Current.Enabled = false;
                TextBox_NormalMapStrength_Current.Enabled = false;
                TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Enabled = false;
                CheckedListBox_SaveSettings_Current.Enabled = false;
            }

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
                    string path = DirDialog.SelectedPath + @"\";

                    if (!File.Exists(path + "hoi4.exe"))
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
                    string path = DirDialog.SelectedPath + @"\";

                    if (!Directory.Exists(path + @"save games"))
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

        private void Button_ModDirectory_Click(object sender, EventArgs e)
        {
            var thread = new Thread(() =>
            {
                var DirDialog = new FolderBrowserDialog
                {
                    Description = GuiLocManager.GetLoc(EnumLocKey.FOLDER_BROWSER_DIALOG_CHOOSE_DIRECTORY_OF_MOD_TITLE),
                    SelectedPath = TextBox_ModDirectory.Text
                };

                if (DirDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = DirDialog.SelectedPath + @"\";

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
                if (settings.gameDirectory[settings.gameDirectory.Length - 1] != '\\')
                {
                    settings.gameDirectory += '\\';
                }

                settings.gameTempDirectory = TextBox_GameTempDirectory.Text;
                if (settings.gameTempDirectory[settings.gameTempDirectory.Length - 1] != '\\')
                {
                    settings.gameTempDirectory += '\\';
                }

                settings.modDirectory = TextBox_ModDirectory.Text;
                if (settings.modDirectory[settings.modDirectory.Length - 1] != '\\')
                {
                    settings.modDirectory += '\\';
                }

                settings.actionHistorySize = int.Parse(TextBox_ActionHistorySize.Text);

                float textureOpacity = float.Parse(TextBox_Textures_Opacity.Text.Replace('.', ',')) / 100f;
                if (textureOpacity < 0) settings.textureOpacity = 0;
                else if (textureOpacity > 1) settings.textureOpacity = 255;
                else settings.textureOpacity = (byte)Math.Round(textureOpacity * 255);

                settings.MAP_VIEWPORT_HEIGHT = double.Parse(TextBox_MAP_VIEWPORT_HEIGHT.Text.Replace('.', ','));
                settings.maxAdditionalTextureSize = int.Parse(ComboBox_MaxAdditionalTextureSize.Text);

                if (settings.defaultModSettings != null)
                {
                    settings.defaultModSettings.MAP_SCALE_PIXEL_TO_KM = double.Parse(TextBox_MAP_SCALE_PIXEL_TO_KM_Default.Text.Replace('.', ','));
                    settings.defaultModSettings.WATER_HEIGHT = double.Parse(TextBox_WATER_HEIGHT_Default.Text.Replace('.', ','));
                    settings.defaultModSettings.normalMapStrength = double.Parse(TextBox_NormalMapStrength_Default.Text.Replace('.', ','));

                    settings.defaultModSettings.exportRiversMapWithWaterPixels = CheckedListBox_SaveSettings_Default.GetItemChecked(0);
                    settings.defaultModSettings.generateNormalMap = CheckedListBox_SaveSettings_Default.GetItemChecked(1);
                }

                if (settings.currentModSettings != null)
                {
                    settings.currentModSettings.MAP_SCALE_PIXEL_TO_KM = double.Parse(TextBox_MAP_SCALE_PIXEL_TO_KM_Current.Text.Replace('.', ','));
                    settings.currentModSettings.WATER_HEIGHT = double.Parse(TextBox_WATER_HEIGHT_Current.Text.Replace('.', ','));
                    settings.currentModSettings.normalMapStrength = double.Parse(TextBox_NormalMapStrength_Current.Text.Replace('.', ','));

                    settings.currentModSettings.exportRiversMapWithWaterPixels = CheckedListBox_SaveSettings_Current.GetItemChecked(0);
                    settings.currentModSettings.generateNormalMap = CheckedListBox_SaveSettings_Current.GetItemChecked(1);
                }

                SettingsManager.SaveSettings();
                if (prevLang != settings.language) GuiLocManager.SetCurrentUICulture(settings.language);
                else LoadData();
            });
        }

        private void ComboBox_UsingSettingsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                if (!isLoading) SettingsManager.Settings.useModSettings = ComboBox_UsingSettingsType.SelectedIndex == 1;
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
                    File.WriteAllText(
                        settings.GetModSettingsFilePath(),
                        JsonConvert.SerializeObject(settings.currentModSettings, Formatting.Indented)
                    );
                    LoadData();
                }

                Button_CreateModSettings.Enabled = false;

            });
        }
    }
}
