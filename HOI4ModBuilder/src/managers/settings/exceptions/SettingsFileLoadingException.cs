using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.settings.exceptions
{
    public class SettingsFileLoadingException : Exception
    {
        public SettingsFileLoadingException(Exception ex) : base(
            GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_WHILE_SETTINGS_FILE_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{settingsFilepath}", SettingsFilePath },
                            { "{exceptionMessage}", ex.Message }
                        }
                    ),
                    ex
            )
        { }
    }
}
