using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidTypeScriptException : ScriptException
    {
        public InvalidTypeScriptException(int lineIndex, string[] args, object value, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_TYPE, lineIndex, args, value, argIndex)
        { }
    }
}
