using HOI4ModBuilder.src.newParser.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser
{
    public class ParserUtils
    {
        private static readonly Dictionary<Type, Func<string, object>> _parseMapping = new Dictionary<Type, Func<string, object>>
        {
            { typeof(bool), (v) => {
                var val = v.ToLower();
                if (val == "yes") return true;
                else if (val == "no") return false;
                else throw new FormatException(val);
            } },
            { typeof(byte), (v) => byte.Parse(v) },
            { typeof(ushort), (v) => ushort.Parse(v) },
            { typeof(short), (v) => short.Parse(v) },
            { typeof(int), (v) => int.Parse(v) },
            { typeof(uint), (v) => uint.Parse(v) },
            { typeof(float), (v) => float.Parse(v) },
            { typeof(double), (v) => double.Parse(v) },
            { typeof(string), (v) => v },
        };

        public static T Parse<T>(string value) => (T)_parseMapping[typeof(T)](value);

        public static string AsseblePath(IParentable obj)
        {
            var parent = obj.GetParent();

            if (parent == null)
                return null;

            if (!(parent is IParseObject parentObj))
                return parent.AssemblePath();

            var staticAdapter = parentObj.GetStaticAdapter();
            foreach (var entry in staticAdapter)
            {
                var payload = entry.Value.Invoke(parent);
                if (payload.Equals(obj))
                    return entry.Key + " => ";
            }

            var dynamicAdapter = parentObj.GetDynamicAdapter();
            foreach (var entry in staticAdapter)
            {
                var payload = entry.Value.Invoke(parent);
                if (payload.Equals(obj))
                    return entry.Key + " => ";
            }

            return "UNKNOWN";
        }
    }
}
