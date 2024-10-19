using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class FormatMethod : ScriptCommand
    {
        private static readonly string _keyword = "FORMAT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"Formats a number to a string value by using pattern line like \"0.000\"",
            $"{_keyword} <ISTRING:result> <INUMBER:value> <ISTRING:pattern>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ISTRING:result>",
            $"\t<INUMBER:value>",
            $"\t<ISTRING:pattern>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new FormatMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var result = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IStringObject
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                var pattern = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is IStringObject
                );

                var valueObj = Convert.ToDouble(value.GetValue());
                var patternObj = Convert.ToString(pattern.GetValue());
                var resultObj = valueObj.ToString(patternObj, System.Globalization.CultureInfo.InvariantCulture);

                result.Set(lineIndex, args, new StringObject(resultObj));
            };
        }
    }
}

