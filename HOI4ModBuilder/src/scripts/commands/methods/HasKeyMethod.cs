using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class HasKeyMethod : ScriptCommand
    {
        private static readonly string _keyword = "HAS_KEY";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <BOOLEAN:to> <IMAP<KEY_TYPE,VALUE_TYPE>:from> <KEY_TYPE:key>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <BOOLEAN:to>",
            $"\t<IMAP<KEY_TYPE,VALUE_TYPE>:from>",
            $"\t<KEY_TYPE:key>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new HasKeyMethod();

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
                var to = (BooleanObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is BooleanObject
                );

                var key = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is IScriptObject
                );

                var from = (IMapObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IMapObject oMap && oMap.GetKeyType().IsSameType(key)
                );

                var checkResult = new BooleanObject();
                from.HasKey(lineIndex, args, key, checkResult);
                to.Set(lineIndex, args, checkResult);
            };
        }
    }
}
