using HOI4ModBuilder.src.utils;
using System;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InvalidOperationScriptException : ScriptException
    {
        public InvalidOperationScriptException(int lineIndex, string[] args)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INVALID_OPERATION, lineIndex, args)
        { }
    }
}
