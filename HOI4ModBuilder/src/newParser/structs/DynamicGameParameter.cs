using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.structs
{
    public struct DynamicGameParameter
    {
        public Func<object, object> provider;
        public Func<object, string, object> factory; // First is Object, Second is Key

    }
}
