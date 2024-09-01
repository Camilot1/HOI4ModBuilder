using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface ILogicalObject : IScriptObject, IAndObject, IOrObject, IXorObject, INotObject
    {
    }
}
