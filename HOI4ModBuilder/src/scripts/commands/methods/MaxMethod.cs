using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class MaxMethod : ScriptCommand
    {
        private static readonly string _keyword = "MAX";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.methods." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <INUMBER:result> <INUMBER:value0> <INUMBER:value1> ... <INUMBER:valueN>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:result>",
            $"\t<INUMBER:value0>",
            $"\t<INUMBER:value1>",
            $"\t...",
            $"\t<INUMBER:valueN>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new MaxMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length >= 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var result = (INumberObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                INumberObject maxValue = null;


                for (int i = 2; i < args.Length; i++)
                {
                    var value = (INumberObject)ScriptParser.ParseValue(
                        varsScope, args[i], lineIndex, args,
                        (o) => o is INumberObject
                    );
                    var tempCheck = new BooleanObject();

                    if (maxValue == null)
                        maxValue = value;
                    else
                    {
                        value.IsGreaterThan(lineIndex, args, maxValue, tempCheck);
                        if (tempCheck.Value)
                            maxValue = value;
                    }
                }

                result.Set(lineIndex, args, maxValue);
            };
        }
    }
}
