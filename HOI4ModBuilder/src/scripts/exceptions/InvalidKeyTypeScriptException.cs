using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidKeyTypeScriptException : ScriptException
    {
        public InvalidKeyTypeScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_KEY_TYPE, lineIndex, args)
        { }
    }
}
