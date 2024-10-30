using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface IListObject :
        IScriptObject, ICollectionObject, IAddObject, IAddRangeObject,
        IInsertObject, IRemoveObject, IRemoveAtObject, IGetObject,
        ISetSizeObject, IGetSizeObject, IClearObject, ISortObject, IReverseObject,
        IShuffleObject, ISwapObject
    {
        void ForEach(Action<IScriptObject> action);
    }
}
