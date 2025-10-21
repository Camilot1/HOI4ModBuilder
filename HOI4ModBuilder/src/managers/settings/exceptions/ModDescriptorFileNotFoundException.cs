using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.managers.settings.exceptions
{
    public class ModDescriptorFileNotFoundException : FileNotFoundException
    {
        public ModDescriptorFileNotFoundException(string path) : base(
            GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_MOD_DESCRIPTOR_FILE_NOT_FOUND,
                    new Dictionary<string, string> { { "{directoryPath}", path } }
                )
            )
        { }
    }
}
