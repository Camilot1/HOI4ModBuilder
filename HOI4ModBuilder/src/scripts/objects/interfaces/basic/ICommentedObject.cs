
namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface ICommentedObject
    {
        void SetPrevComments(int lineIndex, string[] args, IListObject value);
        void SetInlineComment(int lineIndex, string[] args, IStringObject value);
        void GetPrevComments(int lineIndex, string[] args, IListObject value);
        void GetInlineComment(int lineIndex, string[] args, IStringObject value);
    }
}
