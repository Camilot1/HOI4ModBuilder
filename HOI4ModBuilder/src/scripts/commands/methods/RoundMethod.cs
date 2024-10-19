using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class RoundMethod : ScriptCommand
    {
        private static readonly string _keyword = "ROUND";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{FloatDeclarator.GetKeyword()}:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new RoundMethod();

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
                var variable = (FloatObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is FloatObject
                );

                variable.Value = (float)Math.Round(variable.Value);
            };
        }
    }
}
