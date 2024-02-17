using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.exceptions
{
    class IncorrectValueException : Exception
    {
        public IncorrectValueException(string value) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.INCORRECT_VALUE,
                new Dictionary<string, string> { { "{value}", value } }
            )
        )
        { }
    }
}
