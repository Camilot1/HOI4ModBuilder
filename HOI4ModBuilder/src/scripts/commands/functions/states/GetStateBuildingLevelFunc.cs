
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions
{
    public class GetStateBuildingLevelFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_STATE_BUILDING_LEVEL";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:level> <INUMBER:state_id> <ISTRING:building_name>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:level>",
            "\t<INUMBER:state_id>",
            "\t<ISTRING:building_name>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetStateBuildingLevelFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var buildingLevel = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexStateId = 2;
                var stateId = ScriptParser.ParseValue(
                    varsScope, args[argIndexStateId], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexBuildingName = 3;
                var buildingName = ScriptParser.ParseValue(
                    varsScope, args[argIndexBuildingName], lineIndex, args,
                    (o) => o is IScriptObject
                );

                if (!StateManager.TryGetState(Convert.ToUInt16(stateId.GetValue()), out State state))
                    throw new ValueNotFoundScriptException(lineIndex, args, stateId.GetValue(), argIndexStateId);

                if (!BuildingManager.TryGetBuilding(Convert.ToString(buildingName.GetValue()), out Building building))
                    throw new ValueNotFoundScriptException(lineIndex, args, buildingName.GetValue(), argIndexBuildingName);

                buildingLevel.Set(lineIndex, args, new IntObject((int)state.GetStateBuildingLevel(building)));
            };
        }
    }
}
