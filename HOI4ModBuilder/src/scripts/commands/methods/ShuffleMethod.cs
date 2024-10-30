using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class ShuffleMethod : ScriptCommand
    {
        private static readonly string _keyword = "SHUFFLE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISHUFFLE:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ShuffleMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var variable = (IShuffleObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ISortObject
                );

                variable.Shuffle(lineIndex, args);
            };
        }
    }
}
