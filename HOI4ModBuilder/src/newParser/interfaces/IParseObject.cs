using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface IParseObject : INeedToSave, IParentable, ISaveable, IConstantable, IParseCallbackable, IValidatable
    {
        IParseObject GetEmptyCopy();
        Dictionary<string, Func<object, object>> GetStaticAdapter();
        Dictionary<string, DynamicGameParameter> GetDynamicAdapter();
    }
}
