using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.IO;

namespace HOI4ModBuilder.src.scripts.commands.methods
{
    public class IsExistsMethod : ScriptCommand
    {
        private static readonly string _keyword = "IS_EXISTS";
        public static new string GetKeyword() => _keyword;
        public override string GetPath() => "commands.declarators.methods." + _keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_keyword} <BOOLEAN:result> <IFILE|ISTRING:file_path>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <BOOLEAN:result>",
            $"\t<IFILE|ISTRING:file_path>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new IsExistsMethod();

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
                var result = (BooleanObject)ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is BooleanObject
                );

                var filePathObj = ScriptParser.ParseValue(
                    varsScope, args[2], lineIndex, args,
                    (o) => o is IFileObject || o is IStringObject
                );

                var filePath = Convert.ToString(filePathObj.GetValue());

                var resultValue = File.Exists(filePath);
                result.Set(lineIndex, args, new BooleanObject(resultValue));
            };
        }
    }
}

