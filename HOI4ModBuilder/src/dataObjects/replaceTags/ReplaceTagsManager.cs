using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.dataObjects.replaceTags
{
    class ReplaceTagsManager
    {
        private static string _replaceTagsFilePath = @"data\replace_tags.json";
        private static string _customReplaceTagsFilePath = @"data\custom_replace_tags.json";
        private static HashSet<string> _registeredReplaceTags = new HashSet<string>();
        private static Dictionary<string, List<string>> _replaceTagsListsMap = new Dictionary<string, List<string>>(0);

        public static void Load(Settings settings)
        {
            _registeredReplaceTags = new HashSet<string>();
            LoadFile(settings, _replaceTagsFilePath);
            LoadFile(settings, _customReplaceTagsFilePath);
        }

        private static void LoadFile(Settings settings, string filePath)
        {
            if (!File.Exists(filePath)) File.WriteAllText(filePath, "[]");

            foreach (var rtInfo in JsonConvert.DeserializeObject<List<ReplaceTagInfo>>(File.ReadAllText(filePath)))
                if (_registeredReplaceTags.Contains(rtInfo.replaceTag))
                    Logger.LogError(
                        EnumLocKey.ERROR_REPLACE_TAG_DUPLICATE_IN_FILE,
                        new Dictionary<string, string>
                        {
                            { "{replaceTag}", rtInfo.replaceTag },
                            { "{filePath}", filePath }
                        }
                    );
                else
                {
                    _registeredReplaceTags.Add(rtInfo.replaceTag);
                    PreloadReplaceTags(settings, rtInfo);
                }
        }

        public static List<string> AssembleNameWithReplaceTags(string name)
        {
            bool readInside = false;
            var sb = new StringBuilder();
            var tokens = new List<List<string>>();

            foreach (char ch in name.ToCharArray())
            {
                switch (ch)
                {
                    case '<':
                        if (readInside)
                        {
                            Logger.LogError(
                                EnumLocKey.ERROR_REPLACE_TAG_INCORRECT_STRUCTURE,
                                new Dictionary<string, string> { { "{name}", name } }
                            );
                            return new List<string>(0);
                        }
                        readInside = true;
                        if (sb.Length > 0)
                        {
                            tokens.Add(new List<string> { sb.ToString() });
                            sb.Length = 0;
                        }
                        break;
                    case '>':
                        if (!readInside)
                        {
                            Logger.LogError(
                                EnumLocKey.ERROR_REPLACE_TAG_INCORRECT_STRUCTURE,
                                new Dictionary<string, string> { { "{name}", name } }
                            );
                            return new List<string>(0);
                        }
                        readInside = false;
                        if (sb.Length > 0)
                        {
                            string[] innerReplaceTags = sb.ToString().Split('|');
                            sb.Length = 0;

                            List<string> tempRTTags = new List<string>();

                            foreach (string innerReplaceTag in innerReplaceTags)
                            {
                                if (_replaceTagsListsMap.TryGetValue(innerReplaceTag, out List<string> rtTags))
                                {
                                    if (rtTags.Count == 0)
                                    {
                                        Logger.LogError(
                                            EnumLocKey.ERROR_REPLACE_TAG_INNER_TAGS_LIST_IS_EMPTY,
                                            new Dictionary<string, string> { { "{innerReplaceTag}", innerReplaceTag } }
                                        );
                                        return new List<string>(0);
                                    }
                                    tempRTTags.AddRange(rtTags);
                                }
                                else Logger.LogWarning(
                                        EnumLocKey.WARNING_REPLACE_TAG_INNER_TAGS_LIST_ISNT_INITIALIZED,
                                        new Dictionary<string, string>
                                        {
                                            { "{innerReplaceTag}", innerReplaceTag },
                                            { "{name}", name }
                                        }
                                    );
                            }
                            tokens.Add(tempRTTags);
                        }
                        else
                        {
                            Logger.LogError(
                                EnumLocKey.ERROR_REPLACE_TAG_INCORRECT_STRUCTURE,
                                new Dictionary<string, string> { { "{name}", name } }
                            );
                            return new List<string>(0);
                        }
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            if (readInside)
            {
                Logger.LogError(
                    EnumLocKey.ERROR_REPLACE_TAG_INCORRECT_STRUCTURE,
                    new Dictionary<string, string> { { "{name}", name } }
                );
                return new List<string>(0);
            }

            if (sb.Length > 0)
            {
                tokens.Add(new List<string> { sb.ToString() });
                sb.Length = 0;
            }

            if (tokens.Count != 0)
            {
                var prevs = tokens[0];
                for (int i = 1; i < tokens.Count; i++)
                {
                    var currentTokens = tokens[i];
                    var nexts = new List<string>(prevs.Count * currentTokens.Count);

                    foreach (string prev in prevs)
                    {
                        foreach (string curToken in currentTokens)
                        {
                            nexts.Add(prev + curToken);
                        }
                    }

                    prevs = nexts;
                }
                return prevs;
            }
            else return new List<string>(0);
        }

        public static void PreloadReplaceTags(Settings settings, ReplaceTagInfo rtInfo)
        {
            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, rtInfo.filesDirectory);
            var list = new List<string>();

            if (rtInfo.specificFileName != null)
            {
                if (fileInfos.TryGetValue(rtInfo.specificFileName, out FileInfo fileInfo))
                    fileInfos = new Dictionary<string, FileInfo> { { rtInfo.specificFileName, fileInfo } };
                else fileInfos = new Dictionary<string, FileInfo>();
            }

            var FoundedTokens = new Dictionary<string, FileInfo>();

            foreach (var fileInfo in fileInfos.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                {
                    ParadoxParser.Parse(fs, new ReplaceTagDummyFile(FoundedTokens, fileInfo, rtInfo.replaceTag, rtInfo.groupTokenInFile, rtInfo.tokenInFileStartsWith, list));
                }
            }

            if (list.Count > 0) _replaceTagsListsMap[rtInfo.replaceTag] = list;
        }
    }

    class ReplaceTagInfo
    {
        public string replaceTag;
        public string filesDirectory;
        public string specificFileName;
        public string groupTokenInFile;
        public string tokenInFileStartsWith;
    }
}
