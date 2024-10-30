namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface ISwapObject
    {
        void Swap(int lineIndex, string[] args, INumberObject first, INumberObject second);
    }
}
