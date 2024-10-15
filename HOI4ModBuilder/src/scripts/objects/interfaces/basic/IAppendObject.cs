
namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IAppendObject
    {
        void Append(int lineIndex, string[] args, IScriptObject value);
        void AppendRange(int lineIndex, string[] args, ICollectionObject value);
    }
}
