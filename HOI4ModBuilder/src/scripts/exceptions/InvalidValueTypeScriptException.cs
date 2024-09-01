using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidValueTypeScriptException : ScriptException
    {
        public InvalidValueTypeScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_VALUE_TYPE, lineIndex, args)
        { }
    }
}
