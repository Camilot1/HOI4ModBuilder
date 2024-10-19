using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.managers;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    public class GetProvinceCenterFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_PROVINCE_CENTER";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.provinces." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:x> <INUMBER:y> <INUMBER:province_id>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:x>",
            "\tOUT <INUMBER:y>",
            "\t<INUMBER:province_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetProvinceCenterFunc();

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
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexProvinceId = 3;
                var provinceId = ScriptParser.ParseValue(
                    varsScope, args[argIndexProvinceId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!ProvinceManager.TryGetProvince(Convert.ToUInt16(provinceId.GetValue()), out var province))
                    throw new ValueNotFoundScriptException(lineIndex, args, provinceId.GetValue(), argIndexProvinceId);

                x.Set(lineIndex, args, new FloatObject(province.center.x));
                y.Set(lineIndex, args, new FloatObject(province.center.y));
            };
        }
    }
}
