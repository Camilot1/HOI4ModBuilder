using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{

    class ScriptedEffectsFile : IParadoxRead
    {
        private FileInfo _fileInfo;
        private List<ScriptedEffect> _list;

        public ScriptedEffectsFile(FileInfo fileInfo, List<ScriptedEffect> list)
        {
            _fileInfo = fileInfo;
            _list = list;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token.StartsWith("@"))
            { //TODO Reimplement @CONSTANTs
                parser.ReadString();
                return;
            }

            var info = new ScriptedEffect(token);
            Logger.Log($"\tLoading ScriptedEffect \"{info.name}\"");
            Logger.TryOrCatch(
                () =>
                {
                    parser.Parse(info);
                    _list.Add(info);
                },
                (ex) =>
                {
                    Logger.LogError(
                        EnumLocKey.ERROR_COULD_NOT_LOAD_SCRIPTED_BLOCK,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", _fileInfo?.filePath },
                            { "{type}", "ScriptedEffect" },
                            { "{name}", $"{info.name}" },
                            { "{cause}", $"{ex.Message}" },
                        }
                    );
                });
        }
    }

    class ScriptedEffect : IParadoxRead
    {
        public string name;

        public ScriptedEffect(string name)
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            //TODO Implement
        }
    }
}
