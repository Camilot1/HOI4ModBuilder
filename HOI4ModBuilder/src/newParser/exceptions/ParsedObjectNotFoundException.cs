using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.exceptions
{
    public class ParsedObjectNotFoundException : Exception
    {
        public ParsedObjectNotFoundException(string objectName, string parameterName, object parameterValue)
            : base(GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_PARSED_OBJECT_NOT_FOUND,
                new Dictionary<string, string>
                {
                    { "{objectName}", objectName },
                    { "{parameterName}", objectName },
                    { "{parameterValue}", $"{parameterValue}" },
                }))
        {}
    }
}
