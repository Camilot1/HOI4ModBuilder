using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.forms.scripts;

namespace HOI4ModBuilder.src.scripts.commands.functions.console
{
    public class ConsoleWriteFunc : ScriptCommand
    {
        private static readonly string _keyword = "CONSOLE_WRITE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.console." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISCRIPTOBJECT:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ConsoleWriteFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var value = ScriptParser.ParseValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IScriptObject
                );

                ScriptsForm.instance?.PrintToConsole(ScriptParser.FormatToString(value));
            };
        }
    }
}
