using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetValueMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_VALUE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISETVALUE<VALUE_TYPE>:to> <VALUE_TYPE:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\t<ISETVALUE<VALUE_TYPE>>:to>",
            $"\t<VALUE_TYPE:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetValueMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var to = (ISetValueObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ISetValueObject
                );

                var from = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o.IsSameType(to.GetValueType())
                );

                to.SetValue(lineIndex, args, from);
            };
        }
    }
}
