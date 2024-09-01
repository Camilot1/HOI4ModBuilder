using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.utils;

namespace HOI4ModBuilder.src.scripts.commands.functions.time
{
    public class GetDaysSinceYearStartFunction : ScriptCommand
    {
        private static readonly string _keyword = "GET_DAYS_SINCE_YEAR_START";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.time." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <INUMBER:days_since_year_start> <INUMBER:day_index> <INUMBER:month_index>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:days_since_year_start>",
            "\t<INUMBER:day_index>",
            "\t<INUMBER:month_index>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetDaysSinceYearStartFunction();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var daysSinceYearStart = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var dayIndex = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );
                var monthIndex = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is INumberObject
                );

                int value = ScriptUtils.GetDaysSinceYearStart(
                    lineIndex, args, (int)dayIndex.GetValue(), (int)monthIndex.GetValue()
                );
                daysSinceYearStart.Set(lineIndex, args, new IntObject(value));
            };
        }
    }
}

