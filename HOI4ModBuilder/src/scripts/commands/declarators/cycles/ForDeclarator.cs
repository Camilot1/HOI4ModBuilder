using HOI4ModBuilder.src.scripts.commands.keywords;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class ForDeclarator : ScopeScriptCommand
    {
        private static readonly string _for_keyword = "FOR";
        private static readonly string _in_keyword = "IN";
        public static new string GetKeyword() => _for_keyword;
        public static new string GetPath() => "commands.declarators.vars.cycles." + _for_keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            _for_keyword + " <iterator_name> IN <LIST:values>",
            "\t<INNER_CODE>"
        };
        public override ScriptCommand CreateEmptyCopy() => new ForDeclarator();

        public override EnumVarsScopeType GetEnumVarsScopeType() => EnumVarsScopeType.FOR;

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            if (args.Length != 4)
                throw new InvalidArgsCountScriptException(lineIndex, args);

            if (args[2] != _in_keyword)
                throw new InvalidCommandArgsScriptException(lineIndex, args);

            index++;
            var innerVarsScope = new VarsScope(varsScope, EnumVarsScopeType.FOR);
            var commands = ScriptParser.Parse(lines, ref index, indent + 1, innerVarsScope);

            index--;

            _varsScope = varsScope;
            _action = delegate ()
            {
                var iterator = args[1];
                if (varsScope.HasLocalVar(iterator))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args);

                var list = ScriptParser.ParseValue(innerVarsScope, args[3]);
                if (!(list is IListObject))
                    throw new InvalidValueTypeScriptException(lineIndex, args);

                ((IListObject)list).ForEach(item =>
                {
                    if (CheckExitScope())
                        return;

                    innerVarsScope.ClearLocalVars();
                    innerVarsScope.PutLocalVariable(iterator, item);

                    foreach (var command in commands)
                    {
                        command.Execute();
                        if (CheckExitScope())
                            break;
                    }
                });
            };
        }
    }
}
