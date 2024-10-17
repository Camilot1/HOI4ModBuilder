using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class MapObject : IMapObject
    {
        public IScriptObject KeyType;
        public IScriptObject ValueType;

        private Dictionary<IScriptObject, IScriptObject> _map;
        public object GetValue() => _map;
        public MapObject()
        {
            KeyType = new AnyObject();
            ValueType = new AnyObject();
            _map = new Dictionary<IScriptObject, IScriptObject>();
        }

        public MapObject(IScriptObject keyType, IScriptObject valueType)
        {
            KeyType = keyType;
            ValueType = valueType;
            _map = new Dictionary<IScriptObject, IScriptObject>();
        }

        public bool IsSameType(IScriptObject scriptObject)
        {
            return scriptObject is MapObject mapObject &&
                KeyType.IsSameType(mapObject.KeyType) &&
                ValueType.IsSameType(mapObject.ValueType);
        }

        public string GetKeyword() => MapDeclarator.GetKeyword();
        public IScriptObject GetEmptyCopy() => new MapObject(KeyType.GetEmptyCopy(), ValueType.GetEmptyCopy());
        public IScriptObject GetCopy()
        {
            var map = new MapObject(KeyType.GetEmptyCopy(), ValueType.GetEmptyCopy());

            foreach (var entry in _map)
            {
                IScriptObject key, value;

                if (entry.Key is IPrimitiveObject)
                    key = entry.Key.GetCopy();
                else
                    key = entry.Key;

                if (entry.Value is IPrimitiveObject)
                    value = entry.Value.GetCopy();
                else
                    value = entry.Value;

                map._map[key] = value;
            }

            return map;
        }
        public IScriptObject GetKeyType() => KeyType;
        public IScriptObject GetValueType() => ValueType;

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (!IsSameType(value))
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
            _map = ((MapObject)value)._map;
        }

        public void Put(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (!KeyType.IsSameType(key))
                throw new InvalidKeyTypeScriptException(lineIndex, args, key);
            if (!ValueType.IsSameType(value))
                throw new InvalidValueTypeScriptException(lineIndex, args, value);

            _map[key] = value;
        }

        public void HasKey(int lineIndex, string[] args, IScriptObject key, BooleanObject result)
        {
            if (!KeyType.IsSameType(key))
                throw new InvalidKeyTypeScriptException(lineIndex, args, key);

            result.Value = _map.ContainsKey(key);
        }

        public void GetKeys(int lineIndex, string[] args, IScriptObject keys)
        {
            if (!(keys is ListObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, keys);

            var listObject = (ListObject)keys;

            listObject.Clear(lineIndex, args);
            foreach (IScriptObject key in _map.Keys)
            {
                if (!listObject.ValueType.IsSameType(key))
                    throw new InvalidValueTypeScriptException(lineIndex, args, key);

                var obj = key.GetEmptyCopy();
                obj.Set(lineIndex, args, key);
                listObject.Add(lineIndex, args, obj);
            }
        }

        public void HasValue(int lineIndex, string[] args, IScriptObject value, BooleanObject result)
        {
            if (!ValueType.IsSameType(value))
                throw new InvalidKeyTypeScriptException(lineIndex, args, value);

            result.Value = _map.ContainsValue(value);
        }

        public void GetValues(int lineIndex, string[] args, IScriptObject values)
        {
            if (!(values is ListObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, values);

            var listObject = (ListObject)values;

            listObject.Clear(lineIndex, args);
            foreach (IScriptObject value in _map.Values)
            {
                if (!listObject.ValueType.IsSameType(value))
                    throw new InvalidValueTypeScriptException(lineIndex, args, value);

                var obj = value.GetEmptyCopy();
                obj.Set(lineIndex, args, value);
                listObject.Add(lineIndex, args, obj);
            }
        }

        public void Get(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (!KeyType.IsSameType(key))
                throw new InvalidKeyTypeScriptException(lineIndex, args, key);

            if (!ValueType.IsSameType(value))
                throw new InvalidValueTypeScriptException(lineIndex, args, value);

            if (_map.TryGetValue(key, out IScriptObject mapValue))
                value.Set(lineIndex, args, mapValue);
            else
                value.Set(lineIndex, args, ValueType.GetEmptyCopy());
        }

        public void Clear(int lineIndex, string[] args) => _map.Clear();

        public void ForEach(Action<IScriptObject, IScriptObject> action)
        {
            foreach (var entry in _map)
            {
                action(entry.Key, entry.Value);
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in _map)
            {
                sb.Append(ScriptParser.FormatToString(item.Key)).Append(" = ")
                    .Append(ScriptParser.FormatToString(item.Value)).Append(", ");
            }

            if (sb.Length > 2) sb.Length -= 2;

            return GetKeyword() + "<" + KeyType.GetKeyword() + ',' + ValueType.GetKeyword() + ">[ " + sb + " ]";
        }

        public void GetSize(int lineIndex, string[] args, INumberObject result)
            => result.Set(lineIndex, args, new IntObject(_map.Count));
    }
}
