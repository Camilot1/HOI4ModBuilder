using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.parser
{
    public interface IParentObjectOld
    {
        IParseObjectOld GetParent();
        void SetParent(IParseObjectOld parent);
    }
}
