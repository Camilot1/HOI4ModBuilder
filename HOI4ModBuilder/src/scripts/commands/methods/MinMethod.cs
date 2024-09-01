using HOI4ModBuilder.src.scripts.commands.commands;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class MinMethod : ScriptCommand
    {
        private static readonly string _keyword = "MIN";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new MinMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length >= 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {

                var variable = varsScope.GetValue(args[1]);
                INumberObject minValue = null;


                for (int i = 2; i < args.Length; i++)
                {
                    var value = ScriptParser.ParseValue(varsScope, args[i]);

                    if (variable == null)
                        throw new VariableIsNotDeclaredScriptException(lineIndex, args);
                    if (value == null)
                        throw new VariableValueIsNotSetScriptException(lineIndex, args);
                    if (!(value is INumberObject))
                        throw new InvalidValueTypeScriptException(lineIndex, args);

                    var tempValue = value as INumberObject;
                    var tempCheck = new BooleanObject();

                    if (minValue == null)
                        minValue = tempValue;
                    else
                    {
                        tempValue.IsLowerThan(lineIndex, args, minValue, tempCheck);
                        if (tempCheck.Value)
                        {
                            minValue = tempValue;
                        }
                    }
                }

                variable.Set(lineIndex, args, minValue);
            };
        }
    }
}
