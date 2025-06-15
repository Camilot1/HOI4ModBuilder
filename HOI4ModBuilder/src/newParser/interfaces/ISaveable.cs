using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System.Text;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface ISaveable : ICommentable
    {
        void Save(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter);
        SavePattern GetSavePattern();
    }
}
