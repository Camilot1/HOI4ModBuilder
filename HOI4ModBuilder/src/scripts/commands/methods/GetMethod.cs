using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class GetMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <VALUE_TYPE:to> <KEY_TYPE:key|index> <IGET<KEY_TYPE,VALUE_TYPE>:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <VALUE_TYPE:to>",
            $"\t<KEY_TYPE:key|index>",
            $"\t<IGET<KEY_TYPE,VALUE_TYPE>:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var to = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => true
                );

                var from = (IGetObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IGetObject @object &&
                            @object.GetValueType().IsSameType(to)
                );

                var key = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is IScriptObject
                );

                var getResult = from.GetValueType().GetEmptyCopy();
                from.Get(lineIndex, args, key, getResult);
                to.Set(lineIndex, args, getResult);
            };
        }
    }
}
