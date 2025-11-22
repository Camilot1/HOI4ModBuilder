
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.states
{
    public class GetStateCategoryFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_CATEGORY";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISTRING:state_category> <INUMBER:state_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <ISTRING:state_category>",
            "\t<INUMBER:state_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStateCategoryFunc();

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
                var stateCategory = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IStringObject
                );

                int argIndexStateId = 2;
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGet(Convert.ToUInt16(stateId.GetValue()), out var state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                stateCategory.Set(lineIndex, args, new StringObject(state.CurrentStateCategory?.name));
            };
        }
    }
}
