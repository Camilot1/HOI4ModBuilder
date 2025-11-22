using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.regions
{
    public class GetRegionAdjacentProvincesIdsFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_ADJACENT_PROVINCES_IDS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.regions." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ILIST<INUMBER>:adjacent_provinces_ids> <INUMBER:region_id>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ILIST<INUMBER>:adjacent_provinces_ids>",
            $"\t<INUMBER:region_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionAdjacentProvincesIdsFunc();

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
                var provincesIds = (IListObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IListObject obj && obj.GetValueType() is INumberObject
                );

                int argIndexRegionId = 2;
                var regionId = (INumberObject)ScriptParser.ParseValue(
                    varsScope, args[argIndexRegionId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGet(Convert.ToUInt16(regionId.GetValue()), out var region))
                    throw new ValueNotFoundScriptException(lineIndex, args, regionId.GetValue(), argIndexRegionId);

                var hashSetProvincesIds = new HashSet<ushort>();
                region.ForEachAdjacentProvince((thisProvince, otherProvince) => hashSetProvincesIds.Add(otherProvince.Id));

                var listProvincesIds = new List<ushort>(hashSetProvincesIds);
                listProvincesIds.Sort();

                foreach (var provinceId in listProvincesIds)
                    provincesIds.Add(lineIndex, args, new IntObject(provinceId));
            };
        }
    }
}
