using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.scripts.objects
{
    internal class ListObject : IScriptObject, IListObject
    {
        public IScriptObject ValueType;
        private List<IScriptObject> _list;
        public object GetValue() => _list;

        public ListObject()
        {
            ValueType = new AnyObject();
            _list = new List<IScriptObject>();
        }

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
            if (IsSameType(value) || CanBeConvertedFrom(value))
                _list = (List<IScriptObject>)value.GetValue();
            else if (value == null)
                throw new VariableIsNotDeclaredScriptException(lineIndex, args, null);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }
        public bool CanBeConvertedFrom(IScriptObject value)
        {
            if (!(value is IListObject listObject))
                return false;

            if (IsSameType(listObject))
                return true;

            var result = new bool[] { true };
            listObject.ForEach((v) =>
            {
                if (!result[0])
                    return;

                if (!ValueType.IsSameType(v))
                    result[0] = false;
            });

            return result[0];
        }

        public void Add(int lineIndex, string[] args, IScriptObject value)
        {
            if (ValueType.IsSameType(value))
            {
                if (value is IPrimitiveObject)
                    _list.Add(value.GetCopy());
                else
                    _list.Add(value);
            }
            else throw new InvalidValueTypeScriptException(lineIndex, args, value);
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
                    throw new IndexOutOfRangeScriptException(lineIndex, args, index);

                var obj = _list[index];
                value.Set(lineIndex, args, obj);
            }
            else throw new InvalidKeyTypeScriptException(lineIndex, args, key);
        }

        public void Insert(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (!ValueType.IsSameType(value))
                throw new InvalidValueTypeScriptException(lineIndex, args, value);

            if (key is INumberObject numberObject)
            {
                int index = (int)numberObject.GetValue();

                if (index < 0 || index >= _list.Count)
                    throw new IndexOutOfRangeScriptException(lineIndex, args, index);

                if (value is IPrimitiveObject)
                    _list.Insert(index, value.GetCopy());
                else
                    _list.Insert(index, value);
            }
            else throw new InvalidKeyTypeScriptException(lineIndex, args, key);
        }

        public void Remove(int lineIndex, string[] args, IScriptObject value)
        {
            if (ValueType.IsSameType(value))
                _list.Remove(value);
            else throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void RemoveAt(int lineIndex, string[] args, INumberObject value)
        {
            int index = (int)value.GetValue();
            if (index < 0 || index >= _list.Count)
                throw new IndexOutOfRangeScriptException(lineIndex, args, value);

            _list.RemoveAt(index);
        }

        public void AddRange(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is IListObject listObject && ValueType.IsSameType(listObject.GetValueType()))
                listObject.ForEach(obj =>
                {
                    if (obj is IPrimitiveObject)
                        _list.Add(obj.GetCopy());
                    else _list.Add(obj);
                });
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void HasValue(int lineIndex, string[] args, IScriptObject value, BooleanObject result)
        {
            if (!ValueType.IsSameType(value))
                throw new InvalidValueTypeScriptException(lineIndex, args, value);

            result.Value = _list.Contains(value);
        }

        public void GetValues(int lineIndex, string[] args, IScriptObject values)
        {
            if (!(values is ListObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, values);

            var listObject = (ListObject)values;

            listObject.Clear(lineIndex, args);
            foreach (IScriptObject value in _list)
            {
                if (!listObject.ValueType.IsSameType(value))
                    throw new InvalidValueTypeScriptException(lineIndex, args, value);

                var obj = value.GetEmptyCopy();
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
                sb.Append(ScriptParser.FormatToString(item)).Append(", ");

            if (sb.Length > 2) sb.Length -= 2;

            return GetKeyword() + "<" + ValueType?.GetKeyword() + ">[ " + sb + " ]";
        }

        public void GetSize(int lineIndex, string[] args, INumberObject result)
            => result.Set(lineIndex, args, new IntObject(_list.Count));

        public void SetSize(int lineIndex, string[] args, INumberObject value)
        {
            int newSize = (int)value.GetValue();

            if (newSize < 0)
                throw new IndexOutOfRangeScriptException(lineIndex, args, newSize);

            while (_list.Count < newSize)
                _list.Add(ValueType.GetEmptyCopy());

            while (_list.Count > newSize)
                _list.RemoveAt(_list.Count - 1);
        }

        public void Sort(int lineIndex, string[] args) => _list.Sort();
        public void Reverse(int lineIndex, string[] args) => _list.Reverse();

        public void Shuffle(int lineIndex, string[] args) => Shuffle(new Random());
        public void Shuffle(int lineIndex, string[] args, INumberObject seed)
        {
            var random = new Random(Convert.ToInt32(seed.GetValue()));
            Shuffle(random);
        }

        private void Shuffle(Random random)
        {
            int n = _list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                Swap(i, j);
            }
        }

        public void Swap(int lineIndex, string[] args, INumberObject first, INumberObject second)
        {
            int firstIndex = Convert.ToInt32(first.GetValue());
            int secondIndex = Convert.ToInt32(second.GetValue());

            if (firstIndex < 0 || firstIndex >= _list.Count)
                throw new IndexOutOfRangeScriptException(lineIndex, args, firstIndex);
            if (secondIndex < 0 || secondIndex >= _list.Count)
                throw new IndexOutOfRangeScriptException(lineIndex, args, secondIndex);

            Swap(firstIndex, secondIndex);
        }

        private void Swap(int first, int next) => (_list[next], _list[first]) = (_list[first], _list[next]);
    }
}
