using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.newParser;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.states
{
    public class GetStateScriptBlocksFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_SCRIPT_BLOCKS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <LIST<ANY>:blocks> <INUMBER:state_id> [OPTIONAL]<ISTRING:history_date>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <LIST<ANY>:blocks>",
            "\t<INUMBER:state_id>",
            "\t[OPTIONAL]<ISTRING:history_date>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStateBuildingLevelFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3 || a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var list = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject
                );

                int argIndexStateId = 2;
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGetState(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                var argIndexHistoryDate = 3;
                DateTime rawHistoryDate = default;
                try
                {
                    rawHistoryDate = args.Length >= 4 ? ParserUtils.Parse<DateTime>(args[argIndexHistoryDate]) : default;
                }
                catch (Exception ex)
                {
                    throw new InvalidValueTypeScriptException(lineIndex, args, ex.Message, argIndexHistoryDate);
                }

                list.Set(lineIndex, args, state.GetHistoryScriptBlocks(rawHistoryDate));
            };
        }
    }
}
