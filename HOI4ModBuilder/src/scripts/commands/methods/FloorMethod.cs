using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class FloorMethod : ScriptCommand
    {
        private static readonly string _keyword = "FLOOR";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.methods." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{FloatDeclarator.GetKeyword()}:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new FloorMethod();

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
                obj.Value = (float)Math.Floor(obj.Value);
            };
        }
    }
}
