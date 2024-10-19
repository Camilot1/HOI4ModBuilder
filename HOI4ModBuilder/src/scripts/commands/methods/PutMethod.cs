using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.commands
{
    public class PutMethod : ScriptCommand
    {
        private static readonly string _keyword = "PUT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <IPUT<KEY_TYPE,VALUE_TYPE>:to> <KEY_TYPE:key> <VALUE_TYPE:value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <IPUT<KEY_TYPE,VALUE_TYPE>:to>",
            $"\t<KEY_TYPE:key>",
            $"\t<VALUE_TYPE:value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new PutMethod();

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
                var key = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => true
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is IScriptObject
                );

                var to = (IPutObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IPutObject oPut &&
                        oPut.GetKeyType().IsSameType(key) &&
                        oPut.GetValueType().IsSameType(value)
                );

                to.Put(lineIndex, args, key, value);
            };
        }
    }
}
