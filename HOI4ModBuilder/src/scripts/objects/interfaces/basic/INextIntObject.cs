namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface INextIntObject
    {
        void NextInt(int lineIndex, string[] args, INumberObject maxValue, INumberObject result);
        void NextInt(int lineIndex, string[] args, INumberObject minValue, INumberObject maxValue, INumberObject result);
    }
}
