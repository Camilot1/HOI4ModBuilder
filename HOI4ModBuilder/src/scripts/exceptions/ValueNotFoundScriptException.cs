
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class ValueNotFoundScriptException : ScriptException
    {
        public ValueNotFoundScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_VALUE_NOT_FOUND, lineIndex, args)
        { }
    }
}
