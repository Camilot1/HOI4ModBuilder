
using HOI4ModBuilder.src.newParser.interfaces;

namespace HOI4ModBuilder.src.parser.parameter
{
    public interface IGameParameterOld : INeedToSave, IParentObjectOld, IParseCallbacksOld
    {
        object GetObject();
        void SetObject(object obj);
        bool IsAnyDemiliter();
        bool IsQuoted();
    }
}
