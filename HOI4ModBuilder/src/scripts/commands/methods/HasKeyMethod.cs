using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class HasKeyMethod : ScriptCommand
    {
        private static readonly string _keyword = "HAS_KEY";
        public static new string GetKeyword() => _keyword;
        public override ScriptCommand CreateEmptyCopy() => new HasKeyMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 4,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {

                var to = varsScope.GetValue(args[1]);
                var from = varsScope.GetValue(args[2]);
                var key = varsScope.GetValue(args[3]);

                if (to == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);
                if (from == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);
                if (key == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args);

                if (from is IMapObject mapObject)
                {
                    if (to is BooleanObject && key.IsSameType(mapObject.GetKeyType()))
                    {
                        var checkResult = new BooleanObject();
                        mapObject.HasKey(lineIndex, args, key, checkResult);
                        to.Set(lineIndex, args, checkResult);
                    }
                    else throw new InvalidValueTypeScriptException(lineIndex, args);
                }
                else throw new InvalidOperationScriptException(lineIndex, args);
            };
        }
    }
}
