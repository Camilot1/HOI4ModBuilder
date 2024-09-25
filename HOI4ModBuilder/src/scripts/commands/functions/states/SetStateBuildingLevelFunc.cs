
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions
{
    public class SetStateBuildingLevelFunc : ScriptCommand
    {
        private static readonly string _keyword = "SET_STATE_BUILDING_LEVEL";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.states." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <INUMBER:state_id> <ISTRING:building_name> <INUMBER:building_level>",
            "======== OR ========",
            $"{_keyword} (",
            "\t<INUMBER:state_id>",
            "\t<ISTRING:building_name>",
            "\t<INUMBER:building_level>",
            ")"
        };

        public override ScriptCommand CreateEmptyCopy() => new SetStateBuildingLevelFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var buildingName = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IStringObject
                );
                var buildingLevel = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StateManager.TryGetState(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args);

                if (!BuildingManager.TryGetBuilding((string)buildingName.GetValue(), out Building building))
                    throw new ValueNotFoundScriptException(lineIndex, args);

                state.SetStateBuildingLevel(building, Convert.ToUInt32(buildingLevel.GetValue()));
            };
        }
    }
}
