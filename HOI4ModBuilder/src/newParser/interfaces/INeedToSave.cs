using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface INeedToSave
    {
        bool IsNeedToSave();
        void SetNeedToSave(bool needToSave);
    }
}
