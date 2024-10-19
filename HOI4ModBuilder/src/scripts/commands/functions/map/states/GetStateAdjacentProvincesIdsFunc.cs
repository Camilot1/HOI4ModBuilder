using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.states
{
    public class GetStateAdjacentProvincesIdsFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_ADJACENT_PROVINCES_IDS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ILIST<INUMBER>:adjacent_provinces_ids> <INUMBER:state_id>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ILIST<INUMBER>:adjacent_provinces_ids>",
            $"\t<INUMBER:state_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStateAdjacentProvincesIdsFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var provincesIds = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && obj.GetValueType() is INumberObject
                );

                int argIndexStateId = 2;
                var stateId = (INumberObject)ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGetState(Convert.ToUInt16(stateId.GetValue()), out var state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                state.ForEachAdjacentProvince((thisProvince, otherProvince) => provincesIds.Add(lineIndex, args, new IntObject(otherProvince.Id)));
            };
        }
    }
}

