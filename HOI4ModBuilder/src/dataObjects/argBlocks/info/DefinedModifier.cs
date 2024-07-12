using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.info
{

    class DefinedModifierFile : IParadoxRead
    {
        private List<DefinedModifier> _list;

        public DefinedModifierFile(List<DefinedModifier> list)
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

            var info = new DefinedModifier { name = token };
            Logger.Log($"\tLoading DefinedModifier \"{info.name}\"");
            parser.Parse(info);
            _list.Add(info);
        }
    }

    class DefinedModifier : IParadoxRead
    {
        public string name;
        public string colorType;
        public string valueType;
        public byte precision;
        public string postfix;
        public List<string> categories = new List<string>(0);

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "color_type": colorType = parser.ReadString(); break;
                case "value_type": valueType = parser.ReadString(); break;
                case "precision": precision = parser.ReadByte(); break;
                case "postfix": postfix = parser.ReadString(); break;
                case "category": categories.Add(parser.ReadString()); break;
            }
        }
    }
}
