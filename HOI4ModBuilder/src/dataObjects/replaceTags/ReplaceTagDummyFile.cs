using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.dataObjects.replaceTags
{
    class ReplaceTagDummyFile : IParadoxRead
    {
        public Dictionary<string, FileInfo> FoundedTokens { get; private set; }
        public FileInfo CurrentFileInfo { get; private set; }

        public string ReplaceTag { get; private set; }

        private string _listToken;
        private string _tokenStartsWith;
        private List<string> _list;
        private ReplaceTagDummy _replaceTagDummy;
        private ParserDummyBlock _parserDummyBlock;

        public ReplaceTagDummyFile(Dictionary<string, FileInfo> dictionary, FileInfo currentFileInfo, string replaceTag, string listToken, string tokenStartsWith, List<string> list)
        {
            FoundedTokens = dictionary;
            CurrentFileInfo = currentFileInfo;
            ReplaceTag = replaceTag;

            _listToken = listToken;
            _tokenStartsWith = tokenStartsWith;
            _list = list;
            _replaceTagDummy = new ReplaceTagDummy(this, _tokenStartsWith, _list);
            _parserDummyBlock = new ParserDummyBlock();
        }


        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (_listToken == null)
            {
                if (token != "limit" && (_tokenStartsWith == null || token.StartsWith(_tokenStartsWith)))
                {
                    if (!FoundedTokens.ContainsKey(token))
                    {
                        FoundedTokens[token] = CurrentFileInfo;
                        _list.Add(token);
                    }
                    else Logger.LogWarning(
                            EnumLocKey.WARNING_REPLACE_TAG_DUPLICATE_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{token}", token},
                                { "{replaceTag}", ReplaceTag},
                                { "{currentFilePath}", CurrentFileInfo.filePath},
                                { "{firstFilePath}", FoundedTokens[token].filePath}
                            }
                        );
                }
                parser.Parse(_parserDummyBlock);

            }
            else if (token == _listToken && (_tokenStartsWith == null || token.StartsWith(_tokenStartsWith)))
            {
                parser.Parse(_replaceTagDummy);
            }
        }
    }
    class ReplaceTagDummy : IParadoxRead
    {
        private ReplaceTagDummyFile _parentFile;
        private string _tokenStartsWith;
        private List<string> _list;
        private ParserDummyBlock _parserDummyBlock;

        public ReplaceTagDummy(ReplaceTagDummyFile parentFile, string tokenStartsWith, List<string> list)
        {
            _parentFile = parentFile;
            _tokenStartsWith = tokenStartsWith;
            _list = list;
            _parserDummyBlock = new ParserDummyBlock();
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token != "limit" && (_tokenStartsWith == null || token.StartsWith(_tokenStartsWith)))
            {
                if (!_parentFile.FoundedTokens.ContainsKey(token))
                {
                    _parentFile.FoundedTokens[token] = _parentFile.CurrentFileInfo;
                    _list.Add(token);
                }
                else Logger.LogWarning(
                            EnumLocKey.WARNING_REPLACE_TAG_DUPLICATE_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{token}", token},
                                { "{replaceTag}", _parentFile.ReplaceTag},
                                { "{currentFilePath}", _parentFile.CurrentFileInfo.filePath},
                                { "{firstFilePath}", _parentFile.FoundedTokens[token].filePath}
                            }
                        );
            }
            parser.Parse(_parserDummyBlock);
        }
    }
}
