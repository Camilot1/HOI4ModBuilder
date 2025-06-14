using HOI4ModBuilder.src.newParser.interfaces;

namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IReadObject
    {
        void Read(int lineIndex, string[] args, ISetObject result);
        void ReadRange(int lineIndex, string[] args, IListObject result);
    }
}
