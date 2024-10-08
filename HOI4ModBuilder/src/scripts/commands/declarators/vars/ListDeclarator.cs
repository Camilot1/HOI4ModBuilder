using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class ListDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "LIST";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.vars." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> <VALUE_TYPE> [OPTIONAL]<{_keyword}<VALUE_TYPE>:other_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t<VALUE_TYPE>",
            $"\t[OPTIONAL]<{_keyword}<VALUE_TYPE>:other_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new ListDeclarator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3 || a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var name = args[1];

                var valueType = ScriptParser.GetScriptObjectEmptyCopy(args[2]);
                if (valueType == null)
                    throw new InvalidValueTypeScriptException(lineIndex, args);

                var obj = new ListObject(valueType);

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args);

                string otherRawValue = args.Length > 3 ? args[3] : null;
                if (otherRawValue != null)
                    obj.Set(lineIndex, args, varsScope.GetValue(otherRawValue));
            };
        }
    }
}
