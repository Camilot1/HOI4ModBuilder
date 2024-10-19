using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using HOI4ModBuilder.src.scripts.commands.declarators;

namespace HOI4ModBuilder.src.scripts.commands.functions.map.regions.weather
{
    public class GetRegionWeatherPeriodArcticWaterFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_REGION_WEATHER_PERIOD_ARCTIC_WATER";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.map.regions.weather." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:arctic_water_chance> <INUMBER:region_id> <INUMBER:weather_period_index>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:arctic_water_chance>",
            "\t<INUMBER:region_id>",
            "\t<INUMBER:weather_period_index>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetRegionWeatherPeriodArcticWaterFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var arcticWaterChance = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexRegionId = 2;
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[argIndexRegionId], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexWeatherPeriodIndex = 3;
                var weatherPeriodIndex = ScriptParser.ParseValue(
                    varsScope, args[3], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out var region))
                    throw new ValueNotFoundScriptException(lineIndex, args, regionId.GetValue(), argIndexRegionId);

                if (!region.TryGetWeatherPeriod(Convert.ToInt32(weatherPeriodIndex.GetValue()), out var period))
                    throw new IndexOutOfRangeScriptException(lineIndex, args, weatherPeriodIndex.GetValue(), argIndexWeatherPeriodIndex);

                arcticWaterChance.Set(lineIndex, args, new FloatObject(period.ArcticWater));
            };
        }
    }
}
