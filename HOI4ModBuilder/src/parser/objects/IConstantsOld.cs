using HOI4ModBuilder.src.parser.parameter;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser.objects
{
    public interface IConstantsOld : IParseCallbacksOld
    {
        Dictionary<string, GameConstantOld> GetConstants();
        bool TryGetConstant(string name, out GameConstantOld constant);
    }
}
