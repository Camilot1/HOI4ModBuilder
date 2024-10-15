namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface INextFloatObject
    {
        void NextFloat(int lineIndex, string[] args, INumberObject result);
        void NextFloat(int lineIndex, string[] args, INumberObject maxValue, INumberObject result);
        void NextFloat(int lineIndex, string[] args, INumberObject minValue, INumberObject maxValue, INumberObject result);
    }
}