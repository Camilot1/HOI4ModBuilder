using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface IConstantable
    {
        Dictionary<string, GameConstant> GetConstants();
        bool TryGetConstant(string name, out GameConstant constant);
        bool TryGetConstantParentable(string name, out GameConstant constant);
    }
}
