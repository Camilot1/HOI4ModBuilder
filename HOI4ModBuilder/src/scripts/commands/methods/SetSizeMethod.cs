using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class SetSizeMethod : ScriptCommand
    {
        private static readonly string _keyword = "SET_SIZE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <ISETSIZE:variable> <INUMBER:new_size>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <ISETSIZE:variable>",
            $"\t<INUMBER:new_size>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new SetSizeMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 3,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var variable = (ISetSizeObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is ISetSizeObject
                );

                var newSize = (INumberObject)ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                variable.SetSize(lineIndex, args, newSize);
            };
        }
    }
}

