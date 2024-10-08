using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.keywords
{
    public class ContinueKeyword : ScriptCommand
    {
        private static readonly string _keyword = "CONTINUE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.keywords." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword}"
        };
        public override ScriptCommand CreateEmptyCopy() => new ContinueKeyword();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            _varsScope = varsScope;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 1,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var result = varsScope.PutLocalVarInAllSpecificParentScopes(
                    _keyword, new BooleanObject(true), (type) => type == EnumVarsScopeType.FOR
                );

                if (!result)
                    throw new InvalidScopeTypeScriptException(lineIndex, args);
            };
        }
    }
}
