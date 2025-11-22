using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace HOI4ModBuilder.src.newParser
{
    public class ParserUtils
    {
        struct MappingActions
        {
            public Func<object, string> save;
            public Func<string, object> parse;
        }

        private static readonly Dictionary<Type, MappingActions> _typesMapping = new Dictionary<Type, MappingActions>
        {
            { typeof(bool), new MappingActions {
                save = o => ((bool)o) ? "yes" : "no",
                parse = v => {
                    var val = v.ToLower();
                    if (val == "yes")
                        return true;
                    if (val == "no")
                        return false;
                    throw new FormatException(val);
                }
            } },
            { typeof(byte), new MappingActions {
                save = o => "" + o,
                parse = v => {
                    if (byte.TryParse(v, out var parsedValue))
                        return parsedValue;
                    if (!Utils.TryParseFloat(v, out var floatValue))
                        throw new FormatException();
                    if ((byte)floatValue != floatValue || floatValue < byte.MinValue || floatValue > byte.MaxValue)
                        throw new FormatException();
                    return (byte)floatValue;
                }
            }},
            { typeof(ushort), new MappingActions {
                save = o => "" + o,
                parse = v => {
                    if (ushort.TryParse(v, out var parsedValue))
                        return parsedValue;
                    if (!Utils.TryParseFloat(v, out var floatValue))
                        throw new FormatException();
                    if ((ushort)floatValue != floatValue || floatValue < ushort.MinValue || floatValue > ushort.MaxValue)
                        throw new FormatException();
                    return (ushort)floatValue;
                }
            } },
            { typeof(short), new MappingActions {
                save = o => "" + o,
                parse = v => {
                    if (short.TryParse(v, out var parsedValue))
                        return parsedValue;
                    if (!Utils.TryParseFloat(v, out var floatValue))
                        throw new FormatException();
                    if ((short)floatValue != floatValue || floatValue < short.MinValue || floatValue > short.MaxValue)
                        throw new FormatException();
                    return (short)floatValue;
                }
            } },
            { typeof(int), new MappingActions {
                save = o => "" + o,
                parse = v => {
                    if (int.TryParse(v, out var parsedValue))
                        return parsedValue;
                    if (!Utils.TryParseFloat(v, out var floatValue))
                        throw new FormatException();
                    if ((int)floatValue != floatValue || floatValue < int.MinValue || floatValue > int.MaxValue)
                        throw new FormatException();
                    return (int)floatValue;
                }
            }},
            { typeof(uint), new MappingActions {
                save = o => "" + o,
                parse = v => {
                if (uint.TryParse(v, out var parsedValue))
                    return parsedValue;
                if (!Utils.TryParseFloat(v, out var floatValue))
                        throw new FormatException();
                if ((uint)floatValue != floatValue || floatValue < uint.MinValue || floatValue > uint.MaxValue)
                    throw new FormatException();
                return (uint)floatValue;
            }
            }},
            { typeof(float), new MappingActions {
                save = o => Utils.FloatToString((float)o),
                parse = v => Utils.ParseFloat(v)
            } },
            { typeof(double), new MappingActions {
                save = o => Utils.DoubleToString((double)o),
                parse = v => Utils.ParseDouble(v)
            } },
            { typeof(string), new MappingActions {
                save = o => "" + o,
                parse = v => v
            } },
            { typeof(GameString), new MappingActions {
                save = o => ((GameString)o).stringValue,
                parse = v => new GameString { stringValue = v }
            } },
            { typeof(DateTime), new MappingActions {
                save = o => {
                    var dateTime = (DateTime)o;
                    return $"{dateTime.Year}.{dateTime.Month}.{dateTime.Day}";
                },
                parse = v => Utils.TryParseDateTimeStamp(v, out var dateTime) ?
                    dateTime :
                    throw new Exception($"Invalid DateTime format: {v}")
            } },
            { typeof(Province), new MappingActions {
                save = o => "" + ((Province)o).Id,
                parse = v => ushort.TryParse(v, out var id) ?
                    ProvinceManager.Get(id) :
                    throw new Exception($"Invalid ProvinceID format: {v}")
            } },
            { typeof(State), new MappingActions {
                save = o => "" + ((State)o).Id.GetValue(),
                parse = v => ushort.TryParse(v, out var id) ?
                    StateManager.Get(id) :
                    throw new Exception($"Invalid StateID format: {v}")
            } },
            { typeof(StrategicRegion), new MappingActions {
                save = o => "" + ((StrategicRegion)o).Id,
                parse = v => ushort.TryParse(v, out var id) ?
                    StrategicRegionManager.Get(id) :
                    throw new Exception($"Invalid StrategicRegionID format: {v}")
            } },
            { typeof(StateCategory), new MappingActions {
                save = o => "" + ((StateCategory)o).name,
                parse = v => StateCategoryManager.Get(v)
            } },
            { typeof(Resource), new MappingActions {
                save = o => "" + ((Resource)o).tag,
                parse = v => ResourceManager.Get(v)
            } },
            { typeof(Building), new MappingActions {
                save = o => "" + ((Building)o).Name,
                parse = v => BuildingManager.GetBuilding(v)
            } },
            { typeof(SpawnPoint), new MappingActions {
                save = o => "" + ((SpawnPoint)o).name,
                parse = v => BuildingManager.GetSpawnPoint(v)
            } },
            { typeof(Country), new MappingActions {
                save = o => "" + ((Country)o).Tag,
                parse = v => CountryManager.Get(v)
            } },
        };

        public static T Parse<T>(string value)
        {
            if (_typesMapping.TryGetValue(typeof(T), out var funcs))
                return (T)funcs.parse(value);
            else
                throw new Exception("Unknown value \"" + value + "\" type at ParseUtils.Parse: " + typeof(T));
        }

        public static object Parse(Type type, string value)
        {
            if (_typesMapping.TryGetValue(type, out var funcs))
                return funcs.parse(value);
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
            if (_typesMapping.TryGetValue(value.GetType(), out var funcs))
                return funcs.save(value);

            if (value is IEnumerable listValue)
            {
                var sb = new StringBuilder();
                foreach (var obj in listValue)
                    sb.Append(ObjectToSaveString(obj)).Append(" ");
                if (sb.Length > 0)
                    sb.Length = sb.Length - 1;
                return sb.ToString();

            }

            throw new Exception($"Unknown value type: {value} ({value.GetType()})");
        }

        public static bool TryParseScope(string value, out IScriptBlockInfo scope)
        {
            if (InfoArgsBlocksManager.TryGetScope(value, out var scopeBlock))
            {
                scope = scopeBlock;
                return true;
            }

            if (CountryManager.TryGet(value, out var country))
            {
                scope = country;
                return true;
            }

            if (ushort.TryParse(value, out var intValue) && StateManager.TryGet(intValue, out var state))
            {
                scope = state;
                return true;
            }

            scope = null;
            return false;
        }


        public static ScriptBlockParseObject GetScriptBlockParseObject(IParentable parent, string token, object value)
        {
            if (InfoArgsBlocksManager.TryGetInfoArgsBlock(token, out var infoBlock))
            {
                var block = new ScriptBlockParseObject(parent, infoBlock);
                block.SetValue(value);
                return block;
            }

            return null;
        }

        public static ScriptBlockParseObject GetAnyScriptBlockParseObject(IParentable parent, string token)
        {
            if (TryParseScope(token, out var info))
                return new ScriptBlockParseObject(parent, info);

            if (InfoArgsBlocksManager.TryGetInfoArgsBlock(token, out var infoBlock))
                return new ScriptBlockParseObject(parent, infoBlock);

            return null;
        }

        public static ScriptBlockParseObject GetBuildingCustomBlockParseObject(IParentable parentable, string token)
        {
            if (InfoArgsBlocksManager.TryGetBuildingCustomArgsBlock(token, out var infoBlock))
                return new ScriptBlockParseObject(parentable, infoBlock);

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
