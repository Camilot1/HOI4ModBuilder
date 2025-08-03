using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class GetInlineCommentMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_INLINE_COMMENT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods.comments." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISTRING:to> <ICOMMENTED:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ISTRING:to>",
            $"\t<ICOMMENTED:from>",
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
                var to = (IStringObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IStringObject
                );

                var from = (ICommentedObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is ICommentedObject
                );

                from.GetInlineComment(lineIndex, args, to);
            };
        }
    }
}

