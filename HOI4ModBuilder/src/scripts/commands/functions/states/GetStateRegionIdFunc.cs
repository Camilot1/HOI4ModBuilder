using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.hoiDataObjects.map;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.states
{
    public class GetStateRegionIdFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_REGION_ID";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:region_id> <INUMBER:state_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:region_id>",
            "\t<INUMBER:state_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStateRegionIdFunc();

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
                var stateRegionId = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGetState(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args);

                state.TryGetRegionId(out var regionId);
                stateRegionId.Set(lineIndex, args, new IntObject(regionId));
            };
        }
    }
}
