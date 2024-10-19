using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class MinMethod : ScriptCommand
    {
        private static readonly string _keyword = "MIN";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
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
        public override ScriptCommand CreateEmptyCopy() => new MinMethod();

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

                INumberObject minValue = null;


                for (int i = 2; i < args.Length; i++)
                {
                    var value = (INumberObject)ScriptParser.ParseValue(
                        varsScope, args[i], lineIndex, args,
                        (o) => o is INumberObject
                    );
                    var tempCheck = new BooleanObject();

                    if (minValue == null)
                        minValue = value;
                    else
                    {
                        value.IsLowerThan(lineIndex, args, minValue, tempCheck);
                        if (tempCheck.Value)
                            minValue = value;
                    }
                }

                result.Set(lineIndex, args, minValue);
            };
        }
    }
}
