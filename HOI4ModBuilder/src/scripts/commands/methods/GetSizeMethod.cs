using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class GetSizeMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_SIZE";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.methods." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <INUMBER:to> <IGETSIZE:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <VALUE_TYPE:to>",
            $"\t<IGETSIZE>:from>",
            ")"
        };
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
                var from = (IGetSizeObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IGetSizeObject
                );

                var to = (INumberObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is INumberObject
                );

                var result = new IntObject();
                from.GetSize(lineIndex, args, result);
                to.Set(lineIndex, args, result);
            };
        }
    }
}

