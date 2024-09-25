using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.commands.declarators;

namespace HOI4ModBuilder.src.scripts.commands.functions.files
{
    public class GetModDirectoryPathFunc : ScriptCommand
    {
        private static readonly string _keyword = "GET_MOD_DIRECTORY_PATH";
        public static new string GetKeyword() => _keyword;
        public static new string GetPath() => "commands.declarators.functions.files." + _keyword;
        public static new string[] GetDocumentation() => documentation;
        public static readonly string[] documentation = new string[]
        {
            $"{_keyword} <{StringDeclarator.GetKeyword()}|{FileDeclarator.GetKeyword()}:var_name>",
            "======== OR ========",
            $"{_keyword} (",
            $"\tOUT <{StringDeclarator.GetKeyword()}|{FileDeclarator.GetKeyword()}:path>",
            ")"
        };
        public override ScriptCommand CreateEmptyCopy() => new GetModDirectoryPathFunc();

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            args = ScriptParser.ParseCommandCallArgs(
                (a) => a.Length == 2,
                new bool[] { true },
                out _executeBeforeCall,
                lines, ref index, indent, varsScope, args
            );

            _varsScope = varsScope;
            _action = delegate ()
            {
                var result = ScriptParser.GetValue(
                    varsScope, args[1], lineIndex, args,
                    (o) => o is IStringObject || o is IFileObject
                );

                result.Set(lineIndex, args, new StringObject(SettingsManager.settings.modDirectory));
            };
        }
    }
}