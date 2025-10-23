using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.managers
{
    public class LocalModDataManager
    {
        public static void Load(BaseSettings settings)
        {
            settings.currentModSettings = null;

            if (!settings.IsModDirectorySelected())
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_MOD_DIRECTORY_NOT_FOUND));

            if (!settings.IsModSettingsFileExists() && settings.useModSettings)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_USING_CURRENT_MOD_SETTINGS_AND_HOI4MODBUILDER_DIRECTORY_NOT_FOUND,
                    new Dictionary<string, string> { { "{hoi4ModBuilderModSettingsFilePath}", $"{settings.GetModSettingsFilePath()}" } }
                ));

            if (!settings.IsModSettingsFileExists())
            {
                settings.currentModSettings = null;
                return;
            }

            LoadSettings(settings);

        }

        public static void LoadSettings(BaseSettings settings)
        {
            if (!settings.IsModSettingsFileExists())
            {
                settings.currentModSettings = null;
                return;
            }

            settings.currentModSettings = JsonConvert.DeserializeObject<ModSettings>(File.ReadAllText(settings.GetModSettingsFilePath()));
            settings.currentModSettings?.PostInit();
        }

        public static void SaveLocalSettings(BaseSettings settings)
        {
            if (!settings.IsModDirectorySelected() || !settings.useModSettings)
                return;

            if (!settings.IsModSettingsDirectoryExists())
            {
                string title = GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION);
                string text = GuiLocManager.GetLoc(
                    EnumLocKey.CANT_SAVE_BECAUSE_HOI4MODBUILDER_DIRECTORY_DOESNT_EXISTS,
                    new Dictionary<string, string>
                    {
                        { "{modDirectory}", settings.modDirectory },
                        { "{modSettingsDirectory}", BaseSettings.ModSettingsDirectory }
                    }
                );

                var dialogResult = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                if (dialogResult == DialogResult.OK)
                    Directory.CreateDirectory(settings.GetModSettingsDirectoryPath());
                else
                    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_SAVING_PROCESS_ABORTED));
            }

            File.WriteAllText(
                settings.GetModSettingsFilePath(),
                JsonConvert.SerializeObject(settings.currentModSettings, Formatting.Indented)
            );
        }
    }
}
