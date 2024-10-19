using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class AppendMethod : ScriptCommand
    {
        private static readonly string _keyword = "APPEND";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <IAPPEND:to> <ANY:from>"
        };
        public override ScriptCommand CreateEmptyCopy() => new AppendMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var variable = (IAppendObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IAppendObject
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => true
                );

                if (value is ICollectionObject collectionObj)
                    variable.AppendRange(lineIndex, args, collectionObj);
                else
                    variable.Append(lineIndex, args, value);
            };
        }
    }
}

