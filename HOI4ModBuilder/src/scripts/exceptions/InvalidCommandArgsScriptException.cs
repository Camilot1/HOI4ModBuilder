using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidCommandArgsScriptException : ScriptException
    {
        public InvalidCommandArgsScriptException(int lineIndex, string[] args, object value, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_COMMAND_ARGS, lineIndex, args, value, argIndex)
        { }
    }
}

