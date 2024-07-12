using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{

    class ScriptedEffectsFile : IParadoxRead
    {
        private List<ScriptedEffect> _list;

        public ScriptedEffectsFile(List<ScriptedEffect> list)
        {
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
            parser.Parse(info);
            _list.Add(info);
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
