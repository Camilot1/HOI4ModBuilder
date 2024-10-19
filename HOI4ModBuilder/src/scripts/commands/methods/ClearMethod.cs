using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class ClearMethod : ScriptCommand
    {
        private static readonly string _keyword = "CLEAR";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ICLEAR:var_name>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ClearMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            _action = delegate ()
            {
                var obj = (IClearObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IClearObject
                );

                obj.Clear(lineIndex, args);
            };
        }
    }
}


