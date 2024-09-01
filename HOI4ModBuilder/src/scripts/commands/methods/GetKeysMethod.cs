using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.commands.methods;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions
{
    public class GetKeysMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_KEYS";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.methods." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{ListDeclarator.GetKeyword()}<VALUE_TYPE>:to_list> <{MapDeclarator.GetKeyword()}<KEY_TYPE>:from_map>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{ListDeclarator.GetKeyword()}<VALUE_TYPE>:to_list>",
            $"\t<{MapDeclarator.GetKeyword()}<KEY_TYPE>:from_map>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetKeysMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var from = (IMapObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IMapObject
                );
                var keysType = from.GetKeyType();

                var to = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && obj.GetValueType().IsSameType(keysType)
                );

                from.GetKeys(lineIndex, args, to);
            };
        }
    }
}
