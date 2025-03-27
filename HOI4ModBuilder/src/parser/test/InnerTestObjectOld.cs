using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser.objects
{
    public class InnerTestObjectOld : AbstractParseObjectOld
    {
        public override Dictionary<string, DynamicGameParameterOld> GetDynamicAdapter()
        {
            throw new NotImplementedException();
        }

        public override IParseObjectOld GetEmptyCopy() => new InnerTestObjectOld();

        public override Dictionary<string, Func<object, Object>> GetStaticAdapter()
        {
            throw new NotImplementedException();
        }
    }
}
