using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    internal class UnknownCommandScriptException : ScriptException
    {
        public UnknownCommandScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_UNKNOWN_COMMAND, lineIndex, args)
        { }
    }
}
