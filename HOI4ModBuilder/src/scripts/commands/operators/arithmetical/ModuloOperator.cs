using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.operators.arithmetical
{
    public class ModuloOperator : ScriptCommand
    {
        private static readonly string _keyword = "MODULO";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.operators.arithmetical." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <IMODULO:variable> <ANY:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ModuloOperator();

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
                var variable = (IModuloObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IModuloObject
                );

                var value = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => true
                );

                variable.Modulo(lineIndex, args, value);
            };
        }
    }
}
