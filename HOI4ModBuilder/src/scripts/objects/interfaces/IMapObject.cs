
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface IMapObject : IScriptObject, ICollectionObject, ISetObject, IPutObject, IGetObject, IGetSizeObject, IClearObject
    {
        void ForEach(Action<IScriptObject, IScriptObject> action);
    }
}
