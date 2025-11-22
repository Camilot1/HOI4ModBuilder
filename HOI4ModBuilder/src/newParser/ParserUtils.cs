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

namespace HOI4ModBuilder.src.newParser
{
    public class ParserUtils
    {
        private delegate bool TryParseDelegate<T>(string value, out T result);

        struct MappingActions
        {
            public Func<object, string> save;
            public Func<string, object> parse;
        }

        private static bool TryGetMapping(Type type, out MappingActions actions) => _typesMapping.TryGetValue(type, out actions);

        private static MappingActions CreateIntegerMapping<T>(TryParseDelegate<T> tryParse, float minValue, float maxValue, Func<float, T> cast)
        {
            return new MappingActions
            {
                save = o => "" + o,
                parse = v =>
                {
                    if (tryParse(v, out var parsedValue))
                        return parsedValue;

                    if (!Utils.TryParseFloat(v, out var floatValue))
                        throw new FormatException();

                    if (floatValue < minValue || floatValue > maxValue)
                        throw new FormatException();

                    var converted = cast(floatValue);
                    if (Math.Abs(floatValue - Convert.ToSingle(converted)) > float.Epsilon)
                        throw new FormatException();

                    return converted;
                }
            };
        }

        private static readonly Dictionary<Type, MappingActions> _typesMapping = new Dictionary<Type, MappingActions>
        {
            { typeof(bool), new MappingActions {
                save = o => ((bool)o) ? "yes" : "no",
                parse = v => {
                    var val = v.ToLowerInvariant();
                    if (val == "yes")
                        return true;
                    if (val == "no")
                        return false;
                    throw new FormatException(val);
                }
            } },
            { typeof(byte), CreateIntegerMapping(byte.TryParse, byte.MinValue, byte.MaxValue, v => (byte)v) },
            { typeof(ushort), CreateIntegerMapping(ushort.TryParse, ushort.MinValue, ushort.MaxValue, v => (ushort)v) },
            { typeof(short), CreateIntegerMapping(short.TryParse, short.MinValue, short.MaxValue, v => (short)v) },
            { typeof(int), CreateIntegerMapping(int.TryParse, int.MinValue, int.MaxValue, v => (int)v) },
            { typeof(uint), CreateIntegerMapping(uint.TryParse, uint.MinValue, uint.MaxValue, v => (uint)v) },
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
            return (T)Parse(typeof(T), value);
        }

        public static object Parse(Type type, string value)
        {
            if (TryGetMapping(type, out var funcs))
                return funcs.parse(value);

            throw new Exception("Unknown value \"" + value + "\" type at ParseUtils.Parse: " + type);
        }

        public static object ParseObject(string value)
        {
            if (int.TryParse(value, out var intResult))
                return intResult;
            else if (Utils.TryParseFloat(value, out var floatResult))
                return floatResult;

            var loweredValue = value.ToLowerInvariant();
            if (loweredValue == "yes")
                return true;
            else if (loweredValue == "no")
                return false;

            return value;
        }

        public static string ObjectToSaveString(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_typesMapping.TryGetValue(value.GetType(), out var funcs))
                return funcs.save(value);

            if (value is GameConstant gameConstant)
                return "@" + gameConstant.Key;

            if (value is IEnumerable listValue)
            {
                var parts = new List<string>();
                foreach (var obj in listValue)
                    parts.Add(ObjectToSaveString(obj));

                return string.Join(" ", parts);
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

        public static bool TryGetGameFile(IParentable node, out GameFile gameFile)
        {
            var current = node;
            while (current != null)
            {
                if (current is GameFile file)
                {
                    gameFile = file;
                    return true;
                }
                current = current.GetParent();
            }

            gameFile = null;
            return false;
        }

        public static GameFile GetGameFile(IParentable node)
            => TryGetGameFile(node, out var gameFile) ? gameFile : null;


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
            return allowedValueTypes != null && Array.IndexOf(allowedValueTypes, valueType) != -1;
        }

        public static void ParseEqualsDemiliter(GameParser parser)
        {
            parser.ParseDemiliters();
            var demiliters = parser.PullParsedDataString();

            if (demiliters.Length != 1 || demiliters[0] != '=')
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());
        }

        public static string AssemblePath(IParentable obj)
        {
            var parent = obj?.GetParent();

            if (parent == null)
                return null;

            var parentPath = AssemblePath(parent);

            if (!(parent is IParseObject parentObj))
                return parentPath;

            var staticAdapter = parentObj.GetStaticAdapter();
            if (staticAdapter != null)
            {
                foreach (var entry in staticAdapter)
                {
                    var payload = entry.Value.Invoke(parent);
                    if (ReferenceEquals(payload, obj) || payload?.Equals(obj) == true)
                        return CombinePath(parentPath, entry.Key);
                }
            }

            var dynamicAdapter = parentObj.GetDynamicAdapter();
            if (dynamicAdapter != null)
            {
                foreach (var entry in dynamicAdapter)
                {
                    var payload = entry.Value.provider.Invoke(parent);
                    if (ReferenceEquals(payload, obj) || payload?.Equals(obj) == true)
                        return CombinePath(parentPath, entry.Key);
                }
            }

            return CombinePath(parentPath, "UNKNOWN");
        }

        private static string CombinePath(string parentPath, string segment)
        {
            if (string.IsNullOrEmpty(segment))
                return parentPath;

            if (string.IsNullOrEmpty(parentPath))
                return segment;

            return $"{parentPath} => {segment}";
        }
    }
}
