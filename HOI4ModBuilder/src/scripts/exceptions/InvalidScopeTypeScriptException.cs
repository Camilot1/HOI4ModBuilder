using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidScopeTypeScriptException : ScriptException
    {
        public InvalidScopeTypeScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_SCOPE_TYPE, lineIndex, args)
        { }
    }
}
