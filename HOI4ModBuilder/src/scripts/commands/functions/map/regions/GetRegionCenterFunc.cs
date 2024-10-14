using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.regions
{
    public class GetRegionCenterFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_CENTER";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.regions." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:x> <INUMBER:y> <INUMBER:region_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:x>",
            "\tOUT <INUMBER:y>",
            "\t<INUMBER:region_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionCenterFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true, true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var x = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                var y = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexRegionId = 3;
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[argIndexRegionId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out var region))
                    throw new ValueNotFoundScriptException(lineIndex, args, regionId.GetValue(), argIndexRegionId);

                x.Set(lineIndex, args, new FloatObject(region.center.x));
                y.Set(lineIndex, args, new FloatObject(region.center.y));
            };
        }
    }
}
