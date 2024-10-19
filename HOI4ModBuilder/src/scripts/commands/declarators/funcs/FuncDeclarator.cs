
namespace HOI4ModBuilder.src.scripts.commands.declarators.funcs
{
    public class FuncDeclarator : ScopeScriptCommand
    {
        private static readonly string _keyword = "FUNC";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.funcs." + _keyword;

        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <PARAM_TYPE:param0> <PARAM_TYPE:param1> ... <PARAM_TYPE:paramN>",
        };

        public override ScriptCommand CreateEmptyCopy() => new FuncDeclarator();

        public override EnumVarsScopeType GetEnumVarsScopeType() => EnumVarsScopeType.FUNC;

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;

            index++;
            int innerIndent = indent + 1;
            var innerVarsScope = new VarsScope(varsScope, EnumVarsScopeType.FUNC);
            var commands = ScriptParser.Parse(lines, ref index, indent + 1, innerVarsScope);

            if (index < lines.Length)
            {
                var line = lines[index];
                var chainArgs = ScriptParser.GetStringArgs(lineIndex, line);
                if (chainArgs.Length > 0)
                {

                }
            }

            index--;

            _varsScope = varsScope;
            _action = delegate ()
            {
                innerVarsScope.ClearLocalVars();
                foreach (var command in commands)
                {
                    command.Execute();
                    if (CheckExitScope())
                        return;
                }
            };
        }

    }
}

