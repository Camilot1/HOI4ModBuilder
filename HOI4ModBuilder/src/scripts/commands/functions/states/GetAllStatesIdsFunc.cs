using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.commands.functions
{
    public class GetAllStatesIdsFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_ALL_STATES_IDS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <{ListDeclarator.GetKeyword()}<INUMBER>:states_ids>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{ListDeclarator.GetKeyword()}<INUMBER>:states_ids>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetAllStatesIdsFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var statesIds = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && obj.GetValueType() is INumberObject
                );

                foreach (var stateId in StateManager.GetStatesIds())
                    statesIds.Add(lineIndex, args, new IntObject(stateId));
            };
        }
    }
}
