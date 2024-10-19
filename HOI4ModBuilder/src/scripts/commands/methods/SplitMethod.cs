using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SplitMethod : ScriptCommand
    {
        private static readonly string _keyword = "SPLIT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ILIST<ISTRING>:result> <ISPLIT:source> <CHAR:regex>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ILIST<ISTRING>:result>",
            $"\t<ISPLIT:source>",
            $"\t<CHAR:regex>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new SplitMethod();

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
                var result = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject listObject && listObject.GetValueType() is IStringObject
                );

                var source = (ISplitObject)ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is ISplitObject
                );

                var regex = (CharObject)ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is CharObject
                );

                source.Split(lineIndex, args, regex, result);
            };
        }
    }
}


