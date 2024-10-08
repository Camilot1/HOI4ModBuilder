using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.functions
{
    public class GetKeysMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_KEYS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ILIST<VALUE_TYPE>:to_list> <IMAP<KEY_TYPE>:from_map>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ILIST<VALUE_TYPE>:to_list>",
            $"\t<IMAP<KEY_TYPE>:from_map>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetKeysMethod();

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
                var from = (IMapObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IMapObject
                );

                var to = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && 
                    obj.GetValueType().IsSameType(from.GetKeyType())
                );

                from.GetKeys(lineIndex, args, to);
            };
        }
    }
}
