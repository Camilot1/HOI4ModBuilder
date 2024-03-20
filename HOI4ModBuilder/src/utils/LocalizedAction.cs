using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils
{
    class LocalizedAction
    {
        public readonly EnumLocKey LocKey;
        public readonly Action Action;

        public LocalizedAction(EnumLocKey locKey, Action action)
        {
            LocKey = locKey;
            Action = action ?? (() => { });
        }
    }
}
