using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class ClampMethod : ScriptCommand
    {
        private static readonly string _keyword = "CLAMP";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:var_name> <INUMBER:min> <INUMBER:max>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ClampMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            _action = delegate ()
            {
                var variable = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var min = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );
                var max = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is INumberObject
                );

                var newValue = (float)Utils.Clamp(
                    Convert.ToDouble(variable.GetValue()),
                    Convert.ToDouble(min.GetValue()),
                    Convert.ToDouble(max.GetValue())
                );
                variable.Set(lineIndex, args, new FloatObject(newValue));
            };
        }
    }
}
