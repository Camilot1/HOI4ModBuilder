using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.exceptions;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetPrevCommentsMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_PREV_COMMENTS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods.comments." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ICOMMENTED:to> <LIST<ANY|ISTRING>:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\t<ICOMMENTED:to>",
            $"\t<LIST<ANY|ISTRING>:from>",
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

                var from = (IListObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IListObject

                );

                from.ForEach(comment =>
                {
                    if (!(comment is IStringObject stringObject))
                        throw new InvalidValueTypeScriptException(lineIndex, args, comment);

                    if (!stringObject.GetString().StartsWith("#"))
                        throw new ScriptException(src.utils.EnumLocKey.SCRIPT_EXCEPTION_COMMENT_MUST_START_WITH_COMMENT_CHAR, lineIndex, args, stringObject);
                });

                to.SetPrevComments(lineIndex, args, from);
            };
        }
    }
}

