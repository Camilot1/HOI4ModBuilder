namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IShuffleObject
    {
        void Shuffle(int lineIndex, string[] args);
        void Shuffle(int lineIndex, string[] args, INumberObject seed);
    }
}
