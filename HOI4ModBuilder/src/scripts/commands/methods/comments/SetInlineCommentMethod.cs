using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetInlineCommentMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_INLINE_COMMENT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods.comments." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword}<ICOMMENTED:to> <ISTRING:from> ",
            "======== OR ========",
            $"{_keyword} (",
            $"\t<ICOMMENTED:to>",
            $"\t<ISTRING:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetKeyMethod();

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
                var to = (ICommentedObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ICommentedObject
                );

                var from = (IStringObject)ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IStringObject
                );

                if (!from.GetString().StartsWith("#"))
                    throw new ScriptException(src.utils.EnumLocKey.SCRIPT_EXCEPTION_COMMENT_MUST_START_WITH_COMMENT_CHAR, lineIndex, args, from);

                to.SetInlineComment(lineIndex, args, from);
            };
        }
    }
}

