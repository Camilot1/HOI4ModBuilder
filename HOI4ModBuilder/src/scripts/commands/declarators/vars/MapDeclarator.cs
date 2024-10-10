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
        public override string GetPath() => "commands.declarators.vars." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
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
                int argIndexName = 1;
                var name = args[argIndexName];

                int argIndexKeyType = 2;
                var keyTypeRaw = args[argIndexKeyType];
                var keyType = ScriptParser.GetScriptObjectEmptyCopy(keyTypeRaw);
                if (keyType == null)
                    throw new InvalidKeyTypeScriptException(lineIndex, args, keyTypeRaw, argIndexKeyType);

                int argIndexValueType = 3;
                var valueTypeRaw = args[argIndexValueType];
                var valueType = ScriptParser.GetScriptObjectEmptyCopy(valueTypeRaw);
                if (valueType == null)
                    throw new InvalidValueTypeScriptException(lineIndex, args, valueTypeRaw, argIndexValueType);

                var obj = new MapObject(keyType, valueType);

                if (!varsScope.TryDeclareVar(name, obj))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, name, argIndexName);

                string otherRawValue = args.Length > 4 ? args[4] : null;
                if (otherRawValue != null)
                    obj.Set(lineIndex, args, varsScope.GetValue(otherRawValue));
            };
        }
    }
}
