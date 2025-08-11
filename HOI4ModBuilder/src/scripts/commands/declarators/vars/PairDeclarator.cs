using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.declarators.vars
{
    public class PairDeclarator : VarDeclarator
    {
        private static readonly string _keyword = "PAIR";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.vars.collections." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{_keyword}:var_name> <KEY_TYPE> <VALUE_TYPE> [OPTIONAL]<{_keyword}<KEY_TYPE|VALUE_TYPE>:other_pair>",
            "======== OR ========",
            $"{_keyword} <{_keyword}:var_name> <KEY_TYPE> <VALUE_TYPE> [OPTIONAL]<KEY_TYPE>:key> [OPTIONAL]<VALUE_TYPE>:value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t<KEY_TYPE>",
            $"\t<VALUE_TYPE>",
            $"\t[OPTIONAL]<{_keyword}<KEY_TYPE,VALUE_TYPE>:other_pair>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{_keyword}:var_name>",
            $"\t<KEY_TYPE>",
            $"\t<VALUE_TYPE>",
            $"\t[OPTIONAL]<KEY_TYPE>:key>",
            $"\t[OPTIONAL]<VALUE_TYPE>:value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new PairDeclarator();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4 || a.Length == 5 || a.Length == 6,
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
                var keyType = args[argIndexKeyType];
                var keyTypeObj = ScriptFabricsRegister.ProduceNewScriptObject(lineIndex, args, keyType, argIndexKeyType);

                int argIndexValueType = 3;
                var valueType = args[argIndexValueType];
                var valueTypeObj = ScriptFabricsRegister.ProduceNewScriptObject(lineIndex, args, valueType, argIndexValueType);

                var pair = new PairObject(keyTypeObj, valueTypeObj);

                if (!varsScope.TryDeclareVar(name, pair))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, name, argIndexName);

                string optional1 = args.Length > 4 ? args[4] : null;
                string optional2 = args.Length > 5 ? args[5] : null;

                if (optional1 != null && optional2 == null)
                    pair.Set(lineIndex, args, varsScope.GetValue(optional1));
            };
        }
    }
}
