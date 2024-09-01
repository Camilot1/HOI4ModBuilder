using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    internal class ArgumentMustBeMarkedAsOutScriptException : ScriptException
    {
        public ArgumentMustBeMarkedAsOutScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_ARGUMENT_MUST_BE_MARKED_AS_OUT, lineIndex, args)
        { }
    }
}
