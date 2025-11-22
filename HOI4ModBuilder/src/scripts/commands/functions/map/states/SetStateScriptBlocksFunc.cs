using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.states
{
    public class SetStateScriptBlocksFunc : ScriptCommand
    {
        private static readonly string _keyword = "SET_STATE_SCRIPT_BLOCKS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:state_id> <LIST<ANY>:blocks> [OPTIONAL]<ISTRING:history_date>",
            "======== OR ========",
            $"{_keyword} (",
            "\t<INUMBER:state_id>",
            "\t<LIST<ANY>:blocks>",
            "\t[OPTIONAL]<ISTRING:history_date>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new SetStateScriptBlocksFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3 || a.Length == 4,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                int argIndexStateId = 1;
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGet(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                int argIndexList = 2;
                var list = (IListObject)ScriptParser.GetValue(
                    varsScope, args[argIndexList], lineIndex, args,
                    (o) => o is IListObject
                );

                if (!(list.GetValueType() is AnyObject) &&
                    !(list.GetValueType() is IPairObject))
                    throw new InvalidValueTypeScriptException(lineIndex, args, list.GetValueType());

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

                try
                {
                    state.SetHistoryScriptBlocks(rawHistoryDate, list);
                }
                catch (Exception ex)
                {
                    throw new InternalScriptException(lineIndex, args, ex);
                }
            };
        }
    }
}
