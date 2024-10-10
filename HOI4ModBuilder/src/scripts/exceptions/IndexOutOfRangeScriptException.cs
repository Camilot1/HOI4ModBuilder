using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class IndexOutOfRangeScriptException : ScriptException
    {
        public IndexOutOfRangeScriptException(int lineIndex, string[] args, object value, int argIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INDEX_OUT_OF_RANGE, lineIndex, args, value, argIndex)
        { }
        public IndexOutOfRangeScriptException(int lineIndex, string[] args, object value)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INDEX_OUT_OF_RANGE_INTERNAL, lineIndex, args, value)
        { }
    }
}
