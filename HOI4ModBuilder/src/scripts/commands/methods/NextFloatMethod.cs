using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.exceptions;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class NextFloatMethod : ScriptCommand
    {
        private static readonly string _keyword = "NEXT_FLOAT";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INUMBER:result_var> <IRANDOM:random_obj>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:result_var>",
            $"\t<IRANDOM:random_obj>",
            ")",
            "======== OR ========",
            $"{_keyword} <INUMBER:result_var> <IRANDOM:random_obj> <INUMBER:max_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:result_var>",
            $"\t<IRANDOM:random_obj>",
            $"\t<INUMBER:max_value>",
            ")",
            "======== OR ========",
            $"{_keyword} <INUMBER:result_var> <IRANDOM:random_obj> <INUMBER:min_value> <INUMBER:max_value>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <INUMBER:result_var>",
            $"\t<IRANDOM:random_obj>",
            $"\t<INUMBER:min_value>",
            $"\t<INUMBER:max_value>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new NextFloatMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3 || a.Length == 4 || a.Length == 5,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var result = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );

                int argIndexRandomObj = 2;
                var randomObj = (IRandomObject)ScriptParser.GetValue(
                    varsScope, args[argIndexRandomObj], lineIndex, args,
                    (o) => o is IRandomObject
                );

                if (randomObj == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args, argIndexRandomObj);

                var resultValue = new FloatObject();

                if (args.Length == 3)
                {
                    randomObj.NextFloat(lineIndex, args, resultValue);
                }
                else if (args.Length == 4)
                {
                    var maxValue = (INumberObject)ScriptParser.ParseValue(
                        varsScope, args[3], lineIndex, args,
                        (o) => o is INumberObject
                    );
                    randomObj.NextFloat(lineIndex, args, maxValue, resultValue);
                }
                else if (args.Length == 5)
                {
                    var minValue = (INumberObject)ScriptParser.ParseValue(
                        varsScope, args[3], lineIndex, args,
                        (o) => o is INumberObject
                    );
                    var maxValue = (INumberObject)ScriptParser.ParseValue(
                        varsScope, args[4], lineIndex, args,
                        (o) => o is INumberObject
                    );
                    randomObj.NextFloat(lineIndex, args, minValue, maxValue, resultValue);
                }

                result.Set(lineIndex, args, resultValue);
            };
        }
    }
}

