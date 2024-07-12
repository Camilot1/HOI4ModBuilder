using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{
    class ScriptedTriggerFile : IParadoxRead
    {
        private List<ScriptedTrigger> _list;

        public ScriptedTriggerFile(List<ScriptedTrigger> list)
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

            var info = new ScriptedTrigger(token);
            Logger.Log($"\tLoading ScriptedTrigger \"{info.name}\"");
            parser.Parse(info);
            _list.Add(info);
        }
    }

    class ScriptedTrigger : IParadoxRead
    {
        public string name;

        public ScriptedTrigger(string name)
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            //TODO Implement
        }
    }
}
