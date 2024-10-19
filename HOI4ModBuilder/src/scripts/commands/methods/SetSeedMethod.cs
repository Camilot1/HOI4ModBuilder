using HOI4ModBuilder.src.scripts.commands.operators.arithmetical;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetSeedMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_SEED";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISETSEED:variable> <INUMBER:value>"
        };
        public override ScriptCommand CreateEmptyCopy() => new SetSeedMethod();

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
                var variable = (ISetSeedObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ISetObject
                );

                int argIndexValue = 2;
                var value = (INumberObject)ScriptParser.ParseValue(
                    varsScope, args[argIndexValue], lineIndex, args,
                    (o) => o is INumberObject
                );

                if (value == null)
                    throw new VariableIsNotDeclaredScriptException(lineIndex, args, argIndexValue);

                variable.SetSeed(lineIndex, args, value);
            };
        }
    }
}

