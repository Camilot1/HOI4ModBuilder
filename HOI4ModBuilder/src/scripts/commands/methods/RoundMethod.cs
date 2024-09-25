using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.operators
{
    public class RoundMethod : ScriptCommand
    {
        private static readonly string _keyword = "ROUND";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new RoundMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var variable = varsScope.GetValue(args[1]);

                if (variable == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);

                if (variable is FloatObject obj)
                    obj.Value = (float)Math.Round(obj.Value);
                else
                    throw new InvalidOperationScriptException(lineIndex, args);
            };
        }
    }
}
