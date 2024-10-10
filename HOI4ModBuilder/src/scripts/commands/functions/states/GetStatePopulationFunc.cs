using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions
{
    public class GetStatePopulationFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_POPULATION";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:population> <INUMBER:state_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:population>",
            "\t<INUMBER:state_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStatePopulationFunc();

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
                var population = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexStateId = 2;
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGetState(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                population.Set(lineIndex, args, new IntObject(state.manpower));
            };
        }
    }
}
