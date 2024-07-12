using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using FileInfo = HOI4ModBuilder.src.FileInfo;

namespace HOI4ModBuilder.src.hoiDataObjects.localisation
{
    class LocalizationManager
    {
        private static readonly string LOCALIZATION_DIRECTORY_PATH = "localisation" + FileManager.PATH_SEPARATOR;
        private static readonly string LANGUAGES_FILE = "languages.yml";
        private static readonly string LANGUAGES_FILE_PATH = LOCALIZATION_DIRECTORY_PATH + LANGUAGES_FILE;

        private static Dictionary<string, LocValue> _loc = new Dictionary<string, LocValue>();

        public static void Load(Settings settings)
        {
            _loc = new Dictionary<string, LocValue>(512);

            var fileInfoPairs = FileManager.ReadFileInfos(settings, LOCALIZATION_DIRECTORY_PATH, FileManager.ANY_FORMAT);
            if (!fileInfoPairs.TryGetValue(LANGUAGES_FILE, out FileInfo languagesFileInfo))
                throw new FileNotFoundException(LANGUAGES_FILE_PATH);

            string languagesText = File.ReadAllText(languagesFileInfo.filePath);

            if (!CustomYML.TryParseFile(languagesFileInfo, out Dictionary<string, List<YMLLine>> languagesData)) return;

            var langsTable = new Dictionary<string, int>(languagesData.Keys.Count);
            var langsKeys = new List<string>(languagesData.Keys);
            int langIndex = 0;
            foreach (var langKey in langsKeys)
            {
                langsTable[langKey] = langIndex;
                langIndex++;
            }

            var tasks = new Task<Dictionary<string, LocValue>>[langsTable.Count];


            for (int i = 0; i < langsTable.Count; i++)
            {
                var index = i;
                tasks[index] = Task.Factory.StartNew(() =>
                {
                    var langDir = langsKeys[index].Substring(2);
                    var data = FileManager.ReadRecursiveFileInfos(
                        settings, "localisation" + FileManager.PATH_SEPARATOR + langDir + FileManager.PATH_SEPARATOR, FileManager.YML_FORMAT
                    );

                    var innerData = new Dictionary<string, LocValue>(512);

                    while (data.fileInfos != null)
                    {
                        foreach (var fileInfoPair in data.fileInfos)
                        {
                            ParseFile(fileInfoPair.Value, innerData, in index, in langsTable, in langsKeys);
                        }
                    }

                    return innerData;
                });
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                var result = tasks[i].Result;

                foreach (var pair in result)
                {
                    if (_loc.TryGetValue(pair.Key, out var locValue))
                    {
                        var locPointer = locValue.languages[i];
                        var transferLocPointer = pair.Value.languages[i];

                        locPointer.fileInfo = transferLocPointer.fileInfo;
                        locPointer.lineIndex = transferLocPointer.lineIndex;
                        locPointer.version = transferLocPointer.version;

                        locValue.languages[i] = locPointer;

                        _loc[pair.Key] = locValue;
                    }
                    else _loc[pair.Key] = pair.Value;
                }
            }

            var test = _loc;
        }

