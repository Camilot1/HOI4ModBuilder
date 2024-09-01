
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.commands.operators
{
    public class SubtractOperator : ScriptCommand
    {
        private static readonly string _keyword = "SUBTRACT";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new SubtractOperator();

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

                var variable = varsScope.GetValue(args[1]);
                var value = ScriptParser.ParseValue(varsScope, args[2]);

                if (variable == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);
                if (value == null)
                    throw new VariableValueIsNotSetScriptException(lineIndex, args);

                if (variable is ISubtractObject obj)
                    obj.Subtract(lineIndex, args, value);
                else
                    throw new InvalidOperationScriptException(lineIndex, args);
            };
        }
    }
}
