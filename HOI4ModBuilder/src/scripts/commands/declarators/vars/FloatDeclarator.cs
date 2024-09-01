using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class FloatDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "FLOAT";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.vars." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> [OPTIONAL]<{BooleanDeclarator.GetKeyword()}|INUMBER:other_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t[OPTIONAL]<{BooleanDeclarator.GetKeyword()}|INUMBER:other_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new FloatDeclarator();

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
                var name = args[1];
                var rawValue = args.Length > 2 ? args[2] : null;
                var obj = new FloatObject();

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args);

                if (rawValue != null)
                    obj.Set(lineIndex, args, ScriptParser.ParseValue(varsScope, rawValue));
            };
        }
    }
}
