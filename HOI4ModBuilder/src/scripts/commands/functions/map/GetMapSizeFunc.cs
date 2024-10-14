using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.scripts.commands.functions.map.provinces;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map
{
    public class GetMapSizeFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_MAP_SIZE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:x> <INUMBER:y>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:x>",
            "\tOUT <INUMBER:y>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetProvinceCenterFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
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

                x.Set(lineIndex, args, new FloatObject(MapManager.MapSize.x));
                y.Set(lineIndex, args, new FloatObject(MapManager.MapSize.y));
            };
        }
    }
}

