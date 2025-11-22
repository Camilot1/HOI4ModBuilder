using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.src.hoiDataObjects.history.states;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.states
{
    public class GetStatePixelsCountFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_PIXELS_COUNT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:pixels_count> <INUMBER:state_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:pixels_count>",
            "\t<INUMBER:state_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStatePixelsCountFunc();

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
                var pixelsCount = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexStateId = 2;
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGet(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                int sumPixelsCount = 0;

                foreach (var province in state.Provinces)
                    sumPixelsCount += province.pixelsCount;

                pixelsCount.Set(lineIndex, args, new IntObject(sumPixelsCount));
            };
        }
    }
}
