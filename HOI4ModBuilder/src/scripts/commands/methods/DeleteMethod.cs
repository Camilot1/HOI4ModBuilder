using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.IO;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class DeleteMethod : ScriptCommand
    {
        private static readonly string _keyword = "DELETE";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <IFILE|ISTRING:file_path>",
            "======== OR ========",
            $"{_keyword} (",
            $"\t<IFILE|ISTRING:file_path>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new DeleteMethod();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _action = delegate ()
            {
                var filePathObj = ScriptParser.ParseValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IFileObject || o is IStringObject
                );

                var filePath = Convert.ToString(filePathObj.GetValue());
                File.Delete(filePath);
            };
        }
    }
}
