using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    public class GetProvincePixelsCountFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_PROVINCE_PIXELS_COUNT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.provinces." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:pixels_count> <INUMBER:province_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:pixels_count>",
            "\t<INUMBER:province_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetProvincePixelsCountFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var pixelsCount = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexProvinceId = 2;
                var provinceId = ScriptParser.ParseValue(
                    varsScope, args[argIndexProvinceId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!ProvinceManager.TryGetProvince(Convert.ToUInt16(provinceId.GetValue()), out Province province))
                    throw new ValueNotFoundScriptException(lineIndex, args, provinceId.GetValue(), argIndexProvinceId);

                pixelsCount.Set(lineIndex, args, new IntObject(province.pixelsCount));
            };
        }
    }
}