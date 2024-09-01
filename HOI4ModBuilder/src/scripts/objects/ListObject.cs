using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace HOI4ModBuilder.src.scripts.objects
{
    internal class ListObject : IScriptObject, IListObject
    {
        public IScriptObject ValueType;
        private List<IScriptObject> _list;
        public object GetValue() => _list;

        public ListObject(IScriptObject valueType)
        {
            ValueType = valueType;
            _list = new List<IScriptObject>();
        }

        public string GetKeyword() => ListDeclarator.GetKeyword();
        public IScriptObject GetValueType() => ValueType;
        public bool IsSameType(IScriptObject scriptObject) => scriptObject is ListObject;

        public IScriptObject GetEmptyCopy() => new ListObject(ValueType.GetEmptyCopy());
        public IScriptObject GetCopy()
        {
            var list = new ListObject(ValueType.GetEmptyCopy());

            if (list.GetValueType() is IPrimitiveObject)
                _list.ForEach(obj => list._list.Add(obj.GetCopy()));
            else
                _list.ForEach(obj => list._list.Add(obj));

            return list;
        }

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is ListObject listObject)
            {
                if (ValueType.IsSameType(listObject.ValueType))
                    _list = listObject._list;
                else throw new InvalidValueTypeScriptException(lineIndex, args);
            }
            else if (value == null)
                throw new VariableIsNotDeclaredScriptException(lineIndex, args);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args);
        }

        public void Add(int lineIndex, string[] args, IScriptObject value)
        {
            if (value.IsSameType(ValueType))
            {
                if (value is IPrimitiveObject)
                    _list.Add(value.GetCopy());
                else
                    _list.Add(value);
            }
            else throw new InvalidValueTypeScriptException(lineIndex, args);
        }

        public void Clear(int lineIndex, string[] args)
        {
            _list.Clear();
        }

        public void Get(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (key is INumberObject numberObject)
            {
                int index = (int)numberObject.GetValue();

                if (index < 0 || index >= _list.Count)
                    throw new IndexOutOfRangeScriptException(lineIndex, args);

                var obj = _list[index];
                value.Set(lineIndex, args, obj);
            }
            else throw new InvalidValueTypeScriptException(lineIndex, args);
        }

        public void Insert(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (!ValueType.IsSameType(value))
                throw new InvalidValueTypeScriptException(lineIndex, args);

            if (key is INumberObject numberObject)
            {
                int index = (int)numberObject.GetValue();

                if (index < 0 || index >= _list.Count)
                    throw new IndexOutOfRangeScriptException(lineIndex, args);

                if (value is IPrimitiveObject)
                    _list.Insert(index, value.GetCopy());
                else
                    _list.Insert(index, value);
            }
            else throw new InvalidValueTypeScriptException(lineIndex, args);
        }

        public void Remove(int lineIndex, string[] args, IScriptObject value)
        {
            if (ValueType.IsSameType(value))
                _list.Remove(value);
            else throw new InvalidValueTypeScriptException(lineIndex, args);
        }

        public void RemoveAt(int lineIndex, string[] args, INumberObject value)
        {
            int index = (int)value.GetValue();
            if (index < 0 || index >= _list.Count)
                throw new IndexOutOfRangeScriptException(lineIndex, args);

            _list.RemoveAt(index);
        }

        public void AddRange(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is IListObject listObject && listObject.GetValueType().IsSameType(ValueType))

                if (value is IPrimitiveObject)
                    listObject.ForEach(obj => _list.Add(obj.GetCopy()));
                else
                    listObject.ForEach(obj => _list.Add(obj));
            else
                throw new InvalidValueTypeScriptException(lineIndex, args);
        }

        public void HasValue(int lineIndex, string[] args, IScriptObject value, BooleanObject result)
        {
            if (!ValueType.IsSameType(value))
                throw new InvalidKeyTypeScriptException(lineIndex, args);

            result.Value = _list.Contains(value);
        }

        public void GetValues(int lineIndex, string[] args, IScriptObject values)
        {
            if (!(values is ListObject))
                throw new InvalidValueTypeScriptException(lineIndex, args);

            var listObject = (ListObject)values;
            if (!listObject.ValueType.IsSameType(ValueType))
                throw new InvalidValueTypeScriptException(lineIndex, args);

            listObject.Clear(lineIndex, args);
            foreach (IScriptObject value in _list)
            {
                var obj = ValueType.GetEmptyCopy();
                obj.Set(lineIndex, args, value);
                listObject.Add(lineIndex, args, obj);
            }
        }

        public void ForEach(Action<IScriptObject> action)
        {
            foreach (var obj in _list)
            {
                action(obj);
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in _list)
                sb.Append(item.GetValue()).Append(", ");

            if (sb.Length > 2) sb.Length -= 2;

            return GetKeyword() + "<" + ValueType?.GetKeyword() + ">[" + sb + "]";
        }

        public void GetSize(int lineIndex, string[] args, INumberObject result)
            => result.Set(lineIndex, args, new IntObject(_list.Count));

        public void SetSize(int lineIndex, string[] args, INumberObject value)
        {
            int newSize = (int)value.GetValue();

            if (newSize < 0)
                throw new IndexOutOfRangeScriptException(lineIndex, args);

            while (_list.Count < newSize)
                _list.Add(ValueType.GetEmptyCopy());

            while (_list.Count > newSize)
                _list.RemoveAt(_list.Count - 1);
        }
    }
}
