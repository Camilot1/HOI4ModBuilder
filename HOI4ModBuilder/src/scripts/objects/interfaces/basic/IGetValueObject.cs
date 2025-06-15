using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IGetValueObject
    {
        IScriptObject GetValueType();
        void GetValue(int lineIndex, string[] args, IScriptObject value);
    }
}
