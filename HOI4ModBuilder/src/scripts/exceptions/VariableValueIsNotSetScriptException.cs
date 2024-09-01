using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class VariableValueIsNotSetScriptException : ScriptException
    {
        public VariableValueIsNotSetScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_IS_NOT_SET, lineIndex, args)
        { }
    }
}
