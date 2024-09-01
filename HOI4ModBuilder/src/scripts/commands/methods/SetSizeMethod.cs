using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetSizeMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_SIZE";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new SetSizeMethod();

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
                var newSize = ScriptParser.ParseValue(varsScope, args[2]);

                if (variable == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);
                if (newSize == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);

                if (variable is ISetSizeObject varObj && newSize is INumberObject newSizeObj)
                    varObj.SetSize(lineIndex, args, newSizeObj);
                else
                    throw new InvalidOperationScriptException(lineIndex, args);
            };
        }
    }
}

