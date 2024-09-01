using HOI4ModBuilder.src.scripts.commands.operators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.commands.commands
{
    public class PutMethod : ScriptCommand
    {
        private static readonly string _keyword = "PUT";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new PutMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {

                var variable = varsScope.GetValue(args[1]);
                var key = ScriptParser.ParseValue(varsScope, args[2]);
                var value = ScriptParser.ParseValue(varsScope, args[3]);

                if (variable == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);

                if (variable is IPutObject obj)
                    obj.Put(lineIndex, args, key, value);
                else
                    throw new InvalidOperationScriptException(lineIndex, args);
            };
        }
    }
}
