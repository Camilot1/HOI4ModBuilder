using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.dataObjects
{
    abstract class ArgsBlock : IParadoxRead
    {
        public abstract void Save(StringBuilder sb, string outTab, string tab, string key, object next);
        public abstract void TokenCallback(ParadoxParser parser, string token);
    }
}
