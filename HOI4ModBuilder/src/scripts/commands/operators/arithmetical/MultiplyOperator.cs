using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.operators.arithmetical
{
    public class MultiplyOperator : ScriptCommand
    {
        private static readonly string _keyword = "MULTIPLY";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.operators.arithmetical." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <IMULTIPLY:variable> <ANY:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new MultiplyOperator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var variable = (IMultiplyObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IMultiplyObject
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => true
                );

                variable.Multiply(lineIndex, args, value);
            };
        }
    }
}
