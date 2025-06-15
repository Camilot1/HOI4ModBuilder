using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.newParser.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class ReadMethod : ScriptCommand
    {
        private static readonly string _keyword = "READ";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ANY|ILIST<ANY>:to> <IREAD:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ANY|ILIST<ANY>:to>",
            $"\t<IREAD:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new ReadMethod();

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
                var to = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IStringObject ||
                    o is IListObject listObj && listObj.GetValueType() is IStringObject
                );

                var from = (IReadObject)ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IReadObject
                );

                if (to is IListObject @list)
                    from.ReadRange(lineIndex, args, @list);
                else if (to is ISetObject setObject)
                    from.Read(lineIndex, args, setObject);
            };
        }
    }
}
