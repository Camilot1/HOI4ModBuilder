using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    public class GetProvinceRegionIdFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_PROVINCE_REGION_ID";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.states." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:region_d> <INUMBER:province_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:region_d>",
            "\t<INUMBER:province_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetProvinceRegionIdFunc();

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
                var provinceRegionId = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexProvinceId = 2;
                var provinceId = ScriptParser.ParseValue(
                    varsScope, args[argIndexProvinceId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!ProvinceManager.TryGet(Convert.ToUInt16(provinceId.GetValue()), out var province))
                    throw new ValueNotFoundScriptException(lineIndex, args, provinceId.GetValue(), argIndexProvinceId);

                var regionId = province.Region != null ? province.Region.Id : 0;
                provinceRegionId.Set(lineIndex, args, new IntObject(regionId));
            };
        }
    }
}

