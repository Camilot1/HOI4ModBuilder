using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class IntObject : IScriptObject, INumberObject, IPrimitiveObject
    {
        public int Value { get; set; }
        public object GetValue() => Value;

        public IntObject() { }
        public IntObject(int value)
        {
            Value = value;
        }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is IntObject;
        public IScriptObject GetEmptyCopy() => new IntObject();
        public IScriptObject GetCopy() => new IntObject(Value);
        public string GetKeyword() => IntDeclarator.GetKeyword();
        public override string ToString() => GetKeyword() + "(" + Value + ")";
        public override bool Equals(object obj)
            => obj is IntObject @object &&
                   Value == @object.Value;
        public override int GetHashCode() => Value.GetHashCode();

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = booleanObject.Value ? 1 : 0;
            else if (value is CharObject charObject)
                Value = charObject.Value;
            else if (value is IntObject intObject)
                Value = intObject.Value;
            else if (value is FloatObject floatObject)
                Value = (int)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Add(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value += booleanObject.Value ? 1 : 0;
            else if (value is CharObject charObject)
                Value += charObject.Value;
            else if (value is IntObject intObject)
                Value += intObject.Value;
            else if (value is FloatObject floatObject)
                Value += (int)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }
        public void Subtract(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value -= booleanObject.Value ? 1 : 0;
            else if (value is CharObject charObject)
                Value -= charObject.Value;
            else if (value is IntObject intObject)
                Value -= intObject.Value;
            else if (value is FloatObject floatObject)
                Value -= (int)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Multiply(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value *= booleanObject.Value ? 1 : 0;
            else if (value is CharObject charObject)
                Value *= charObject.Value;
            else if (value is IntObject intObject)
                Value *= intObject.Value;
            else if (value is FloatObject floatObject)
                Value *= (int)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Divide(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is CharObject charObject)
                Value /= charObject.Value;
            else if (value is IntObject intObject)
                Value /= intObject.Value;
            else if (value is FloatObject floatObject)
                Value /= (int)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Modulo(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is CharObject charObject)
                Value %= charObject.Value;
            else if (value is IntObject intObject)
                Value %= intObject.Value;
            else if (value is FloatObject floatObject)
                Value %= (int)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public bool IsGreaterThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value > 1) : (Value > 0);
            else if (relativeObject is CharObject charObject)
                result.Value = Value > charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value > intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value > floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsGreaterThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value >= 1) : (Value >= 0);
            else if (relativeObject is CharObject charObject)
                result.Value = Value >= charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value >= intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value >= floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsLowerThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value < 1) : (Value < 0);
            else if (relativeObject is CharObject charObject)
                result.Value = Value < charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value < intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value < floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsLowerThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value <= 1) : (Value <= 0);
            else if (relativeObject is CharObject charObject)
                result.Value = Value <= charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value <= intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value <= floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value == 1) : (Value == 0);
            else if (relativeObject is CharObject charObject)
                result.Value = Value == charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value == intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value == floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsNotEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value != 1) : (Value != 0);
            else if (relativeObject is CharObject charObject)
                result.Value = Value != charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value != intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value != floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is IScriptObject scriptObject)
                return Value.CompareTo(scriptObject.GetValue());
            else return 0;
        }
    }
}
