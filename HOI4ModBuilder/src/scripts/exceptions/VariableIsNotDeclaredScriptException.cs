using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class VariableIsNotDeclaredScriptException : ScriptException
    {
        public VariableIsNotDeclaredScriptException(int lineIndex, string[] args, object value)
            : base(EnumLocKey.SCRIPT_EXCEPTION_VARIABLE_IS_NOT_DECLARED, lineIndex, args, value)
        { }
    }
}
