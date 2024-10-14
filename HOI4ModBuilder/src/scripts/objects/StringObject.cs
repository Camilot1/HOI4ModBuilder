using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Text;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class StringObject : IStringObject, IPrimitiveObject
    {
        public string Value { get; set; }
        public object GetValue() => Value;

        public StringObject() : this("") { }
        public StringObject(string value) { Value = value; }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is StringObject;

        public IScriptObject GetEmptyCopy() => new StringObject();
        public IScriptObject GetCopy() => new StringObject(Value);

        public IScriptObject GetValueType() => new StringObject();
        public string GetKeyword() => StringDeclarator.GetKeyword();
        public override string ToString() => GetKeyword() + "(" + Value + ")";
        public override bool Equals(object obj)
            => obj is StringObject @object &&
                   Value == @object.Value;
        public override int GetHashCode() => Value.GetHashCode();

        public void Set(int lineIndex, string[] args, IScriptObject value) => Value = value.GetValue().ToString();
        public void Add(int lineIndex, string[] args, IScriptObject value) => Value += value.GetValue().ToString();

        public void Get(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            int index;
            if (key is INumberObject numberObject)
                index = (int)numberObject.GetValue();
            else throw new InvalidKeyTypeScriptException(lineIndex, args, key);

            if (index < 0 || index >= Value.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, index);

            if (value is INumberObject)
            {
                var charObject = new CharObject(Value[index]);
                value.Set(lineIndex, args, charObject);
            }
            else throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void HasValue(int lineIndex, string[] args, IScriptObject value, BooleanObject result)
        {
            string findValue;
            if (value is INumberObject || value is IStringObject)
                findValue = value.GetValue().ToString();
            else throw new InvalidValueTypeScriptException(lineIndex, args, value);

            var flag = Value.Contains(findValue);
            result.Set(lineIndex, args, new BooleanObject(flag));
        }

        public void GetValues(int lineIndex, string[] args, IScriptObject values)
        {
            if (!(values is IListObject listObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, values);

            if (!(listObject.GetValueType() is INumberObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, listObject);

            var chars = Value.ToCharArray();

            foreach (var ch in chars)
                listObject.Add(lineIndex, args, new CharObject(ch));
        }

        public IScriptObject GetKeyType() => new CharObject();

        public void Put(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (!(key is INumberObject))
                throw new InvalidKeyTypeScriptException(lineIndex, args, key);

            if (!(value is INumberObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, value);

            int keyIndex = (int)key.GetValue();
            if (keyIndex < 0 || keyIndex >= Value.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, keyIndex);

            int valueIndex = (int)value.GetValue();
            if (valueIndex < 0 || valueIndex > char.MaxValue)
                throw new IndexOutOfRangeScriptException(lineIndex, args, valueIndex);

            string before = Value.Substring(0, keyIndex);
            string after = Value.Substring(keyIndex + 1, Value.Length - keyIndex);

            Value = before + (char)valueIndex + after;
        }

        public void HasKey(int lineIndex, string[] args, IScriptObject key, BooleanObject result)
        {
            if (key is INumberObject)
                result.Value = Value.Contains((char)key.GetValue() + "");
            else if (key is IStringObject stringObj)
                result.Value = Value.Contains(stringObj.GetValue().ToString());
            else
                throw new InvalidKeyTypeScriptException(lineIndex, args, key);
        }

        public void GetKeys(int lineIndex, string[] args, IScriptObject keys)
        {
            if (!(keys is IListObject listObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, keys);

            if (!(listObject.GetValueType() is INumberObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, listObject);

            for (int i = 0; i < Value.Length; i++)
                listObject.Add(lineIndex, args, new IntObject(i));
        }

        public void Insert(int lineIndex, string[] args, IScriptObject key, IScriptObject value)
        {
            if (!(key is INumberObject))
                throw new InvalidKeyTypeScriptException(lineIndex, args, key);

            int index = (int)value.GetValue();
            if (index < 0 || index >= Value.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, index);

            string valueObj;
            if (value is INumberObject numberObj)
                valueObj = "" + (char)numberObj.GetValue();
            else
                valueObj = value.GetValue().ToString();


            Value = Value.Insert(index, valueObj);
        }

        public void RemoveAt(int lineIndex, string[] args, INumberObject value)
        {
            int index = (int)value.GetValue();
            if (index < 0 || index >= Value.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, index);

            Value = Value.Remove(index, 1);
        }

        public void GetSize(int lineIndex, string[] args, INumberObject result)
            => result.Set(lineIndex, args, new IntObject(Value.Length));

        public void Clear(int lineIndex, string[] args) => Value = "";

        public void SetSize(int lineIndex, string[] args, INumberObject value)
        {
            int size = Value.Length;
            int newSize = (int)value.GetValue();

            if (newSize < 0)
                throw new IndexOutOfRangeScriptException(lineIndex, args, newSize);

            if (size < newSize)
                Value = Value.Substring(0, newSize);
            else if (newSize > size)
            {
                var sb = new StringBuilder();

                sb.Append(Value);
                for (int i = size; i < newSize; i++)
                    sb.Append(' ');

                Value = sb.ToString();
            }
        }

        public void Trim(int lineIndex, string[] args) => Value = Value.Trim();

        public bool IsGreaterThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (!(relativeObject is IStringObject stringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            bool checkResult = string.Compare(Value, Convert.ToString(stringObject.GetValue())) > 0;
            result.Set(lineIndex, args, new BooleanObject(checkResult));

            return checkResult;
        }

        public bool IsGreaterThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (!(relativeObject is IStringObject stringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            bool checkResult = string.Compare(Value, Convert.ToString(stringObject.GetValue())) >= 0;
            result.Set(lineIndex, args, new BooleanObject(checkResult));

            return checkResult;
        }

        public bool IsLowerThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (!(relativeObject is IStringObject stringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            bool checkResult = string.Compare(Value, Convert.ToString(stringObject.GetValue())) < 0;
            result.Set(lineIndex, args, new BooleanObject(checkResult));

            return checkResult;
        }

        public bool IsLowerThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (!(relativeObject is IStringObject stringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            bool checkResult = string.Compare(Value, Convert.ToString(stringObject.GetValue())) <= 0;
            result.Set(lineIndex, args, new BooleanObject(checkResult));

            return checkResult;
        }

        public bool IsEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (!(relativeObject is IStringObject stringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            bool checkResult = Value == Convert.ToString(stringObject.GetValue());
            result.Set(lineIndex, args, new BooleanObject(checkResult));

            return checkResult;
        }

        public bool IsNotEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (!(relativeObject is IStringObject stringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            bool checkResult = Value != Convert.ToString(stringObject.GetValue());
            result.Set(lineIndex, args, new BooleanObject(checkResult));

            return checkResult;
        }
    }
}
