using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.regions.weather
{
    public class GetRegionWeatherPeriodBlizzardFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_WEATHER_PERIOD_BLIZZARD";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.regions.weather." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <INUMBER:blizzard_chance> <INUMBER:<region_id> <INUMBER:weather_period_index>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:blizzard_chance>",
            "\t<INUMBER:<region_id>",
            "\t<INUMBER:weather_period_index>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionWeatherPeriodBlizzardFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var blizzardChance = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );
                var weatherPeriodIndex = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out var region))
                    throw new ValueNotFoundScriptException(lineIndex, args);

                if (!region.TryGetWeatherPeriod(Convert.ToInt32(weatherPeriodIndex.GetValue()), out var period))
                    throw new IndexOutOfRangeScriptException(lineIndex, args);

                blizzardChance.Set(lineIndex, args, new FloatObject(period.Blizzard));
            };
        }
    }
}

