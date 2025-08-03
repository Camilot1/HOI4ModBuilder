using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface IStringObject
        : IScriptObject, IAddObject, IGetObject, IPutObject, IInsertObject, IRemoveAtObject, IClearObject,
        IGetSizeObject, ISetSizeObject, ITrimObject, IRelativeObject, ISplitObject
    {
        string GetString();
    }
}
