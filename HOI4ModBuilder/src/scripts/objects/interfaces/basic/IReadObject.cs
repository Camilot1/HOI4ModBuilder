namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IReadObject
    {
        void Read(int lineIndex, string[] args, IStringObject result);
        void ReadRange(int lineIndex, string[] args, IListObject result);
    }
}
