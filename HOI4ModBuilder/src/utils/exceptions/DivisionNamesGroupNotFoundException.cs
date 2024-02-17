using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.exceptions
{
    class DivisionNamesGroupNotFoundException : Exception
    {
        public DivisionNamesGroupNotFoundException(string divisionNamesGroup) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.DIVISION_NAMES_GROUP_NOT_FOUND,
                new Dictionary<string, string> { { "{divisionNamesGroup}", divisionNamesGroup } }
            )
        )
        { }
    }
}
