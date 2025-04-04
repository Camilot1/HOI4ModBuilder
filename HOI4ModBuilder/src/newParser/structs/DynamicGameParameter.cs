using System;

namespace HOI4ModBuilder.src.newParser.structs
{
    public struct DynamicGameParameter
    {
        public bool parseInnerBlock;
        public Func<object, object> provider;
        public Func<object, string, object> factory; // First is Object, Second is Key
    }
}
