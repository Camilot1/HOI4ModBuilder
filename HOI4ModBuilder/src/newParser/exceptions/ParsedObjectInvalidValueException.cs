using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.exceptions
{
    public class ParsedObjectInvalidValueException : Exception
    {
        public ParsedObjectInvalidValueException(string objectName, string parameterName, object parameterValue)
            : base(GuiLocManager.GetLoc(
                EnumLocKey.EXCEPTION_PARSED_OBJECT_INVALID_VALUE,
                new Dictionary<string, string>
                {
                    { "{objectName}", objectName },
                    { "{parameterName}", objectName },
                    { "{parameterValue}", $"{parameterValue}" },
                }))
        { }
    }
}
