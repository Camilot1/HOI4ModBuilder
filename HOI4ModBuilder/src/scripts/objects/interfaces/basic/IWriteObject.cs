namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IWriteObject
    {
        void Write(int lineIndex, string[] args, IScriptObject value);
        void WriteRange(int lineIndex, string[] args, IScriptObject value);
    }
}
