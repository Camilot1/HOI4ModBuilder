using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.commands.functions;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class MapDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "MAP";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.vars." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> <KEY_VALUE> <VALUE_TYPE> [OPTIONAL]<{_keyword}<KEY_TYPE,VALUE_TYPE>:other_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t<KEY_VALUE>",
            $"\t<VALUE_TYPE>",
            $"\t[OPTIONAL]<{_keyword}<KEY_TYPE,VALUE_TYPE>:other_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new MapDeclarator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4 || a.Length == 5,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var name = args[1];

                var keyType = ScriptParser.GetScriptObjectEmptyCopy(args[2]);
                if (keyType == null)
                    throw new InvalidKeyTypeScriptException(lineIndex, args);

                var valueType = ScriptParser.GetScriptObjectEmptyCopy(args[3]);
                if (valueType == null)
                    throw new InvalidValueTypeScriptException(lineIndex, args);

                var obj = new MapObject(keyType, valueType);

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args);

                string otherRawValue = args.Length > 4 ? args[4] : null;
                if (otherRawValue != null)
                    obj.Set(lineIndex, args, varsScope.GetValue(otherRawValue));
            };
        }
    }
}
