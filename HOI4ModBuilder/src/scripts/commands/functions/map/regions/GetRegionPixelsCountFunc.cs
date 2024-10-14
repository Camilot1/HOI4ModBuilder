using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.regions
{
    public class GetRegionPixelsCountFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_PIXELS_COUNT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.regions." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:pixels_count> <INUMBER:region_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:pixels_count>",
            "\t<INUMBER:region_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionPixelsCountFunc();

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

                int argIndexRegionId = 2;
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[argIndexRegionId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out var region))
                    throw new ValueNotFoundScriptException(lineIndex, args, regionId.GetValue(), argIndexRegionId);

                int[] sumPixelsCount = new int[1];

                region.ForEachProvince(p => sumPixelsCount[0] += p.pixelsCount);

                pixelsCount.Set(lineIndex, args, new IntObject(sumPixelsCount[0]));
            };
        }
    }
}
