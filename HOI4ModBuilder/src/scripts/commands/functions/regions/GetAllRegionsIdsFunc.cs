using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using System;
using HOI4ModBuilder.src.scripts.commands.declarators;

namespace HOI4ModBuilder.src.scripts.commands.functions.regions
{
    public class GetAllRegionsIdsFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_ALL_REGIONS_IDS";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.regions." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{ListDeclarator.GetKeyword()}<INUMBER>:regions_ids>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{ListDeclarator.GetKeyword()}<INUMBER>>:regions_ids>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetAllRegionsIdsFunc();

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
                var regionsIds = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && obj.GetValueType() is INumberObject
                );

                foreach (var regionId in StrategicRegionManager.GetRegionsIds())
                    regionsIds.Add(lineIndex, args, new IntObject(regionId));
            };
        }
    }
}

