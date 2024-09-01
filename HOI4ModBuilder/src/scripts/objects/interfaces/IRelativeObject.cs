namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface IRelativeObject : IScriptObject
    {
        bool IsGreaterThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result);
        bool IsGreaterThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result);
        bool IsLowerThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result);
        bool IsLowerThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result);
        bool IsEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result);
        bool IsNotEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result);
    }
}
