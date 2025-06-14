using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class GetValueMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_VALUE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <VALUE_TYPE:to> <IGETVALUE<VALUE_TYPE>:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <VALUE_TYPE>:to>",
            $"\t<IGETVALUE<VALUE_TYPE>:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetValueMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var from = (IGetValueObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IGetValueObject
                );

                var to = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o.IsSameType(from.GetValueType())
                );

                from.GetValue(lineIndex, args, to);
            };
        }
    }
}
