using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class GetSizeMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_SIZE";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new GetSizeMethod();

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
                var variable = varsScope.GetValue(args[1]);
                var target = varsScope.GetValue(args[2]);

                if (variable == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);
                if (target == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);

                if (variable is INumberObject varObj && target is IGetSizeObject targetObj)
                {
                    var result = new IntObject();
                    targetObj.GetSize(lineIndex, args, result);
                    varObj.Set(lineIndex, args, result);
                }
                else
                    throw new InvalidOperationScriptException(lineIndex, args);
            };
        }
    }
}

