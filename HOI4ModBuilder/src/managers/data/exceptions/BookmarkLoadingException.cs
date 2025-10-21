using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.data.exceptions
{
    public class BookmarkLoadingException : Exception
    {
        public BookmarkLoadingException(string dateTimeString, Exception innerException) : base(
            GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_WHILE_BOOKMARK_LOADING,
                new Dictionary<string, string> { { "{bookmark}", dateTimeString } }
            ),
            innerException
        )
        { }
    }
}
