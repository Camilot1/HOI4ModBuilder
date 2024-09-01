using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidCommandArgsCountScriptException : ScriptException
    {
        public InvalidCommandArgsCountScriptException(int lineIndex, string[] args, int[] allowedCounts)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_COMMAND_ARGS_COUNT, lineIndex, args, allowedCounts)
        { }
    }
}
