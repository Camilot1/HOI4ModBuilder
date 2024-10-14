using HOI4ModBuilder.src.scripts.commands.functions.time;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.utils;
using System;

namespace HOI4ModBuilder.src.scripts.commands.functions.utils
{
    public class InterpolateFunc : ScriptCommand
    {
        private static readonly string _keyword = "INTERPOLATE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.functions.utils." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:result_value> <INUMBER:target_value> " +
                $"<INUMBER:start_arg> <INUMBER:start_value> <INUMBER:end_arg> <INUMBER:end_value>",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:result_value>",
            "\t<INUMBER:target_value>",
            "\t<INUMBER:start_arg>",
            "\t<INUMBER:start_value>",
            "\t<INUMBER:end_arg>",
            "\t<INUMBER:end_value>",
            ")",
            "======== OR ========",
            $"{_keyword} <INUMBER:result_value> <INUMBER:target_x> <INUMBER:target_y> " +
                $"<INUMBER:left_down_x> <INUMBER:left_down_y> <INUMBER:left_down_value> " +
                $"<INUMBER:right_down_x> <INUMBER:right_down_y> <INUMBER:right_down_value> " +
                $"<INUMBER:right_up_x> <INUMBER:right_up_y> <INUMBER:right_up_value> " +
                $"<INUMBER:left_up_x> <INUMBER:left_up_y> <INUMBER:left_up_value> ",
            "======== OR ========",
            $"{_keyword} (",
            "\tOUT <INUMBER:result_value>",
            "\t<INUMBER:target_x>",
            "\t<INUMBER:target_y>",
            "\t<INUMBER:left_down_x>",
            "\t<INUMBER:left_down_y>",
            "\t<INUMBER:left_down_value>",
            "\t<INUMBER:right_down_x>",
            "\t<INUMBER:right_down_y>",
            "\t<INUMBER:right_down_value>",
            "\t<INUMBER:right_up_x>",
            "\t<INUMBER:right_up_y>",
            "\t<INUMBER:right_up_value>",
            "\t<INUMBER:left_up_x>",
            "\t<INUMBER:left_up_y>",
            "\t<INUMBER:left_up_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new InterpolateFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 7 || a.Length == 16,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var resultValue = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                INumberObject[] parameters = new INumberObject[args.Length - 2];
                for (int i = 0; i < args.Length - 2; i++)
                {
                    parameters[i] = (INumberObject)ScriptParser.ParseValue(
                        varsScope, args[i + 2], lineIndex, args,
                        (o) => o is INumberObject
                    );
                }

                var result = 0f;

                if (args.Length == 7)
                {
                    result = (float)ScriptUtils.Interpolate1D(
                        Convert.ToDouble(parameters[0].GetValue()),
                        Convert.ToDouble(parameters[1].GetValue()),
                        Convert.ToDouble(parameters[2].GetValue()),
                        Convert.ToDouble(parameters[3].GetValue()),
                        Convert.ToDouble(parameters[4].GetValue())
                    );
                }
                else
                {
                    result = (float)ScriptUtils.Interpolate2D(
                        Convert.ToDouble(parameters[0].GetValue()),
                        Convert.ToDouble(parameters[1].GetValue()),
                        Convert.ToDouble(parameters[2].GetValue()),
                        Convert.ToDouble(parameters[3].GetValue()),
                        Convert.ToDouble(parameters[4].GetValue()),
                        Convert.ToDouble(parameters[5].GetValue()),
                        Convert.ToDouble(parameters[6].GetValue()),
                        Convert.ToDouble(parameters[7].GetValue()),
                        Convert.ToDouble(parameters[8].GetValue()),
                        Convert.ToDouble(parameters[9].GetValue()),
                        Convert.ToDouble(parameters[10].GetValue()),
                        Convert.ToDouble(parameters[11].GetValue()),
                        Convert.ToDouble(parameters[12].GetValue()),
                        Convert.ToDouble(parameters[13].GetValue())
                    );
                }

                resultValue.Set(lineIndex, args, new FloatObject(result));
            };
        }
    }
}


