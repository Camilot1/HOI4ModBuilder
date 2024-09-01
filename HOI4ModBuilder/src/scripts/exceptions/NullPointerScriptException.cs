using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class NullPointerScriptException : ScriptException
    {
        public NullPointerScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_NULL_POINTER, lineIndex, args)
        { }
    }
}
