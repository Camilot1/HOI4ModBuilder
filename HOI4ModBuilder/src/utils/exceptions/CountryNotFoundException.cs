using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.exceptions
{
    class CountryNotFoundException : Exception
    {
        public CountryNotFoundException(string countryTag) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.COUNTRY_NOT_FOUND,
                new Dictionary<string, string> { { "{countryTag}", countryTag } }
            )
        )
        { }
    }
}
