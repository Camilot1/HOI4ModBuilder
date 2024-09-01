using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class ClearMethod : ScriptCommand
    {
        private static readonly string _keyword = "CLEAR";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.methods." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <ICLEAR:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ClearMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            _varsScope = varsScope;
            _action = delegate ()
            {
                var obj = (IClearObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IClearObject
                );

                obj.Clear(lineIndex, args);
            };
        }
    }
}


