using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.parser.parameter
{
    public struct DynamicGameParameterOld
    {
        public Func<object, object> parameterProvider;
        public Func<object, string, object> payloadFactory; // First is Object, Second is Key

    }
}
