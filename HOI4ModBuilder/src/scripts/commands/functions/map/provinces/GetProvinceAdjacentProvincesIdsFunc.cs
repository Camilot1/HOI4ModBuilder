using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.src.scripts.exceptions;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    public class GetProvinceAdjacentProvincesIdsFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_PROVINCE_ADJACENT_PROVINCES_IDS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.provinces." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ILIST<INUMBER>:adjacent_provinces_ids> <INUMBER:province_id>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ILIST<INUMBER>:adjacent_provinces_ids>",
            $"\t<INUMBER:province_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetProvinceAdjacentProvincesIdsFunc();

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

                int argIndexProvinceId = 2;
                var provinceId = (INumberObject)ScriptParser.ParseValue(
                    varsScope, args[argIndexProvinceId], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!ProvinceManager.TryGetProvince(Convert.ToUInt16(provinceId.GetValue()), out var province))
                    throw new ValueNotFoundScriptException(lineIndex, args, provinceId.GetValue(), argIndexProvinceId);

                province.ForEachAdjacentProvince((thisProvince, otherProvince) => provincesIds.Add(lineIndex, args, new IntObject(otherProvince.Id)));
            };
        }
    }
}