using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.exceptions
{
    class UnknownTokenException : Exception
    {
        public UnknownTokenException(string token) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_UNKNOWN_TOKEN,
                new Dictionary<string, string> { { "{token}", token } }
            )
        )
        { }
    }
}
