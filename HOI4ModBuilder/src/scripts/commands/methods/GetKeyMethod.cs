using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class GetKeyMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_KEY";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <KEY_TYPE:to> <IGETKEY<KEY_TYPE>:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <KEY_TYPE>:to>",
            $"\t<IGETKEY<KEY_TYPE>:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetKeyMethod();

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
                var from = (IGetKeyObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IGetKeyObject
                );

                var to = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o.IsSameType(from.GetKeyType())
                );

                from.GetKey(lineIndex, args, to);
            };
        }
    }
}
