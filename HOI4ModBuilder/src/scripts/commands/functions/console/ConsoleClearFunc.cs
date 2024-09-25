using HOI4ModBuilder.src.forms.scripts;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.functions.console
{
    public class ConsoleClearFunc : ScriptCommand
    {
        private static readonly string _keyword = "CONSOLE_CLEAR";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.console." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <ISCRIPTOBJECT>:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ConsoleClearFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var value = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IScriptObject
                );

                ScriptsForm.instance?.ClearConsole();
            };
        }
    }
}
