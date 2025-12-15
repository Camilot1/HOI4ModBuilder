using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    public class GetMaxProvinceIdFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_MAX_PROVINCE_ID";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.provinces." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:max_province_id>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:max_province_id>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetMaxProvinceIdFunc();

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
                var maxProvinceId = (INumberObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject obj
                );

                maxProvinceId.Set(lineIndex, args, new IntObject(ProvinceManager.NextVacantProvinceId - 1));
            };
        }
    }
}
