using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.settings.exceptions
{
    public class SettingsFileLoadingException : Exception
    {
        public SettingsFileLoadingException(string settingsFilePath, Exception ex) : base(
            GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_WHILE_SETTINGS_FILE_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{settingsFilepath}", settingsFilePath },
                            { "{exceptionMessage}", ex.Message }
                        }
                    ),
                    ex
            )
        { }
    }
}
