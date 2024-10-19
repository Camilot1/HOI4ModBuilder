using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.regions.weather
{
    internal class GetRegionWeatherPeriodMinSnowLevelFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_WEATHER_PERIOD_MIN_SNOW_LEVEL";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.regions.weather." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:min_snow_level> <INUMBER:region_id> <INUMBER:weather_period_index>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:min_snow_level>",
            "\t<INUMBER:region_id>",
            "\t<INUMBER:weather_period_index>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionWeatherPeriodMinSnowLevelFunc();

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
                var minSnowLevel = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexRegionId = 2;
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[argIndexRegionId], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexWeatherReriodIndex = 3;
                var weatherPeriodIndex = ScriptParser.ParseValue(
                    varsScope, args[argIndexWeatherReriodIndex], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out var region))
                    throw new ValueNotFoundScriptException(lineIndex, args, regionId.GetValue(), argIndexRegionId);

                if (!region.TryGetWeatherPeriod(Convert.ToInt32(weatherPeriodIndex.GetValue()), out var period))
                    throw new IndexOutOfRangeScriptException(lineIndex, args, weatherPeriodIndex.GetValue(), argIndexWeatherReriodIndex);

                minSnowLevel.Set(lineIndex, args, new FloatObject(period.MinSnowLevel));
            };
        }
    }
}

