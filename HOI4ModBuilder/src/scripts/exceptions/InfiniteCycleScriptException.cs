using HOI4ModBuilder.src.utils;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class InfiniteCycleScriptException : ScriptException
    {
        public InfiniteCycleScriptException(int lineIndex, string[] args, object start, object end, object step)
            : base(EnumLocKey.SCRIPT_EXCEPTION_INFINITE_CYCLE, lineIndex, args, new Dictionary<string, string>
            {
                { "{start}", "" + start },
                { "{end}", "" + end },
                { "{step}", "" + step }
            })
        { }
    }
}
