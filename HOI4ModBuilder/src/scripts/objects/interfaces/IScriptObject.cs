
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.objects
{
    public interface IScriptObject : ISetObject
    {
        object GetValue();
        bool IsSameType(IScriptObject scriptObject);
        IScriptObject GetEmptyCopy();
        IScriptObject GetCopy();
        string GetKeyword();
    }
}
