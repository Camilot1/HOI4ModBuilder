using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class IntDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "INT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.vars." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> [OPTIONAL]<{BooleanDeclarator.GetKeyword()}|INUMBER:other_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t[OPTIONAL]<{BooleanDeclarator.GetKeyword()}|INUMBER:other_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new IntDeclarator();

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
                var obj = new IntObject();

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, name, argIndexName);

                if (rawValue != null)
                    obj.Set(lineIndex, args, ScriptParser.ParseValue(varsScope, rawValue));
            };
        }
    }
}
