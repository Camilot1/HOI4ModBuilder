using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class CeilMethod : ScriptCommand
    {
        private static readonly string _keyword = "CEIL";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{FloatDeclarator.GetKeyword()}:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new CeilMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            _varsScope = varsScope;
            _action = delegate ()
            {
                var obj = (FloatObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is FloatObject
                );

                obj.Value = (float)Math.Ceiling(obj.Value);
            };
        }
    }
}
