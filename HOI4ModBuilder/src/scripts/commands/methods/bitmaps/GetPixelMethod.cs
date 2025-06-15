using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands.methods.bitmaps
{
    public class GetPixelMethod : ScriptCommand
    {
        private static readonly string _keyword = "GET_PIXEL";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods.bitmaps." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <INT_ARGB|INT_RGB|BYTES_ARGB|BYTES_RBG|BYTE_A|BYTE_R|BYTE_G|BYTE_B> <IBITMAP:from>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <VALUE_TYPE:to>",
            $"\t<IGETSIZE>:from>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetPixelMethod();

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
                var to = (INumberObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is INumberObject
                );
                var from = (IGetSizeObject)ScriptParser.GetValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IGetSizeObject
                );

                var result = new IntObject();
                from.GetSize(lineIndex, args, result);
                to.Set(lineIndex, args, result);
            };
        }
    }
}
