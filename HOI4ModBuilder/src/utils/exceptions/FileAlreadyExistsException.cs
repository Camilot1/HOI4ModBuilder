using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.exceptions
{
    public class FileAlreadyExistsException : Exception
    {
        public FileAlreadyExistsException(string filePath) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_FILE_ALREADY_EXISTS,
                new Dictionary<string, string> { { "{filePath}", filePath } }
            ))
        { }
    }
}
