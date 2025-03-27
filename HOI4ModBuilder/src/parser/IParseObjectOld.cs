using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser
{
    public interface IParseObjectOld : INeedToSave, IParseCallbacksOld, IParentObjectOld
    {
        IParseObjectOld GetEmptyCopy();
        Dictionary<string, Func<object, object>> GetStaticAdapter();
        Dictionary<string, DynamicGameParameterOld> GetDynamicAdapter();
    }
}
