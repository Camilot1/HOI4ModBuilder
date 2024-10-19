using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.declarators.vars
{
    public class RandomDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "RANDOM";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.vars." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> [OPTIONAL]<INUMBER:seed>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t[OPTIONAL]<INUMBER:seed>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new RandomDeclarator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2 || a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                int argIndexName = 1;
                var name = args[argIndexName];

                var rawSeed = args.Length > 2 ? args[2] : null;
                var obj = new RandomObject();

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, name, argIndexName);

                if (rawSeed != null)
                    obj.Set(lineIndex, args, ScriptParser.ParseValue(varsScope, rawSeed));
            };
        }
    }
}

