using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidArgsCountScriptException : ScriptException
    {
        public InvalidArgsCountScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_ARGS_COUNT, lineIndex, args)
        { }
    }
}
