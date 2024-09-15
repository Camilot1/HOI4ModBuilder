
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;
using OpenTK.Graphics.ES10;
using HOI4ModBuilder.src.scripts.commands.declarators;

namespace HOI4ModBuilder.src.scripts.commands.functions.regions.weather
{
    public class IsRegionHasWeatherFunction : ScriptCommand
    {
        private static readonly string _keyword = "IS_REGION_HAS_WEATHER";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.regions.weather." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{BooleanDeclarator.GetKeyword()}:is_region_has_weather> <INUMBER:<region_id> <INUMBER:weather_period_index>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{BooleanDeclarator.GetKeyword()}:is_region_has_weather>",
            "\t<INUMBER:<region_id>",
            "\t<INUMBER:weather_period_index>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new IsRegionHasWeatherFunction();

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
                var isRegionHasWeather = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is BooleanObject
                );
                var regionId = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (!StrategicRegionManager.TryGetRegion(Convert.ToUInt16(regionId.GetValue()), out StrategicRegion region))
                    throw new ValueNotFoundScriptException(lineIndex, args);

                bool result = region.Weather != null && region.Weather.GetPeriodsCount() != 0;

                isRegionHasWeather.Set(lineIndex, args, new BooleanObject(result));
            };
        }
    }
}

