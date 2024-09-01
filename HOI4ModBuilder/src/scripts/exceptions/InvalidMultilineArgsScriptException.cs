using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidMultilineArgsScriptException : ScriptException
    {
        public InvalidMultilineArgsScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_MULTILINE_ARGS, lineIndex, args)
        { }
    }
}