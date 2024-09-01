using System;

namespace HOI4ModBuilder.src.scripts.commands
{
    public abstract class ScriptCommand
    {
        protected Action _action;
        protected VarsScope _varsScope;
        protected Action _executeBeforeCall;
        protected int lineIndex;

        public abstract void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args);
        public void Execute()
        {
            ScriptParser.AfterCommand(this, lineIndex, _varsScope);
            _executeBeforeCall?.Invoke();
            _action();
        }

        public abstract ScriptCommand CreateEmptyCopy();
        public static string GetKeyword() => throw new NotImplementedException();
        public static string GetPath() => throw new NotImplementedException();
        public static string GetDocumentation() => throw new NotImplementedException();
    }
}
