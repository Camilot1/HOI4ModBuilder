using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.data.exceptions
{
    public class BookmarkNotFoundException : Exception
    {
        public BookmarkNotFoundException(string dateTimeString) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_BOOKMARK_NOT_FOUND,
                new Dictionary<string, string> { { "{bookmark}", dateTimeString } }
            )
        )
        { }
    }
}
