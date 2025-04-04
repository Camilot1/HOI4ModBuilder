using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.parameters
{
    public struct ParserFlags<T>
    {
        public string TOKEN;
        public Func<T, IParadoxParameter> PROVIDER;
        public Func<object> FABRIC;
        public object DEFAULT;
    }
}
