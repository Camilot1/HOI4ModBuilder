using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class FileNotFoundScriptException : ScriptException
    {
        public FileNotFoundScriptException(int lineIndex, string[] args, object value)
            : base(EnumLocKey.SCRIPT_EXCEPTION_FILE_NOT_FOUND_INTERNAL, lineIndex, args, value)
        { }
        public FileNotFoundScriptException(int lineIndex, string[] args, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_FILE_NOT_FOUND, lineIndex, args, argIndex)
        { }
    }
}
