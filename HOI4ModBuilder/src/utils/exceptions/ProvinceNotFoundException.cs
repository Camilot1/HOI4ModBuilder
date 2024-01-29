using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.exceptions
{
    internal class ProvinceNotFoundException : Exception
    {
        public ProvinceNotFoundException(ushort provinceId) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_COUNTRY_NOT_FOUND,
                new Dictionary<string, string> { { "{provinceId}", "" + provinceId } }
            )
        )
        { }
    }
}
