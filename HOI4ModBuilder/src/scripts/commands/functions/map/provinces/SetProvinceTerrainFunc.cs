using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.hoiDataObjects.common.terrain;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.provinces
{
    internal class SetProvinceTerrainFunc : ScriptCommand
    {
        private static readonly string _keyword = "SET_PROVINCE_TERRAIN";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.provinces." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:province_id> <ISTRING:terrain_name>",
            "======== OR ========",
            $"{_keyword} (",
            "\t<INUMBER:province_id>",
            "\t<ISTRING:terrain_name>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new SetProvinceTerrainFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                int argIndexProvinceId = 1;
                var provinceId = ScriptParser.ParseValue(
                    varsScope, args[argIndexProvinceId], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexTerrainName = 2;
                var terrainName = ScriptParser.GetValue(
                    varsScope, args[argIndexTerrainName], lineIndex, args,
                    (o) => o is IStringObject
                );

                if (!ProvinceManager.TryGetProvince(Convert.ToUInt16(provinceId.GetValue()), out Province province))
                    throw new ValueNotFoundScriptException(lineIndex, args, provinceId.GetValue(), argIndexProvinceId);

                if (!TerrainManager.TryGetProvincialTerrain(Convert.ToString(terrainName.GetValue()), out var terrain))
                    throw new ValueNotFoundScriptException(lineIndex, args, terrainName.GetValue(), argIndexTerrainName);

                province.Terrain = terrain;
            };
        }
    }
}
