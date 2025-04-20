using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Globalization;

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
            { typeof(byte), (v) => {
                if (byte.TryParse(v, out var parsedValue))
                    return parsedValue;
                else if (Utils.TryParseFloat(v, out var floatValue)) {
                    if ((byte)floatValue != floatValue || floatValue < byte.MinValue || floatValue > byte.MaxValue)
                        throw new FormatException();
                    else
                        return (byte)floatValue;
                }
                else throw new FormatException();
            } },
            { typeof(ushort), (v) => {
                if (ushort.TryParse(v, out var parsedValue))
                    return parsedValue;
                else if (Utils.TryParseFloat(v, out var floatValue)) {
                    if ((ushort)floatValue != floatValue || floatValue < ushort.MinValue || floatValue > ushort.MaxValue)
                        throw new FormatException();
                    else
                        return (ushort)floatValue;
                }
                else throw new FormatException();
            } },
            { typeof(short), (v) => {
                if (short.TryParse(v, out var parsedValue))
                    return parsedValue;
                else if (Utils.TryParseFloat(v, out var floatValue)) {
                    if ((short)floatValue != floatValue || floatValue < short.MinValue || floatValue > short.MaxValue)
                        throw new FormatException();
                    else
                        return (short)floatValue;
                }
                else throw new FormatException();
            } },
            { typeof(int), (v) => {
                if (int.TryParse(v, out var parsedValue))
                    return parsedValue;
                else if (Utils.TryParseFloat(v, out var floatValue)) {
                    if ((int)floatValue != floatValue || floatValue < int.MinValue || floatValue > int.MaxValue)
                        throw new FormatException();
                    else
                        return (int)floatValue;
                }
                else throw new FormatException();
            } },
            { typeof(uint), (v) => {
                if (uint.TryParse(v, out var parsedValue))
                    return parsedValue;
                else if (Utils.TryParseFloat(v, out var floatValue)) {
                    if ((uint)floatValue != floatValue || floatValue < uint.MinValue || floatValue > uint.MaxValue)
                        throw new FormatException();
                    else
                        return (uint)floatValue;
                }
                else throw new FormatException();
            } },
            { typeof(float), (v) => Utils.ParseFloat(v) },
            { typeof(double), (v) => double.Parse(v) },
            { typeof(GameString), (v) => new GameString { stringValue = v } },
            { typeof(GameKeyObject<>), (v) => new GameKeyObject<object>() { key = v } },
            { typeof(DateTime), (v) => Utils.TryParseDateTimeStamp(v, out var dateTime) ? dateTime : throw new Exception("Invalid DateTime format: " + v) },
            { typeof(string), (v) => v },
        };

        public static T Parse<T>(string value)
        {
            if (_parseMapping.TryGetValue(typeof(T), out var func))
                return (T)func(value);
            else
                throw new Exception("Unknown value \"" + value + "\" type at ParseUtils.Parse: " + typeof(T));
        }

        public static object Parse(Type type, string value)
        {
            if (_parseMapping.TryGetValue(type, out var func))
                return func(value);
            else
                throw new Exception("Unknown value \"" + value + "\" type at ParseUtils.Parse: " + type);
        }

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

        public static string ObjectToSaveString(object value)
        {
            if (value is bool valueBool)
                return valueBool ? "yes" : "no";
            else if (value is short || value is ushort || value is int || value is uint)
                return "" + value;
            else if (value is float valueFloat)
                return Utils.FloatToString(valueFloat);
            else if (value is DateTime dateTime)
                return $"{dateTime.Year}.{dateTime.Month}.{dateTime.Day}";
            else if (value is Country valueCountry)
                return valueCountry.Tag;
            else if (value is State valueState)
                return "" + valueState.Id.GetValue();
            else if (value is GameString valueGameString)
                return valueGameString.stringValue;
            else if (value is string valueString)
                return valueString;
            else
                throw new Exception($"Unknown value type: {value} ({value.GetType()})");
        }

        public static bool TryParseScope(string value, out IScriptBlockInfo scope)
        {
            if (InfoArgsBlocksManager.TryGetScope(value, out var scopeBlock))
            {
                scope = scopeBlock;
                return true;
            }

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

        public static ScriptBlockParseObject GetAnyScriptBlockParseObject(IParentable parent, string token)
        {
            if (TryParseScope(token, out var info))
                return new ScriptBlockParseObject(parent, info);

            if (InfoArgsBlocksManager.TryGetInfoArgsBlock(token, out var infoBlock))
                return new ScriptBlockParseObject(parent, infoBlock);

            return null;
        }

        public static object SetParent(IParentable child, IParentable parent)
        {
            child.SetParent(parent);
            return child;
        }


        public static ScriptBlockParseObject ScriptBlockFabricProvide(IParentable parent, Func<IScriptBlockInfo>[] scriptBlockInfoProviders)
        {
            if (scriptBlockInfoProviders == null)
                return null;

            foreach (var provider in scriptBlockInfoProviders)
            {
                var scriptBlockInfo = provider.Invoke();
                if (scriptBlockInfo != null)
                    return new ScriptBlockParseObject(parent, scriptBlockInfo);
            }

            return null;
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
