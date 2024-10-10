using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class VariableIsAlreadyDeclaredScriptException : ScriptException
    {
        public VariableIsAlreadyDeclaredScriptException(int lineIndex, string[] args, object value, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_VARIABLE_IS_ALREADY_DECLARED, lineIndex, args, value, argIndex)
        { }
    }
}
