using HOI4ModBuilder.src.utils;
using System;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InternalScriptException : ScriptException
    {
        public InternalScriptException(int lineIndex, string[] args, Exception ex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INTERNAL, lineIndex, args, ex)
        { }
    }
}
