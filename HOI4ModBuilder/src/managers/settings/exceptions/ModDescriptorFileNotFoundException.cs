using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.settings.exceptions
{
    public class ModDescriptionFileNotFoundException : FileNotFoundException
    {
        public ModDescriptionFileNotFoundException(string path) : base(
            GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_MOD_DESCRIPTOR_FILE_NOT_FOUND,
                    new Dictionary<string, string> { { "{directoryPath}", path } }
                )
            )
        { }
    }
}
