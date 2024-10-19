using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class TrimMethod : ScriptCommand
    {
        private static readonly string _keyword = "TRIM";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ITRIM:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new TrimMethod();

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
                var variable = (ITrimObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ITrimObject
                );

                variable.Trim(lineIndex, args);
            };
        }
    }
}

