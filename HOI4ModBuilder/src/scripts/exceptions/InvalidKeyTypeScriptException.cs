using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidKeyTypeScriptException : ScriptException
    {
        public InvalidKeyTypeScriptException(int lineIndex, string[] args, object value, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_KEY_TYPE, lineIndex, args, value, argIndex)
        { }

        public InvalidKeyTypeScriptException(int lineIndex, string[] args, object value)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_KEY_TYPE_INTERNAL, lineIndex, args, value)
        { }
    }
}
