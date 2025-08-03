using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetKeyMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_KEY";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISETKEY<KEY_TYPE>:to> <KEY_TYPE:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\t<ISETKEY<KEY_TYPE>>:to>",
            $"\t<KEY_TYPE:from>",
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
                var to = (ISetKeyObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ISetKeyObject
                );

                var from = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o.IsSameType(to.GetKeyType())
                );

                to.SetKey(lineIndex, args, from);
            };
        }
    }
}
