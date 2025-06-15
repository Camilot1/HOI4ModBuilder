using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.primitives;

namespace HOI4ModBuilder.src.scripts.commands.declarators.vars
{
    public class ColorDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "COLOR";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.vars.primitives." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            "If other_value is a STRING definiton, it should be marked like " +
            "\"COLOR(a=255;r=255;g=255;b=255)\" or \"COLOR(alpha=255;red=255;green=255;blue=255)\" " +
            "or any other combinations of those parameters",
            $"{_keyword} <{_keyword}:var_name> [OPTIONAL]<ISTRING|ICOLOR:other_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t[OPTIONAL]<ISTRING|ICOLOR:other_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new CharDeclarator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2 || a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
            lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                int argIndexName = 1;
                var name = args[argIndexName];

                var rawValue = args.Length > 2 ? args[2] : null;
                var obj = new ColorObject();

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, name, argIndexName);

                if (rawValue != null)
                    obj.Set(lineIndex, args, ScriptParser.ParseValue(varsScope, rawValue));
            };
        }
    }
}
