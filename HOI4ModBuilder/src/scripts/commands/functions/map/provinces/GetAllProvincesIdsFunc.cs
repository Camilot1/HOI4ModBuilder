using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.managers;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    public class GetAllProvincesIdsFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_ALL_PROVINCES_IDS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.provinces." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ILIST<INUMBER>:provinces_ids>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ILIST<INUMBER>:provinces_ids>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetAllProvincesIdsFunc();

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
                var provincesIds = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && obj.GetValueType() is INumberObject
                );

                foreach (var provinceId in ProvinceManager.GetProvincesIds())
                    provincesIds.Add(lineIndex, args, new IntObject(provinceId));
            };
        }
    }
}