        private static void ParseFile(FileInfo fileInfo, Dictionary<string, LocValue> data, in int langIndex, in Dictionary<string, int> langsTable, in List<string> langsKeys)
        {
            CustomYML.TryParseFile(fileInfo, out var rawData);

            foreach (var rawInfo in rawData)
            {
                if (langIndex != langsTable[rawInfo.Key])
                {
                    Logger.LogError(
                        EnumLocKey.ERROR_LOCALIZATION_FILE_CONTAINS_LOC_KEY_FOR_DIFFERENT_LANGUAGE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", fileInfo.filePath },
                            { "{expectedLanguage}", langsKeys[langIndex]},
                            { "{parsedLanguage}", rawInfo.Key }
                        }
                    );
                    continue;
                }

                foreach (var umlLine in rawInfo.Value)
                {
                    if (!data.TryGetValue(umlLine.key, out var locValue))
                        locValue.languages = new LocPointer[langsTable.Count];

                    locValue.languages[langIndex] = new LocPointer
                    {
                        fileInfo = fileInfo,
                        lineIndex = umlLine.lineIndex,
                        version = umlLine.version
                    };

                    data[umlLine.key] = locValue;
                }
            }

        }

    }

    public class CustomYML
    {
        private static readonly int initialCapacity = 4096;

        public static bool TryParseFile(FileInfo fileInfo, out Dictionary<string, List<YMLLine>> data)
        {
            bool hasNoErrors = true;

            data = new Dictionary<string, List<YMLLine>>();
            List<YMLLine> list = null;

            var lines = File.ReadAllLines(fileInfo.filePath);
            var sb = new StringBuilder(initialCapacity);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                try
                {
                    var umlLine = ParseLine(sb, line, i, out bool isLanguageLine);
                    sb.Length = 0;

                    if (isLanguageLine)
                    {
                        if (!data.TryGetValue(umlLine.key, out list))
                        {
                            list = new List<YMLLine>(64);
                            data[umlLine.key] = list;
                        }
                    }
                    else if (list != null)
                    {
                        umlLine.lineIndex = i;
                        list.Add(umlLine);
                    }
                    else
                        throw new UMLParserException(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_UML_LINE_IS_OUTSIDE_OF_LANGUAGE_SCOPE,
                                new Dictionary<string, string> {
                                    { "{lineIndex}", $"{i}" },
                                    { "{line}", $"{line}" }
                                }
                            ));
                }
                catch (Exception ex)
                {
                    Logger.LogException(new Exception(fileInfo.filePath + ": " + ex));
                    hasNoErrors = false;
                }
            }

            return hasNoErrors;
        }

        private static YMLLine ParseLine(StringBuilder sb, string line, int lineIndex, out bool isLanguageLine)
        {
            var state = EnumUMLState.READY_FOR_KEY;
            isLanguageLine = false;
            bool hasVersion = false;

            YMLLine ymlData = default;

            int i = 0;
            while (i < line.Length)
            {
                char ch = line[i];
                i++;

                if (state == EnumUMLState.READY_FOR_KEY)
                {
                    if (char.IsWhiteSpace(ch)) continue;

                    if (ch == '#')
                    {
                        if (ymlData.key == null)
                            throw new UMLParserException(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_UML_LINE_KEY_IS_NULL,
                                new Dictionary<string, string> { { "{lineIndex}", $"{lineIndex}" } }
                            ));
                        else if (ymlData.value == null && !hasVersion)
                            isLanguageLine = true;
                        else
                            throw new UMLParserException(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_UML_INCORRECT_LINE_STRUCTURE,
                                    new Dictionary<string, string> {
                                        { "{lineIndex}", $"{lineIndex}" },
                                        { "{line}", $"{line}" }
                                    }
                                ));

                        return ymlData;
                    }
                    else if (isAllowedInKey(ch))
                    {
                        sb.Append(ch);
                        state = EnumUMLState.KEY;
                    }
                    else
                        throw new UMLParserException(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_UML_LINE_CHARACTER_IS_NOT_ALLOWED_IN_KEY,
                                new Dictionary<string, string> {
                                    { "{lineIndex}", $"{lineIndex}" },
                                    { "{character}", $"{ch}" }
                                }
                            ));
                }
                else if (state == EnumUMLState.KEY)
                {
                    if (ch == ':')
                    {
                        ymlData.key = sb.ToString();
                        sb.Length = 0;
                        state = EnumUMLState.VERSION;
                    }
                    else if (isAllowedInKey(ch))
                    {
                        sb.Append(ch);
                    }
                    else
                        throw new UMLParserException(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_UML_LINE_CHARACTER_IS_NOT_ALLOWED_IN_KEY,
                                new Dictionary<string, string> {
                                    { "{lineIndex}", $"{lineIndex}" },
                                    { "{character}", $"{ch}" }
                                }
                            ));
                }
                else if (state == EnumUMLState.VERSION)
                {
                    if (char.IsWhiteSpace(ch))
                    {
                        if (sb.Length > 0)
                        {
                            if (int.TryParse(sb.ToString(), out ymlData.version))
                            {
                                hasVersion = true;
                                sb.Length = 0;
                            }
                            else
                                throw new UMLParserException(GuiLocManager.GetLoc(
                                    EnumLocKey.EXCEPTION_UML_LINE_INCORRECT_VERSION_VALUE,
                                        new Dictionary<string, string> {
                                            { "{lineIndex}", $"{lineIndex}" },
                                            { "{value}", $"{sb}" }
                                        }
                                    ));
                        }

                        state = EnumUMLState.READY_FOR_VALUE;
                    }
                    else sb.Append(ch);
                }
                else if (state == EnumUMLState.READY_FOR_VALUE)
                {
                    if (char.IsWhiteSpace(ch)) continue;
                    else if (ch == '"') state = EnumUMLState.VALUE;
                    else
                        throw new UMLParserException(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_UML_LINE_CHARACTER_IS_NOT_ALLOWED_IN_VALUE,
                                new Dictionary<string, string> {
                                    { "{lineIndex}", $"{lineIndex}" },
                                    { "{character}", $"{ch}" }
                                }
                            ));
                }
                else if (state == EnumUMLState.VALUE)
                {
                    if (ch == '\\')
                    {
                        i++;
                        ch = line[i];
                        sb.Append(ch);
                    }
                    else if (ch == '"')
                    {
                        ymlData.value = sb.ToString();
                        state = EnumUMLState.PARSED_VALUE;
                    }
                }
                else if (state == EnumUMLState.PARSED_VALUE)
                {
                    if (!char.IsWhiteSpace(ch))
                        throw new UMLParserException(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_UML_LINE_CHARACTER_IS_NOT_ALLOWED_AFTER_VALUE,
                                new Dictionary<string, string> {
                                    { "{lineIndex}", $"{lineIndex}" },
                                    { "{character}", $"{ch}" }
                                }
                            ));
                }
            }

            if (state != EnumUMLState.PARSED_VALUE && state != EnumUMLState.VERSION && isLanguageLine)
                throw new UMLParserException(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_UML_INCORRECT_LINE_STRUCTURE,
                        new Dictionary<string, string> {
                            { "{lineIndex}", $"{lineIndex}" },
                            { "{line}", $"{line}" }
                        }
                    ));
            else if (state == EnumUMLState.VERSION) isLanguageLine = true;

            return ymlData;
        }

        private static bool isLanguageLine(StringBuilder sb, string value, int lineIndex, out string language)
        {
            language = null;
            if (value.Length == 0 || !value.StartsWith("l_")) return false;

            sb.Append("l_");

            int colonCounter = 0;

            for (int i = 2; i < value.Length; i++)
            {
                var ch = value[i];
                if (!isAllowedInKey(ch))
                {
                    if (ch == ':' && colonCounter == 0) colonCounter++;
                    else return false;
                }
                else sb.Append(ch);
            }

            language = sb.ToString();
            return true;
        }

        private static bool isAllowedInKey(char ch)
        {
            return
                ch >= 97 && ch <= 122 || //a-z
                ch >= 65 && ch <= 90 || //A-Z
                ch >= 48 && ch <= 57 || //0-9
                ch == 95 || //_
                ch == 45 || //-
                ch == 46; //.
        }
    }

    public class UMLParserException : Exception
    {
        public UMLParserException(string message) : base(message) { }
    }

    public struct YMLLine
    {
        public int lineIndex;
        public string key;
        public int version;
        public string value;
    }

    public struct LocValue
    {
        public LocPointer[] languages;
    }

    public struct LocPointer
    {
        public FileInfo fileInfo;
        public int lineIndex;
        public int version;
        public string newValue;
    }

    public enum EnumUMLState
    {
        READY_FOR_KEY,
        KEY,
        VERSION,
        READY_FOR_VALUE,
        VALUE,
        PARSED_VALUE
    }
}
