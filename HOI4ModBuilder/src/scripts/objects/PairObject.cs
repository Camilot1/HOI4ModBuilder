
using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using OpenTK.Input;
using YamlDotNet.Core.Tokens;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class PairObject : IPairObject
    {
        public IScriptObject KeyType;
        private IScriptObject _key;

        public IScriptObject ValueType;
        private IScriptObject _value;

        public object GetKey() => _key;
        public object GetValue() => _value;

        public PairObject()
        {
            KeyType = new AnyObject();
            ValueType = new AnyObject();
        }

        public PairObject(IScriptObject key, IScriptObject value)
        {
            KeyType = key;
            ValueType = value;
        }

        public override string ToString() => $"{GetKeyword()}<{KeyType?.GetKeyword()}, {ValueType?.GetKeyword()}>({_key}, {_value})";

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is PairObject;

        public IScriptObject GetEmptyCopy() => new PairObject(KeyType.GetEmptyCopy(), ValueType.GetEmptyCopy());

        public IScriptObject GetCopy()
        {
            var pair = new PairObject(KeyType.GetEmptyCopy(), ValueType.GetEmptyCopy());

            if (KeyType is IPrimitiveObject)
                pair._key = _key?.GetCopy();
            else
                pair._key = _key;

            if (ValueType is IPrimitiveObject)
                pair._value = _value?.GetCopy();
            else
                pair._value = _value;

            return pair;
        }

        public string GetKeyword() => PairDeclarator.GetKeyword();

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (IsSameType(value) || CanBeConvertedFrom(value))
            {
                var copy = (IPairObject)value.GetCopy();
                _key = (IScriptObject)copy.GetKey();
                _value = (IScriptObject)copy.GetValue();
            }
            else if (value == null)
                throw new VariableIsNotDeclaredScriptException(lineIndex, args, lineIndex);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }


        public bool CanBeConvertedFrom(IScriptObject value)
        {
            if (!(value is IPairObject pairObject))
                return false;

            if (IsSameType(pairObject))
                return true;

            return KeyType.IsSameType(((IGetKeyObject)pairObject).GetKeyType()) &&
                ValueType.IsSameType(((IGetValueObject)pairObject).GetValueType());
        }

        public IScriptObject GetKeyType() => KeyType;

        public void GetKey(int lineIndex, string[] args, IScriptObject key)
        {
            if (_key is IPrimitiveObject)
                key.Set(lineIndex, args, _key.GetCopy());
            else
                key.Set(lineIndex, args, _key);
        }

        public IScriptObject GetValueType() => ValueType;

        public void GetValue(int lineIndex, string[] args, IScriptObject value)
        {
            if (_value is IPrimitiveObject)
                value.Set(lineIndex, args, _value.GetCopy());
            else
                value.Set(lineIndex, args, _value);
        }

        public void SetKey(int lineIndex, string[] args, IScriptObject key)
        {
            SetKey(key);
        }

        public void SetKey(IScriptObject key)
        {
            if (key is IPrimitiveObject)
                _key = key.GetCopy();
            else
                _key = key;
        }

        public void SetValue(int lineIndex, string[] args, IScriptObject value)
        {
            SetValue(value);
        }

        public void SetValue(IScriptObject value)
        {
            if (value is IPrimitiveObject)
                _value = value.GetCopy();
            else
                _value = value;
        }
    }
}
