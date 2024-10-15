
namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface ISplitObject
    {
        void Split(int lineIndex, string[] args, CharObject regex, IListObject result);
    }
}
