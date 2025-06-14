using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IGetKeyObject
    {
        object GetKey();
        IScriptObject GetKeyType();
        void GetKey(int lineIndex, string[] args, IScriptObject key);
    }
}
