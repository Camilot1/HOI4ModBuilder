using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class FileDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "FILE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.vars." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> [OPTIONAL]<{StringDeclarator.GetKeyword()}|{_keyword}:other_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t[OPTIONAL]<{StringDeclarator.GetKeyword()}|{_keyword}:other_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new FileDeclarator();

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

                var rawValue = args.Length > 2 ? args[2] : null;
                var obj = new FileObject();

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, name, argIndexName);

                if (rawValue != null)
                    obj.Set(lineIndex, args, ScriptParser.ParseValue(varsScope, rawValue));
            };
        }
    }
}
