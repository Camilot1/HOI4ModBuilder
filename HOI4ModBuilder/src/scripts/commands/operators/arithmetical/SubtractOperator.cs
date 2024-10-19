using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.operators.arithmetical
{
    public class SubtractOperator : ScriptCommand
    {
        private static readonly string _keyword = "SUBTRACT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.operators.arithmetical." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISUBTRACT:variable> <ANY:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new SubtractOperator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var variable = (ISubtractObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ISubtractObject
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => true
                );

                variable.Subtract(lineIndex, args, value);
            };
        }
    }
}
