using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.commands.operators.arithmetical
{
    public class DivideOperator : ScriptCommand
    {
        private static readonly string _keyword = "DIVIDE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.operators.arithmetical." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <IDIVIDE:variable> <ANY:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new DivideOperator();

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
                var variable = (IDivideObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IDivideObject
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => true
                );

                variable.Divide(lineIndex, args, value);
            };
        }
    }
}
