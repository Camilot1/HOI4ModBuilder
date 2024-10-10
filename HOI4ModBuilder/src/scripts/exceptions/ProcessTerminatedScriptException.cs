using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.exceptions
{
    public class ProcessTerminatedScriptException : ScriptException
    {
        public ProcessTerminatedScriptException(int lineIndex)
            : base(EnumLocKey.SCRIPT_EXCEPTION_PROCESS_TERMINATED, lineIndex)
        { }
    }
}
