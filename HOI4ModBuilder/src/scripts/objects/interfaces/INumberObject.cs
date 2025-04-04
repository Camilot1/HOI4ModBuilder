
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;

namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface INumberObject
        : IScriptObject, ISetObject, IAddObject, ISubtractObject, IMultiplyObject, IDivideObject,
        IModuloObject, IRelativeObject, IComparable
    {
    }
}
