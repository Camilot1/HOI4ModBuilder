using HOI4ModBuilder.src.utils;
namespace HOI4ModBuilder.src.scripts.exceptions
{
    internal class ArgumentMustNotBeMarkedAsOutScriptException : ScriptException
    {
        public ArgumentMustNotBeMarkedAsOutScriptException(int lineIndex, string[] args, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_ARGUMENT_MUST_NOT_BE_MARKED_AS_OUT, lineIndex, args, argIndex)
        { }
    }
}