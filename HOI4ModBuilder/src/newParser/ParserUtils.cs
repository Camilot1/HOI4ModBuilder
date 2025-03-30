using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
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
        public static object ParseObject(string value)
        {
            if (int.TryParse(value, out var intResult))
                return intResult;
            else if (float.TryParse(value, out var floatResult))
                return floatResult;

            var loweredValue = value.ToLower();
            if (loweredValue == "yes")
                return true;
            else if (loweredValue == "no")
                return false;

            return value;
        }

        public static bool TryParseScope(string value, out object scope)
        {
            if (CountryManager.TryGetCountry(value, out var country))
            {
                scope = country;
                return true;
            }

            if (ushort.TryParse(value, out var intValue) && StateManager.TryGetState(intValue, out var state))
            {
                scope = state;
                return true;
            }

            scope = null;
            return false;
        }

        public static ScriptBlockParseObject ScriptBlockFabricProvide(IParentable parent, IScriptBlockInfo scriptBlockInfo)
        {
            if (scriptBlockInfo == null)
                return null;

            return new ScriptBlockParseObject(parent, scriptBlockInfo);
        }

        public static bool IsAllowedValueType(EnumValueType valueType, EnumValueType[] allowedValueTypes)
        {
            if (allowedValueTypes == null)
                return false;

            foreach (var allowedValueType in allowedValueTypes)
            {
                if (allowedValueType == valueType)
                    return true;
            }

            return false;
        }

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
