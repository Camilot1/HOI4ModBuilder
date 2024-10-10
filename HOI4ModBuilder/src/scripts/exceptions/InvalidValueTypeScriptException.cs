using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidValueTypeScriptException : ScriptException
    {
        public InvalidValueTypeScriptException(int lineIndex, string[] args, object value, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_VALUE_TYPE, lineIndex, args, value, argIndex)
        { }

        public InvalidValueTypeScriptException(int lineIndex, string[] args, object value)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_VALUE_TYPE_INTERNAL, lineIndex, args, value)
        { }
    }
}
