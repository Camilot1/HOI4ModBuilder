using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface ISaveable : ICommentable
    {
        void Save(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key);
        bool CustomSave(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key);
        SaveAdapter GetSaveAdapter();
    }
}
