using System;

namespace HOI4ModBuilder.src.scripts.commands
{
    public abstract class ScriptCommand
    {
        protected Action _action;
        protected VarsScope _innerVarsScope;
        protected Action _executeBeforeCall;
        protected int lineIndex;

        public abstract void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args);
        public void Execute()
        {
            ScriptParser.AfterCommand(this, lineIndex, _innerVarsScope);
            _executeBeforeCall?.Invoke();
            _action();
        }

        public abstract ScriptCommand CreateEmptyCopy();
        public static string GetKeyword() => throw new NotImplementedException();
        public abstract string GetPath();
        public abstract string[] GetDocumentation();
    }
}
