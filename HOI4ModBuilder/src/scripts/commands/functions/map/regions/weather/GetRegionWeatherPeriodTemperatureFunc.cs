using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.regions.weather
{
    public class GetRegionWeatherPeriodTemperatureFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_WEATHER_PERIOD_TEMPERATURE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.regions.weather." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:min_temperature> <INUMBER:max_temperature> <INUMBER:region_id> <INUMBER:weather_period_index>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:min_temperature>",
            $"\tOUT <INUMBER:max_temperature>",
            "\t<INUMBER:region_id>",
            "\t<INUMBER:weather_period_index>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionWeatherPeriodTemperatureFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 5,
                new bool[] { true, true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var minTemp = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var maxTemp = ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexRegionId = 3;
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[argIndexRegionId], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexWeatherPeriodIndex = 4;
                var weatherPeriodIndex = ScriptParser.ParseValue(
                    varsScope, args[argIndexWeatherPeriodIndex], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out StrategicRegion region))
                    throw new ValueNotFoundScriptException(lineIndex, args, regionId.GetValue(), argIndexRegionId);

                if (!region.TryGetWeatherPeriod(Convert.ToInt32(weatherPeriodIndex.GetValue()), out var period))
                    throw new IndexOutOfRangeScriptException(lineIndex, args, weatherPeriodIndex.GetValue(), argIndexWeatherPeriodIndex);

                minTemp.Set(lineIndex, args, new FloatObject(period.Temperature[0]));
                maxTemp.Set(lineIndex, args, new FloatObject(period.Temperature[1]));
            };
        }
    }
}
