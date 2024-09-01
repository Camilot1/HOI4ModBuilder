using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class ArgumentMustBeVarDeclarator : ScriptException
    {
        public ArgumentMustBeVarDeclarator(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_ARGUMENT_MUST_BE_VAR_DECLARATOR, lineIndex, args)
        { }
    }
}
